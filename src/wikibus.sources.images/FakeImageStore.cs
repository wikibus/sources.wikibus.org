using System.IO;
using System.Threading.Tasks;

namespace Wikibus.Sources.Images
{
    public class FakeImageStore : IImageStorage
    {
        public Task<ImageUploadResult> UploadImage(string name, Stream fileStream)
        {
            return Task.FromResult(new ImageUploadResult(
                "https://loremflickr.com/1024/768/bus",
                "https://loremflickr.com/320/240/bus",
                "foo"));
        }

        public Task<bool> DeleteImage(string externalId)
        {
            return Task.FromResult(true);
        }
    }
}
