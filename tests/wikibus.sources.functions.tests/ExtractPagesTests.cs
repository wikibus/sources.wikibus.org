using Shouldly;
using Wikibus.Sources.Images;
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
        private const string SourceId = "http://foo.bar/source";
        private const string Name = "e-citaro.pdf";
        private const string BlobUri = "http://foo.bar/blob";

        private readonly FakeImageService imageService;
        private readonly MockHttpMessageHandler httpMessageHandler;
        private readonly ExtractPages functions;
        private readonly ISourcesRepository sourceRepository;

        public ExtractPagesTests()
        {
            this.sourceRepository = Substitute.For<ISourcesRepository>();
            this.imageService = new FakeImageService();
            var matcher = Substitute.For<IUriTemplateMatcher>();
            this.httpMessageHandler = new MockHttpMessageHandler();

            matcher.Match<Brochure>(Arg.Any<Uri>())
                .Returns(new UriTemplateMatches(new Dictionary<string, object>
                {
                    { "id", 10 },
                }));

            this.sourceRepository.GetBrochure(Arg.Any<Uri>())
                .Returns(Task.FromResult(new Brochure
                {
                    Id = new Uri(SourceId),
                }));

            this.functions = new ExtractPages(
                this.sourceRepository,
                this.imageService,
                matcher);

            this.functions.Client = this.httpMessageHandler.ToHttpClient();
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
            await this.functions.Run(pdfUploaded);

            // then
            this.imageService.AddedImages.ShouldBe(9);
        }

        [Fact]
        public async Task WhenBrochureHasImages_DoesNothing()
        {
            // given
            var brochure = new Brochure
            {
                Id = new Uri(SourceId),
                Images =
                {
                    Members = new[] { new Image(), }
                }
            };
            this.sourceRepository.GetBrochure(Arg.Any<Uri>())
                .Returns(Task.FromResult(brochure));
            var pdfUploaded = new PdfUploaded
            {
                BlobUri = BlobUri,
                Name = Name,
                SourceId = SourceId,
            };

            // when
            await this.functions.Run(pdfUploaded);

            // then
            this.imageService.AddedImages.ShouldBe(0);
        }
    }
}
