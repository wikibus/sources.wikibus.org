using System;
using System.Threading.Tasks;
using Argolis.Models;
using Argolis.Nancy;
using Nancy;
using Nancy.ModelBinding;
using Wikibus.Common;
using Wikibus.Nancy;
using Wikibus.Sources.EF;

namespace Wikibus.Sources.Nancy
{
    public sealed class SourceStorageLocationModule : ArgolisModule
    {
        private readonly ISourceContext sourcesContext;

        public SourceStorageLocationModule(
            IModelTemplateProvider provider,
            ISourceContext sourcesContext)
            : base(provider)
        {
            this.RequiresPermissions(Permissions.WriteSources);

            this.sourcesContext = sourcesContext;
            this.Put<StorageLocation>(this.UpdateStorageLocation);
        }

        private async Task<dynamic> UpdateStorageLocation(dynamic args)
        {
            int brochureId = args.brochureId;
            var brochure = await this.sourcesContext.Brochures.FindAsync(brochureId);
            var newLocation = this.BindAndValidate<StorageLocation>(new BindingConfig
            {
                BodyOnly = true,
            });

            if (this.Context.ModelValidationResult.IsValid == false)
            {
                return HttpStatusCode.BadRequest;
            }

            brochure.FileCabinet = newLocation.FilingCabinetId;
            brochure.FileOffset = newLocation.Position;

            await this.sourcesContext.SaveChangesAsync();

            return HttpStatusCode.NoContent;
        }
    }
}
