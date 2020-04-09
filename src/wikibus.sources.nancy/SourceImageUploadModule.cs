using System.Linq;
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
            this.RequiresAnyPermissions(Permissions.WriteSources, Permissions.AdminSources);

            this.imageService = new SourceImageService(storage, data);
            this.data = data;

            this.Post("/book/{id}/images", request => this.UploadImages((int)request.id));
            this.Post("/brochure/{id}/images", request => this.UploadImages((int)request.id));
        }

        private HttpStatusCode? CheckSource(SourceEntity source)
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

            return null;
        }

        private async Task<dynamic> UploadImages(int sourceId)
        {
            var source = await this.data.Sources.FindAsync(sourceId);

            var result = this.CheckSource(source);
            if (result.HasValue)
            {
                return result;
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
