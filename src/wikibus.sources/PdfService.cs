using System.IO;
using System.Threading.Tasks;
using Anotar.Serilog;
using Argolis.Models;
using Wikibus.Sources.Events;
using wikibus.storage;

namespace Wikibus.Sources
{
    public class PdfService : IPdfService
    {
        private readonly IFileStorage fileStorage;
        private readonly IStorageQueue queue;
        private readonly IWishlistPersistence wishlistPersistence;
        private readonly IUriTemplateMatcher matcher;

        public PdfService(
            IFileStorage fileStorage,
            IStorageQueue queue,
            IWishlistPersistence wishlistPersistence,
            IUriTemplateMatcher matcher)
        {
            this.fileStorage = fileStorage;
            this.queue = queue;
            this.wishlistPersistence = wishlistPersistence;
            this.matcher = matcher;
        }

        public async Task UploadResourcePdf<T>(T resource, string name, Stream stream)
            where T : Source
        {
            var id = this.matcher.Match<T>(resource.Id).Get<int>("id");
            var uri = await this.fileStorage.UploadFile(name, $"sources{id}", MimeMapping.KnownMimeTypes.Pdf, stream);

            resource.SetContent(uri, (int)stream.Length);
        }

        public async Task NotifyPdfUploaded<T>(T resource, string name)
            where T : Source
        {
            LogTo.Information("Notifying PDF upload for {0}", resource.Id);

            var id = this.matcher.Match<T>(resource.Id).Get<int>("id");

            var pdfUploaded = new PdfUploaded
            {
                BlobUri = resource.Content.ContentUrl.ToString(),
                Name = name,
                SourceId = resource.Id.ToString(),
            };
            await this.queue.AddMessage(PdfUploaded.Queue, pdfUploaded);

            await this.wishlistPersistence.MarkDone(id);
        }
    }
}
