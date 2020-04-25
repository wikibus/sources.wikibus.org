using System;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Serilog;
using Microsoft.EntityFrameworkCore;
using Nancy;
using Nancy.ModelBinding;
using Wikibus.Common;
using Wikibus.Nancy;
using Wikibus.Nancy.Requests;
using Wikibus.Sources.EF;
using Wikibus.Sources.Images;

namespace Wikibus.Sources.Nancy
{
    /// <summary>
    /// Servers images of sources over HTTP
    /// </summary>
    public class SourceImagesModule : NancyModule
    {
        private readonly SourceImageService imageService;
        private readonly IImageStorage storage;
        private readonly ISourceContext data;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceImagesModule"/> class.
        /// </summary>
        public SourceImagesModule(
            IImageStorage storage,
            ISourceContext data)
        {
            this.RequiresAnyPermissions(Permissions.WriteSources, Permissions.AdminSources);

            this.imageService = new SourceImageService(storage, data);
            this.storage = storage;
            this.data = data;

            this.Post("/book/{id}/images", request => this.UploadImages((int)request.id));
            this.Post("/brochure/{id}/images", request => this.UploadImages((int)request.id));
            this.Delete("image/{publicId*}", this.DeleteImage);
            this.Post("image/{publicId*}", this.ReorderImages);
        }

        private async Task<dynamic> HandleIfAllowed(SourceEntity source, Func<Task<dynamic>> handler)
        {
            if (source == null)
            {
                return HttpStatusCode.NotFound;
            }

            if (!this.Context.CurrentUser.HasPermission(Permissions.AdminSources)
                && source.User != this.Context.CurrentUser.GetNameClaim())
            {
                return HttpStatusCode.Forbidden;
            }

            return await handler();
        }

        private async Task<dynamic> ReorderImages(dynamic request)
        {
            string publicId = request.publicId;
            var result = await (from image in this.data.Images
                join source in this.data.Sources on image.SourceId equals source.Id
                where image.ExternalId == publicId
                select new
                {
                    source,
                }).SingleOrDefaultAsync();

            var images = await this.data.Images.OrderBy(i => i.OrderIndex)
                .Where(i => i.SourceId == result.source.Id).ToListAsync();

            return await this.HandleIfAllowed(result.source, async () =>
            {
                var reorderRequest = this.Bind<ImageOrder>();
                var newIndex = (short)reorderRequest.OrderIndex;
                if (newIndex < 1)
                {
                    newIndex = 1;
                }

                if (newIndex > images.Count)
                {
                    newIndex = (short)images.Count;
                }

                LogTo.Information("Applying order {0} to image {1}", newIndex, publicId);

                var movedImage = images.Find(i => i.ExternalId == publicId);
                images.Remove(movedImage);
                images.Insert(newIndex - 1, movedImage);

                for (short i = 1; i <= images.Count; i++)
                {
                    var image = images[i - 1];
                    image.OrderIndex = i;
                }

                await this.data.SaveChangesAsync();

                return HttpStatusCode.OK;
            });
        }

        private async Task<dynamic> DeleteImage(dynamic request)
        {
            string publicId = request.publicId;
            var result = await (from image in this.data.Images
                join source in this.data.Sources on image.SourceId equals source.Id
                where image.ExternalId == publicId
                select new
                {
                    image,
                    source,
                }).SingleOrDefaultAsync();

            return await this.HandleIfAllowed(result.source, async () =>
            {
                if (result.image == null)
                {
                    return HttpStatusCode.NoContent;
                }

                if (!await this.storage.DeleteImage(publicId))
                {
                    return HttpStatusCode.InternalServerError;
                }

                this.data.Images.Remove(result.image);
                await this.data.SaveChangesAsync();

                return HttpStatusCode.OK;
            });
        }

        private async Task<dynamic> UploadImages(int sourceId)
        {
            var source = await this.data.Sources.FindAsync(sourceId);

            return await this.HandleIfAllowed(source, async () =>
            {
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
            });
        }
    }
}
