using System;
using System.IO;
using System.Threading.Tasks;
using Anotar.Serilog;

namespace wikibus.storage
{
    public class LocalFileStorage : IFileStorage
    {
        public async Task<Uri> UploadFile(string name, string folder, string contentType, Stream contents)
        {
            LogTo.Debug("Uploading file {0} to folder {1}", name, folder);

            var fileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            using (var file = File.Create(fileName))
            {
                await contents.CopyToAsync(file);
            }

            LogTo.Debug("Saved as {0}", fileName);
            return new Uri(fileName);
        }
    }
}
