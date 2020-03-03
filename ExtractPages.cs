using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Argolis.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Ghostscript.NET.Rasterizer;
using Wikibus.Sources;
using Wikibus.Sources.Images;

namespace wikibus.sources.functions.pdf2img
{
    public class ExtractPages
    {
        private readonly SourceImageService imageService;
        private readonly IUriTemplateMatcher matcher;

        public ExtractPages(SourceImageService imageService, IUriTemplateMatcher matcher)
        {
            this.imageService = imageService;
            this.matcher = matcher;
        }

        [FunctionName("ExtractPages")]
        public async Task Run(
            [BlobTrigger("sources/{name}.pdf")]Stream pdf,
            string name,
            IDictionary<string, string> metaData,
            ILogger log)
        {
            var sourceUri = new Uri(metaData["Source"]);
            var sourceId = this.matcher.Match<Brochure>(sourceUri).Get<int>("id");

            log.LogInformation($"Extracting pages from {name}.pdf");
            using (var rasterizer = new GhostscriptRasterizer())
            {
                rasterizer.Open(pdf);
                for (int pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                {
                    using (var imageStream = new MemoryStream())
                    {
                        var image = rasterizer.GetPage(96, 96, pageNumber);
                        image.Save(imageStream, ImageFormat.Jpeg);
                        imageStream.Seek(0, SeekOrigin.Begin);

                        await this.imageService.AddImage(sourceId, $"{sourceId}_{pageNumber}", imageStream);
                    }
                }
            }
        }
    }
}
