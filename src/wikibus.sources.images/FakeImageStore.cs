using System;
using System.IO;
using System.Threading.Tasks;

namespace Wikibus.Sources.Images
{
    public class FakeImageStore : IImageStorage
    {
        private static readonly Random Id = new Random();

        public Task<ImageUploadResult> UploadImage(string name, Stream fileStream)
        {
            return Task.FromResult(new ImageUploadResult(
                "https://loremflickr.com/1024/768/bus",
                "https://loremflickr.com/320/240/bus",
                "foo" + Id.Next()));
        }

        public Task<bool> DeleteImage(string externalId)
        {
            return Task.FromResult(true);
        }
    }
}
