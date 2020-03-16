using System.IO;
using System.Threading.Tasks;
using Anotar.Serilog;
using NullGuard;
using Wikibus.Sources.EF;

namespace Wikibus.Sources.Images
{
    public class SourceImageService : ISourceImageService
    {
        private readonly IImageStorage storage;
        private readonly ISourceContext data;

        public SourceImageService([AllowNull] IImageStorage storage, [AllowNull] ISourceContext data)
        {
            this.storage = storage;
            this.data = data;
        }

        public async Task AddImage(int id, string name, Stream image)
        {
            LogTo.Information("Uploading image {0} for source={1}", name, id);
            var source = await this.data.Sources.FindAsync(id);

            var result = await this.storage.UploadImage(name, image);
            source.Images.Add(new ImageEntity
            {
                ExternalId = result.ExternalId,
                OriginalUrl = result.Original,
                ThumbnailUrl = result.Thumbnail
            });
        }

        public Task DeleteImage(string externalId)
        {
            LogTo.Information("Deleting image {0}", externalId);
            return this.storage.DeleteImage(externalId);
        }
    }
}
