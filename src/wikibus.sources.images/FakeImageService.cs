using System;
using System.IO;
using System.Threading.Tasks;
using Anotar.Serilog;

namespace Wikibus.Sources.Images
{
    public class FakeImageService : ISourceImageService
    {
        public int AddedImages { get; private set;  }

        public async Task AddImage(int id, string name, Stream image)
        {
            this.AddedImages++;

            var tempFilePath = Path.GetTempFileName() + ".jpg";
            LogTo.Debug($"Writing image to {tempFilePath}");

            using var tempFile = File.OpenWrite(tempFilePath);
            await image.CopyToAsync(tempFile);
        }

        public Task DeleteImage(string externalId)
        {
            throw new NotImplementedException();
        }
    }
}
