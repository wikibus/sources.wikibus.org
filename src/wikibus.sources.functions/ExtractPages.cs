using System.Linq;
using Anotar.Serilog;
using PdfSharp.Pdf.IO;
using Wikibus.Sources.EF;

namespace Wikibus.Sources.Functions
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Argolis.Models;
    using Events;
    using ImageMagick;
    using Images;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Sources;

    public class ExtractPages
    {
        private readonly ISourcesRepository sources;
        private readonly ISourceImageService imageService;
        private readonly IUriTemplateMatcher matcher;
        private readonly ISourceContext sourcesContext;
        private readonly ISourcesPersistence persistence;

        public ExtractPages(
            ISourcesRepository sources,
            ISourceImageService imageService,
            IUriTemplateMatcher matcher,
            ISourceContext sourcesContext,
            ISourcesPersistence persistence)
        {
            this.sources = sources;
            this.imageService = imageService;
            this.matcher = matcher;
            this.sourcesContext = sourcesContext;
            this.persistence = persistence;
            this.Client = new HttpClient();
        }

        public HttpClient Client { get; set; }

        [FunctionName("ExtractPages")]
        public async Task Run([QueueTrigger(PdfUploaded.Queue)] PdfUploaded pdf)
        {
            LogTo.Information($"Extracting pages from {pdf.Name}.pdf");

            var sourceUri = new Uri(pdf.SourceId);
            var source = await this.sources.GetBrochure(sourceUri);
            if (source == null)
            {
                LogTo.Warning("Source {0} not found", sourceUri);
                return;
            }

            var sourceId = this.matcher.Match<Brochure>(sourceUri).Get<int>("id");

            if (source.HasNonLegacyImage)
            {
                LogTo.Warning("Source {0} already has images. Skipping.", sourceUri);
                return;
            }

            var pdfRequestStream = await this.Client.GetStreamAsync(pdf.BlobUri);
            using (var pdfContents = new MemoryStream())
            {
                await pdfRequestStream.CopyToAsync(pdfContents);
                pdfContents.Seek(0, SeekOrigin.Begin);

                var settings = new MagickReadSettings
                {
                    Density = new Density(300, 300),
                    FrameCount = 1,
                    FrameIndex = 0,
                };

                using (MagickImageCollection images = new MagickImageCollection())
                {
                    images.Read(pdfContents, settings);

                    var image = images.First();
                    using (var imageStream = new MemoryStream())
                    {
                        image.Format = MagickFormat.Jpeg;
                        image.Write(imageStream);
                        imageStream.Seek(0, SeekOrigin.Begin);

                        await this.imageService.AddImage(sourceId, $"{sourceId}_cover", imageStream);
                        await this.sourcesContext.SaveChangesAsync();
                    }
                }

                if (!source.Pages.HasValue)
                {
                    LogTo.Information("Brochure has no page count. Setting page count from PDF");

                    pdfContents.Seek(0, SeekOrigin.Begin);
                    var pdfDoc = PdfReader.Open(pdfContents);
                    source.Pages = pdfDoc.PageCount;
                    await this.persistence.SaveBrochure(source);
                }
            }
        }
    }
}
