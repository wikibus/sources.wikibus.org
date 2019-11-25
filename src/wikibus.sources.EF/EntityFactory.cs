using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Argolis.Models;
using Wikibus.Common;
using Wikibus.Sources.Images;

namespace Wikibus.Sources.EF
{
    public class EntityFactory
    {
        private readonly IWikibusConfiguration configuration;
        private readonly ClaimsPrincipal principal;
        private readonly ISourceContext context;
        private readonly IUriTemplateExpander expander;

        public EntityFactory(
            IUriTemplateExpander expander,
            IWikibusConfiguration configuration,
            ClaimsPrincipal principal,
            ISourceContext context)
        {
            this.expander = expander;
            this.configuration = configuration;
            this.principal = principal;
            this.context = context;
        }

        public bool OnlyCoverImage { get; set; }

        public Book CreateBook(EntityWrapper<BookEntity> bookEntity)
        {
            var book = new Book
            {
                Id = this.expander.ExpandAbsolute<Book>(new { id = bookEntity.Entity.Id })
            };

            if (bookEntity.Entity.BookISBN != null)
            {
                book.ISBN = bookEntity.Entity.BookISBN.Trim();
            }

            if (bookEntity.Entity.Pages != null)
            {
                book.Pages = bookEntity.Entity.Pages;
            }

            if (bookEntity.Entity.Year != null)
            {
                book.Year = bookEntity.Entity.Year;
            }

            if (bookEntity.Entity.Month != null)
            {
                book.Month = bookEntity.Entity.Month;
            }

            if (bookEntity.Entity.BookTitle != null)
            {
                book.Title = bookEntity.Entity.BookTitle;
            }

            if (bookEntity.Entity.BookAuthor != null)
            {
                book.Author = new Author
                {
                    Name = bookEntity.Entity.BookAuthor
                };
            }

            this.CreateImageLinks(book, bookEntity);
            MapLanguages(book, bookEntity.Entity);
            MapDate(book, bookEntity.Entity);

            return book;
        }

        public Brochure CreateBrochure(EntityWrapper<BrochureEntity> source)
        {
            var target = new Brochure
            {
                Id = this.expander.ExpandAbsolute<Brochure>(new { id = source.Entity.Id })
            };
            if (source.Entity.Notes != null)
            {
                target.Description = source.Entity.Notes;
            }

            if (source.Entity.FolderCode != null)
            {
                target.Code = source.Entity.FolderCode;
            }

            if (source.Entity.FolderName != null)
            {
                target.Title = source.Entity.FolderName;
            }

            this.CreateImageLinks(target, source);

            MapSource(target, source.Entity);
            this.MapStorageLocation(target, source.Entity);

            return target;
        }

        public Issue CreateMagazineIssue(EntityWrapper<MagazineIssueEntity> issue)
        {
            var magazineIssue = new Issue
            {
                Id = this.expander.ExpandAbsolute<Issue>(new { number = issue.Entity.MagIssueNumber.Value, name = issue.Entity.Magazine.Name }),
                Magazine = new Magazine
                {
                    Id = this.expander.ExpandAbsolute<Magazine>(new { name = issue.Entity.Magazine.Name })
                }
            };
            if (issue.Entity.MagIssueNumber != null)
            {
                magazineIssue.Number = issue.Entity.MagIssueNumber.ToString();
            }

            this.CreateImageLinks(magazineIssue, issue);
            MapSource(magazineIssue, issue.Entity);

            return magazineIssue;
        }

        public Magazine CreateMagazine(MagazineEntity entity)
        {
            var magazine = new Magazine
            {
                Id = this.expander.ExpandAbsolute<Magazine>(new { name = entity.Name })
            };
            magazine.Title = entity.Name;
            return magazine;
        }

        private static void MapSource(Source target, SourceEntity source)
        {
            target.Month = source.Month;
            target.Year = source.Year;
            target.Pages = source.Pages;

            MapLanguages(target, source);
            MapDate(target, source);
        }

        private static void MapLanguages(Source target, SourceEntity source)
        {
            var languages = new List<string>();
            if (source.Language != null)
            {
                languages.Add(source.Language);
            }

            if (source.Language2 != null)
            {
                languages.Add(source.Language2);
            }

            target.Languages = languages.Select(l => new Language(l)).ToArray();
        }

        private static void MapDate(Source target, SourceEntity source)
        {
            if (source.Year.HasValue && source.Month.HasValue && source.Day.HasValue)
            {
                try
                {
                    target.Date = new DateTime(source.Year.Value, source.Month.Value, source.Day.Value);
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }
        }

        private void CreateImageLinks<TEntity>(Source source, EntityWrapper<TEntity> entity)
            where TEntity : SourceEntity
        {
            var images = entity.Entity.Images.Select((image, index) => new Image
            {
                Id = $"{this.configuration.BaseResourceNamespace}image/{image.ExternalId}",
                ContentUrl = image.OriginalUrl,
                OrderIndex = index,
                Thumbnail = new Image
                {
                    ContentUrl = image.ThumbnailUrl
                }
            });

            if (!this.OnlyCoverImage)
            {
                source.Images.Members = images.ToArray();
                source.Images.TotalItems = source.Images.Members.Length;
            }
            else
            {
                source.Images.Members = new Image[0];
                source.Images.TotalItems = images.Count();
            }

            if (entity.HasLegacyImage)
            {
                var legacyImage = new LegacyImage
                {
                    ContentUrl = source.Id.ToString().Replace(
                                     this.configuration.BaseResourceNamespace,
                                     this.configuration.BaseApiNamespace) + "/image"
                };
                source.Images.Members = new[] { legacyImage }.Concat(source.Images.Members).ToArray();
                source.Images.TotalItems += 1;
            }

            source.CoverImage = source.Images.Members.FirstOrDefault();
        }

        private void MapStorageLocation(Brochure target, BrochureEntity sourceEntity)
        {
            if (this.principal.HasPermission(Permissions.WriteSources))
            {
                var cabinetName = (from cabinet in this.context.FileCabinets
                    where cabinet.Id == sourceEntity.FileCabinet
                    select $"{cabinet.Description} ({sourceEntity.FileOffset})")
                    .SingleOrDefault();

                target.Location = new StorageLocation
                {
                    Id = this.expander.ExpandAbsolute<StorageLocation>(new
                    {
                        brochureId = sourceEntity.Id
                    }),
                    FilingCabinetId = sourceEntity.FileCabinet,
                    Name = cabinetName ?? "Not filed yet",
                    Position = sourceEntity.FileOffset
                };
            }
        }
    }
}
