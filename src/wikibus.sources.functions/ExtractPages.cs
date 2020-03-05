using ImageMagick;

namespace Wikibus.Sources.Functions
{
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Argolis.Models;
    using Events;
    using Images;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Sources;

    public class ExtractPages
    {
        private readonly ISourceImageService imageService;
        private readonly IUriTemplateMatcher matcher;
        private readonly HttpClient httpClient;

        public ExtractPages(ISourceImageService imageService, IUriTemplateMatcher matcher, HttpClient httpClient)
        {
            this.imageService = imageService;
            this.matcher = matcher;
            this.httpClient = httpClient;
        }

        public ExtractPages(ISourceImageService imageService, IUriTemplateMatcher matcher)
            : this(imageService, matcher, new HttpClient())
        {
        }

        [FunctionName("ExtractPages")]
        public async Task Run(
            [QueueTrigger("pdf-uploads")]PdfUploaded pdf,
            ILogger log)
        {
            var sourceId = this.matcher.Match<Brochure>(pdf.SourceId).Get<int>("id");

            var pdfContents = await this.httpClient.GetStreamAsync(pdf.BlobUri);

            log.LogInformation($"Extracting pages from {pdf.Name}.pdf");
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
