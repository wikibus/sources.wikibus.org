using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wikibus.Sources.EF.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Priv");

            migrationBuilder.EnsureSchema(
                name: "Sources");

            migrationBuilder.CreateSequence(
                name: "ImagesSequenceHiLo",
                schema: "Sources",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "SourcesSequenceHiLo",
                schema: "Sources",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "FileCabinet",
                schema: "Priv",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileCabinet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Magazine",
                schema: "Sources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SubName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Magazine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Source",
                schema: "Sources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: true),
                    Language2 = table.Column<string>(nullable: true),
                    Pages = table.Column<int>(nullable: true),
                    Year = table.Column<short>(nullable: true),
                    Month = table.Column<byte>(nullable: true),
                    Day = table.Column<byte>(nullable: true),
                    Image = table.Column<byte[]>(nullable: false),
                    SourceType = table.Column<string>(nullable: false),
                    BookTitle = table.Column<string>(nullable: true),
                    BookAuthor = table.Column<string>(nullable: true),
                    BookISBN = table.Column<string>(nullable: true),
                    FolderCode = table.Column<string>(nullable: true),
                    FolderName = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    FileCabinet = table.Column<int>(nullable: true),
                    FileOffset = table.Column<int>(nullable: true),
                    MagIssueMagazine = table.Column<int>(nullable: true),
                    MagIssueNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Source_Magazine_MagIssueMagazine",
                        column: x => x.MagIssueMagazine,
                        principalSchema: "Sources",
                        principalTable: "Magazine",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                schema: "Sources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    OriginalUrl = table.Column<string>(nullable: true),
                    ThumbnailUrl = table.Column<string>(nullable: true),
                    ExternalId = table.Column<string>(nullable: true),
                    SourceId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_Source_SourceId",
                        column: x => x.SourceId,
                        principalSchema: "Sources",
                        principalTable: "Source",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_SourceId",
                schema: "Sources",
                table: "Images",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Source_MagIssueMagazine",
                schema: "Sources",
                table: "Source",
                column: "MagIssueMagazine");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileCabinet",
                schema: "Priv");

            migrationBuilder.DropTable(
                name: "Images",
                schema: "Sources");

            migrationBuilder.DropTable(
                name: "Source",
                schema: "Sources");

            migrationBuilder.DropTable(
                name: "Magazine",
                schema: "Sources");

            migrationBuilder.DropSequence(
                name: "ImagesSequenceHiLo",
                schema: "Sources");

            migrationBuilder.DropSequence(
                name: "SourcesSequenceHiLo",
                schema: "Sources");
        }
    }
}
