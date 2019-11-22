using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nancy;
using Wikibus.Sources.EF;
using Wikibus.Sources.Images;

namespace Wikibus.Sources.Nancy
{
    /// <summary>
    /// Servers images of sources over HTTP
    /// </summary>
    public class SourceImageDeleteModule : NancyModule
    {
        private readonly IImageStorage storage;
        private readonly ISourceContext data;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceImagesModule"/> class.
        /// </summary>
        public SourceImageDeleteModule(
            IImageStorage storage,
            ISourceContext data)
        {
            this.storage = storage;
            this.data = data;

            this.Delete("image/{publicId*}", request => this.DeleteImage(request.publicId));
        }

        private async Task<dynamic> DeleteImage(string publicId)
        {
            var image = await this.data.Images.Where(i => i.ExternalId == publicId).SingleOrDefaultAsync();

            if (image == null)
            {
                return HttpStatusCode.NoContent;
            }

            if (!await this.storage.DeleteImage(publicId))
            {
                return HttpStatusCode.InternalServerError;
            }

            this.data.Images.Remove(image);
            await this.data.SaveChangesAsync();

            return HttpStatusCode.OK;
        }
    }
}
