using Shouldly;
using wikibus.sources.functions.tests;
using Xunit.Abstractions;

namespace Wikibus.Sources.Functions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Argolis.Models;
    using Events;
    using Functions;
    using Microsoft.Extensions.Logging;
    using MimeMapping;
    using NSubstitute;
    using Resourcer;
    using RichardSzalay.MockHttp;
    using Sources;
    using Xunit;

    public class ExtractPagesTests
    {
        private static readonly Uri SourceId = new Uri("http://foo.bar/source");
        private static readonly string Name = "e-citaro.pdf";
        private static readonly string BlobUri = "http://foo.bar/blob";

        private readonly ILogger log = Substitute.For<ILogger>();
        private readonly FakeImageService imageService;
        private readonly MockHttpMessageHandler httpMessageHandler;
        private readonly ExtractPages functions;

        public ExtractPagesTests(ITestOutputHelper output)
        {
            this.imageService = new FakeImageService(output);
            var matcher = Substitute.For<IUriTemplateMatcher>();
            this.httpMessageHandler = new MockHttpMessageHandler();

            matcher.Match<Brochure>(Arg.Any<Uri>())
                .Returns(new UriTemplateMatches(new Dictionary<string, object>
                {
                    { "id", 10 },
                }));

            this.functions = new ExtractPages(
                this.imageService,
                matcher,
                this.httpMessageHandler.ToHttpClient());
        }

        [Fact]
        public async Task ConvertsPages()
        {
            // given
            this.httpMessageHandler.Expect(BlobUri)
                .Respond(KnownMimeTypes.Pdf, Resource.AsStream("Pdfs/mb1.pdf"));
            var pdfUploaded = new PdfUploaded
            {
                BlobUri = BlobUri,
                Name = Name,
                SourceId = SourceId,
            };

            // when
            await this.functions.Run(pdfUploaded, this.log);

            // then
            this.imageService.AddedImages.ShouldBe(9);
        }
    }
}
