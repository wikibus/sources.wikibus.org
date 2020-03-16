using System;
using System.IO;
using System.Threading.Tasks;
using Wikibus.Sources.Images;
using Xunit.Abstractions;

namespace wikibus.sources.functions.tests
{
    public class FakeImageService : ISourceImageService
    {
        private readonly ITestOutputHelper output;

        public FakeImageService(ITestOutputHelper output)
        {
            this.output = output;
        }

        public int AddedImages { get; private set;  }

        public async Task AddImage(int id, string name, Stream image)
        {
            this.AddedImages++;

            var tempFilePath = Path.GetTempFileName() + ".jpg";
            this.output.WriteLine($"Writing image to {tempFilePath}");

            using var tempFile = File.OpenWrite(tempFilePath);
            await image.CopyToAsync(tempFile);
        }

        public Task DeleteImage(string externalId)
        {
            throw new NotImplementedException();
        }
    }
}
