using System;
using System.Threading.Tasks;
using Argolis.Hydra.Resources;
using Argolis.Models;
using Argolis.Nancy;
using Nancy;
using Nancy.ModelBinding;
using Wikibus.Common;
using Wikibus.Nancy;

namespace Wikibus.Sources.Nancy
{
    public sealed class SourcesUpdateModule : ArgolisModule
    {
        private readonly IUriTemplateExpander expander;

        public SourcesUpdateModule(
            ISourcesPersistence persistence,
            ISourcesRepository repository,
            IModelTemplateProvider modelTemplateProvider,
            IUriTemplateExpander expander)
            : base(modelTemplateProvider)
        {
            this.RequiresPermissions(Permissions.WriteSources);

            this.expander = expander;
            this.Put<Brochure>(async r => await this.PutSingle(persistence.SaveBrochure, repository.GetBrochure));
            using (this.Templates)
            {
                this.Post<Collection<Brochure>>(async r => await this.CreateBrochure(persistence.CreateBrochure, repository.GetBrochure));
            }
        }

        private async Task<dynamic> CreateBrochure(
            Func<Brochure, Task> saveResource,
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

            await saveResource(brochure);

            return this.Negotiate
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Location", brochure.Id.ToString())
                .WithModel(brochure);
        }

        private async Task<T> PutSingle<T>(
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
            await saveResource(brochure);

            return await getResource(brochure.Id);
        }
    }
}
