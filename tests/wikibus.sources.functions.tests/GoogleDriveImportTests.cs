using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Apis.Drive.v3.Data;
using MockQueryable.NSubstitute;
using NSubstitute;
using Wikibus.Sources.EF;
using Xunit;

namespace Wikibus.Sources.Functions.Tests
{
    public class GoogleDriveImportTests
    {
        private readonly GoogleDriveImport sut;
        private readonly IDriveServiceFacade drive;
        private readonly ISourceContext context;
        private readonly IPdfService pdfService;
        private readonly ISourcesPersistence persistence;

        public GoogleDriveImportTests()
        {
            this.context = Substitute.For<ISourceContext>();
            this.drive = Substitute.For<IDriveServiceFacade>();
            this.pdfService = Substitute.For<IPdfService>();
            this.persistence = Substitute.For<ISourcesPersistence>();

            this.sut = new GoogleDriveImport(
                this.drive,
                this.context,
                this.persistence,
                this.pdfService);
        }

        [Fact]
        public async Task CreatesBrochureForCorrectUser()
        {
            // given
            var users = new List<UserEntity>
            {
                new UserEntity { UserId = "HasNoFiles", DriveImportFolder = "EmptyFolder" },
                new UserEntity { UserId = "HasSomeFiles", DriveImportFolder = "FooBar" }
            };
            var dbSet = users.AsQueryable().BuildMockDbSet();
            this.context.Users.Returns(dbSet);
            this.drive.FindFiles(Arg.Any<IEnumerable<string>>()).Returns(this.TestFiles());

            // when
            await this.sut.Run(new HttpRequestMessage());

            // then
            await this.persistence
                .Received(1)
                .CreateBrochure(Arg.Any<Brochure>(), "HasSomeFiles");
           await this.persistence.Received(1)
                .CreateBrochure(Arg.Any<Brochure>(), Arg.Any<string>());
        }

        [Fact]
        public async Task CreatesBrochureWithFileExtensionAsName()
        {
            // given
            var users = new List<UserEntity>
            {
                new UserEntity { UserId = "HasSomeFiles", DriveImportFolder = "FooBar" }
            };
            var dbSet = users.AsQueryable().BuildMockDbSet();
            this.context.Users.Returns(dbSet);
            this.drive.FindFiles(Arg.Any<IEnumerable<string>>()).Returns(this.TestFiles());

            // when
            await this.sut.Run(new HttpRequestMessage());

            // then
            await this.persistence
                .Received(1)
                .CreateBrochure(Arg.Is<Brochure>(brochure => brochure.Title == "Ikarus 280"), Arg.Any<string>());
        }

        private IEnumerator<File> TestFiles()
        {
            yield return new File
            {
                Name = "Ikarus 280.pdf",
                Parents = new List<string> { "FooBar" }
            };
        }
    }
}
