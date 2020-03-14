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
        private readonly HttpClient httpClient;

        public ExtractPages(
            ISourcesRepository sources,
            ISourceImageService imageService,
            IUriTemplateMatcher matcher,
            HttpClient httpClient)
        {
            this.sources = sources;
            this.imageService = imageService;
            this.matcher = matcher;
            this.httpClient = httpClient;
        }

        public ExtractPages(
            ISourcesRepository sources,
            ISourceImageService imageService,
            IUriTemplateMatcher matcher)
            : this(sources, imageService, matcher, new HttpClient())
        {
        }

        [FunctionName("ExtractPages")]
        public async Task Run([QueueTrigger(PdfUploaded.Queue)] PdfUploaded pdf)
        {
            var sourceUri = new Uri(pdf.SourceId);
            var sourceId = this.matcher.Match<Brochure>(sourceUri).Get<int>("id");

            var source = await this.sources.GetBrochure(sourceUri);
            if (source.Images.Members.Any())
            {
                return;
            }

            var pdfContents = await this.httpClient.GetStreamAsync(pdf.BlobUri);

            LogTo.Information($"Extracting pages from {pdf.Name}.pdf");
            var settings = new MagickReadSettings
            {
                Density = new Density(300, 300)
            };

            using (MagickImageCollection images = new MagickImageCollection())
            {
                images.Read(pdfContents, settings);

                var pageNumber = 1;
                foreach (var image in images)
                {
                    using (var imageStream = new MemoryStream())
                    {
                        image.Format = MagickFormat.Jpeg;
                        image.Write(imageStream);
                        imageStream.Seek(0, SeekOrigin.Begin);

                        await this.imageService.AddImage(sourceId, $"{sourceId}_{pageNumber}", imageStream);
                    }

                    pageNumber++;
                }
            }
        }
    }
}
