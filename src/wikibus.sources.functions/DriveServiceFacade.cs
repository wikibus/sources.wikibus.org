using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Serilog;
using Google.Apis.Drive.v3;
using Microsoft.Extensions.Configuration;
using File = Google.Apis.Drive.v3.Data.File;

namespace Wikibus.Sources.Functions
{
    public interface IDriveServiceFacade
    {
        Task<IEnumerator<File>> FindFiles(string[] folders);

        Task<Stream> GetFileContents(File file);

        Task MoveFile(File file, string parent);
    }

    public class DriveServiceFacade : IDriveServiceFacade
    {
        private readonly DriveService drive;
        private readonly IConfiguration configuration;

        public DriveServiceFacade(DriveService drive, IConfiguration configuration)
        {
            this.drive = drive;
            this.configuration = configuration;
        }

        public async Task<IEnumerator<File>> FindFiles(string[] folders)
        {
            if (!folders.Any())
            {
                LogTo.Information("No folders to search");
                return new List<File>().GetEnumerator();
            }

            LogTo.Information("Searching for pdfs in {0} folders", folders.Length);

            var request = this.drive.Files.List();
            var parentsQuery = string.Join(" or ", folders.Select(folder => $"'{folder}' in parents"));
            request.Q = $"mimeType='application/pdf' and {parentsQuery}";
            request.Fields = "files(id,name,parents)";

            var result = await request.ExecuteAsync();
            LogTo.Information("Found {0} files", result.Files.Count);

            return result.Files.GetEnumerator();
        }

        public async Task<Stream> GetFileContents(File file)
        {
            var stream = new MemoryStream();
            await this.drive.Files.Get(file.Id).DownloadAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public async Task MoveFile(File file, string parent)
        {
            var update = new File();
            var moveRequest = this.drive.Files.Update(update, file.Id);
            moveRequest.AddParents = this.configuration["Google:Drive:ImportedFolderId"];
            moveRequest.RemoveParents = parent;
            await moveRequest.ExecuteAsync();

            LogTo.Information("Pdf {0} moved to 'done'", file.Name);
        }
    }
}
