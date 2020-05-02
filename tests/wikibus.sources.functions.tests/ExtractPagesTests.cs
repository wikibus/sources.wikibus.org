using Shouldly;
using Wikibus.Sources.EF;
using Wikibus.Sources.Images;

namespace Wikibus.Sources.Functions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Argolis.Models;
    using Events;
    using Functions;
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
        private readonly ISourceContext context;
        private readonly ISourcesPersistence persistence;

        public ExtractPagesTests()
        {
            this.persistence = Substitute.For<ISourcesPersistence>();
            this.sourceRepository = Substitute.For<ISourcesRepository>();
            this.context = Substitute.For<ISourceContext>();
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
                matcher,
                this.context,
                this.persistence);

            this.functions.Client = this.httpMessageHandler.ToHttpClient();
        }

        [Fact]
        public async Task ConvertsCoverPage()
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
            this.imageService.AddedImages.ShouldBe(1);
            this.context.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task WhenBrochureHasNoPageCount_SetsFromPdf()
        {
            // given
            var brochure = new Brochure
            {
                Id = new Uri(SourceId),
            };
            this.sourceRepository.GetBrochure(Arg.Any<Uri>())
                .Returns(Task.FromResult(brochure));
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
            await this.persistence
                .Received(1)
                .SaveBrochure(Arg.Is<Brochure>(b => b.Pages == 9));
        }

        [Fact]
        public async Task WhenBrochureHasPageCount_DoesNotModify()
        {
            // given
            var brochure = new Brochure
            {
                Id = new Uri(SourceId),
                Pages = 8,
            };
            this.sourceRepository.GetBrochure(Arg.Any<Uri>())
                .Returns(Task.FromResult(brochure));
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
            await this.persistence
                .Received(0)
                .SaveBrochure(Arg.Any<Brochure>(), Arg.Any<bool>());
        }

        [Fact]
        public async Task WhenBrochureHasNonLegacyImages_DoesNothing()
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
