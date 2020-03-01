﻿using System.Linq;
using System.Threading.Tasks;
using Nancy;
using Wikibus.Common;
using Wikibus.Nancy;
using Wikibus.Sources.EF;
using Wikibus.Sources.Images;

namespace Wikibus.Sources.Nancy
{
    /// <summary>
    /// Servers images of sources over HTTP
    /// </summary>
    public class SourceImageUploadModule : NancyModule
    {
        private readonly SourceImageService imageService;
        private readonly ISourceContext data;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceImagesModule"/> class.
        /// </summary>
        public SourceImageUploadModule(
            IImageStorage storage,
            ISourceContext data)
        {
            this.RequiresPermissions(Permissions.WriteSources);

            this.imageService = new SourceImageService(storage, data);
            this.data = data;

            this.Post("/book/{id}/images", request => this.UploadImages((int)request.id));
            this.Post("/brochure/{id}/images", request => this.UploadImages((int)request.id));
        }

        private async Task<dynamic> UploadImages(int sourceId)
        {
            var source = await this.data.Sources.FindAsync(sourceId);

            if (source == null)
            {
                return HttpStatusCode.NotFound;
            }

            foreach (var httpFile in this.Request.Files)
            {
                await this.imageService.AddImage(sourceId, httpFile.Name, httpFile.Value);
            }

            try
            {
                await this.data.SaveChangesAsync();
            }
            catch
            {
                source.Images.ToList()
                    .ForEach(i => this.imageService.DeleteImage(i.ExternalId));

                throw;
            }

            return HttpStatusCode.Accepted;
        }
    }
}
