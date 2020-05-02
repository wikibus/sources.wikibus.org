using System;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Serilog;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Wikibus.Sources.EF;
using File = Google.Apis.Drive.v3.Data.File;

namespace Wikibus.Sources.Functions
{
    public class GoogleDriveImport
    {
        private readonly IDriveServiceFacade drive;
        private readonly ISourceContext context;
        private readonly ISourcesPersistence persistence;
        private readonly IPdfService pdfService;

        public GoogleDriveImport(
            IDriveServiceFacade drive,
            ISourceContext context,
            ISourcesPersistence persistence,
            IPdfService pdfService)
        {
            this.drive = drive;
            this.context = context;
            this.persistence = persistence;
            this.pdfService = pdfService;
        }

        [FunctionName("GoogleDriveImport")]
        public Task Run([TimerTrigger("0 0 * * * *", RunOnStartup = true)] TimerInfo timer)
        {
            return this.DoImport();
        }

        public async Task DoImport()
        {
            var maxTime = DateTime.Now.AddMinutes(4);

            var toScan = await (from user in this.context.Users
                    where user.DriveImportFolder != null
                    select user).ToListAsync();

            var folders = toScan.Select(item => item.DriveImportFolder);

            using (var files = await this.drive.FindFiles(folders.ToArray()))
            {
                while (DateTime.Now < maxTime && files.MoveNext())
                {
                    LogTo.Information("Importing file {0}", files.Current.Name);
                    var currentFolder = (from parent in files.Current.Parents
                        where toScan.Any(i => i.DriveImportFolder == parent)
                        select parent).Single();
                    var userName = toScan.Single(i => i.DriveImportFolder == currentFolder).UserId;

                    var brochure = await this.CreateBrochure(files.Current, userName);
                    await this.UploadPdf(files.Current, brochure);
                    await this.drive.MoveFile(files.Current, currentFolder);
                }
            }
        }

        private async Task<Brochure> CreateBrochure(File file, string userName)
        {
            var title = file.Name.Replace(".pdf", string.Empty);
            var brochure = new Brochure
            {
                Title = title
            };

            await this.persistence.CreateBrochure(brochure, userName);

            LogTo.Information("Created brochure {0}", brochure.Id);
            return brochure;
        }

        private async Task UploadPdf(File file, Brochure brochure)
        {
            var fileStream = await this.drive.GetFileContents(file);

            await this.pdfService.UploadResourcePdf(brochure, file.Name, fileStream);
            await this.persistence.SaveBrochure(brochure, true);
            await this.pdfService.NotifyPdfUploaded(brochure, file.Name);

            LogTo.Information("Uploaded pdf {0}", file.Name);
        }
    }
}
