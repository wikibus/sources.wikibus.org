using System;
using System.Linq;
using System.Threading.Tasks;
using Argolis.Hydra.Resources;
using Argolis.Models;
using Argolis.Nancy;
using Nancy;
using Nancy.ModelBinding;
using Wikibus.Common;
using Wikibus.Nancy;
using Wikibus.Sources.Events;
using wikibus.storage;

namespace Wikibus.Sources.Nancy
{
    public sealed class SourcesUpdateModule : ArgolisModule
    {
        private readonly IUriTemplateExpander expander;
        private readonly IFileStorage fileStorage;
        private readonly IStorageQueue queue;
        private readonly IWishlistPersistence wishlistPersistence;

        public SourcesUpdateModule(
            ISourcesPersistence persistence,
            ISourcesRepository repository,
            IModelTemplateProvider modelTemplateProvider,
            IUriTemplateExpander expander,
            IFileStorage fileStorage,
            IStorageQueue queue,
            IWishlistPersistence wishlistPersistence)
            : base(modelTemplateProvider)
        {
            this.RequiresAnyPermissions(Permissions.WriteSources, Permissions.AdminSources);

            this.expander = expander;
            this.fileStorage = fileStorage;
            this.queue = queue;
            this.wishlistPersistence = wishlistPersistence;
            this.Put<Brochure>(async r =>
                await this.PutSingle(brochure => persistence.SaveBrochure(brochure), repository.GetBrochure));
            this.Post<SourceContent>(
                async r => await this.UploadPdf((int)r.id, repository.GetBrochure, brochure => persistence.SaveBrochure(brochure, true)));
            using (this.Templates)
            {
                this.Post<Collection<Brochure>>(async r =>
                    await this.CreateBrochure(persistence.CreateBrochure, repository.GetBrochure));
            }
        }

        private async Task<dynamic> CreateBrochure(
            Func<Brochure, string, Task> saveResource,
            Func<Uri, Task<Brochure>> getResource)
        {
            var brochure = this.BindAndValidate<Brochure>(
                new BindingConfig
                {
                    BodyOnly = true,
                }, "Id");
            if (this.Context.ModelValidationResult.IsValid == false)
            {
                return HttpStatusCode.BadRequest;
            }

            await saveResource(brochure, this.Context.CurrentUser.GetNameClaim());

            return this.Negotiate
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Location", brochure.Id.ToString())
                .WithModel(await getResource(brochure.Id));
        }

        private async Task<dynamic> PutSingle<T>(
            Func<T, Task> saveResource,
            Func<Uri, Task<T>> getResource)
            where T : Source, new()
        {
            var brochure = this.BindTo(
                new T
                {
                    Id = this.expander.ExpandAbsolute<T>(this.Context.Parameters)
                },
                new BindingConfig { Overwrite = true },
                "Id");

            var currentState = await getResource(brochure.Id);
            if (!this.Context.CurrentUser.HasPermission(Permissions.AdminSources) && !currentState.OwnedBy(this.Context.CurrentUser))
            {
                return HttpStatusCode.Forbidden;
            }

            await saveResource(brochure);

            return await getResource(brochure.Id);
        }

        private async Task<dynamic> UploadPdf(
            int id,
            Func<Uri, Task<Brochure>> getResource,
            Func<Brochure, Task> saveResource)
        {
            var pdf = this.Request.Files
                          .Select(file => new { name = file.Name, stream = file.Value })
                          .FirstOrDefault() ?? new
                      {
                          name = $"{id}.pdf",
                          stream = this.Request.Body
                      };

            var uri = await this.fileStorage.UploadFile(pdf.name, $"sources{id}", MimeMapping.KnownMimeTypes.Pdf, pdf.stream);

            var brochureId = this.expander.ExpandAbsolute<Brochure>(new { id });
            var resource = await getResource(brochureId);
            if (resource == null)
            {
                return HttpStatusCode.NotFound;
            }

            if (!this.Context.CurrentUser.HasPermission(Permissions.AdminSources) && !resource.OwnedBy(this.Context.CurrentUser))
            {
                return HttpStatusCode.Forbidden;
            }

            resource.SetContent(uri, (int)pdf.stream.Length);

            await saveResource(resource);

            var pdfUploaded = new PdfUploaded
            {
                BlobUri = uri.ToString(),
                Name = pdf.name,
                SourceId = brochureId.ToString(),
            };
            await this.queue.AddMessage(PdfUploaded.Queue, pdfUploaded);

            await this.wishlistPersistence.MarkDone(id);

            return this.Response.AsRedirect(brochureId.ToString());
        }
    }
}
