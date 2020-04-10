using System.Linq;
using Anotar.Serilog;

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

        public ExtractPages(
            ISourcesRepository sources,
            ISourceImageService imageService,
            IUriTemplateMatcher matcher)
        {
            this.sources = sources;
            this.imageService = imageService;
            this.matcher = matcher;
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

            if (source.Images.Members.Any())
            {
                LogTo.Warning("Source {0} already has images. Skipping.", sourceUri);
                return;
            }

            var pdfContents = await this.Client.GetStreamAsync(pdf.BlobUri);

            var settings = new MagickReadSettings
            {
                Density = new Density(300, 300),
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
                }
            }
        }
    }
}
