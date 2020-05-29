using System;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Serilog;
using Argolis.Hydra.Resources;
using Argolis.Models;
using Microsoft.EntityFrameworkCore;
using NullGuard;
using Wikibus.Sources.Filters;

namespace Wikibus.Sources.EF
{
    public class SourcesRepository : ISourcesRepository
    {
        private readonly ISourceContext context;
        private readonly EntityFactory factory;
        private readonly IUriTemplateMatcher matcher;

        public SourcesRepository(
            ISourceContext context,
            EntityFactory factory,
            IUriTemplateMatcher matcher)
        {
            this.context = context;
            this.factory = factory;
            this.matcher = matcher;
        }

        [return: AllowNull]
        public async Task<Magazine> GetMagazine(Uri identifier)
        {
            var id = this.matcher.Match<Magazine>(identifier).Get<string>("name");

            if (id == null)
            {
                return null;
            }

            var source = await (from mag in this.context.Magazines
                          where mag.Name == id
                          select mag).SingleOrDefaultAsync();

            if (source == null)
            {
                return null;
            }

            return this.factory.CreateMagazine(source);
        }

        [return: AllowNull]
        public async Task<Brochure> GetBrochure(Uri identifier)
        {
            LogTo.Debug("Getting brochure {0}", identifier);
            var uriTemplateMatches = this.matcher.Match<Brochure>(identifier);

            if (!uriTemplateMatches.ContainsKey("id"))
            {
                LogTo.Debug("Id could not have been extracted from URI {0}", identifier);
                return null;
            }

            var id = uriTemplateMatches.Get<int>("id");
            var source = await (from b in this.context.Brochures.Include(b => b.Images)
                          where b.Id == id
                          select new EntityWrapper<BrochureEntity>
                          {
                              Entity = b,
                              HasLegacyImage = b.Image.Bytes != null
                          }).SingleOrDefaultAsync();

            if (source == null)
            {
                LogTo.Debug("Brochure {0} brochure not found", id);
                return null;
            }

            return this.factory.CreateBrochure(source);
        }

        [return: AllowNull]
        public async Task<Book> GetBook(Uri identifier)
        {
            var uriTemplateMatches = this.matcher.Match<Book>(identifier);
            if (!uriTemplateMatches.ContainsKey("id"))
            {
                return null;
            }

            var id = uriTemplateMatches.Get<int>("id");
            var source = await (from b in this.context.Books.Include(b => b.Images)
                          where b.Id == id
                          select new EntityWrapper<BookEntity>
                          {
                              Entity = b,
                              HasLegacyImage = b.Image.Bytes != null
                          }).SingleOrDefaultAsync();

            if (source == null)
            {
                return null;
            }

            return this.factory.CreateBook(source);
        }

        public async Task<SearchableCollection<Book>> GetBooks(Uri identifier, BookFilters filters, int page, int pageSize = 10)
        {
            this.factory.OnlyCoverImage = true;
            var books = this.context.Books.Include(b => b.Images);
            return await books.GetCollectionPage<Book, BookEntity, SearchableCollection<Book>>(
                identifier,
                entities => entities.OrderByDescending(b => b.BookTitle),
                this.FilterBooks(filters),
                page,
                pageSize,
                this.factory.CreateBook);
        }

        public async Task<SearchableCollection<Brochure>> GetBrochures(Uri identifier, BrochureFilters filters, int page, int pageSize = 10)
        {
            this.factory.OnlyCoverImage = true;
            var brochures = this.context.Brochures.Include(b => b.Images);
            return await brochures.GetCollectionPage<Brochure, BrochureEntity, BrochureCollection>(
                identifier,
                collection => collection
                    .OrderByDescending(b => b.Updated)
                    .ThenByDescending(b => b.Id),
                this.FilterBrochures(filters),
                page,
                pageSize,
                this.factory.CreateBrochure);
        }

        public async Task<SearchableCollection<Magazine>> GetMagazines(Uri identifier, MagazineFilters filters, int page, int pageSize = 10)
        {
            this.factory.OnlyCoverImage = true;
            return await this.context.Magazines.GetCollectionPage<Magazine, MagazineEntity, SearchableCollection<Magazine>>(
                identifier,
                entity => entity.Name,
                this.FilterMagazines(filters),
                page,
                pageSize,
                this.factory.CreateMagazine);
        }

        public async Task<Collection<Issue>> GetMagazineIssues(Uri uri)
        {
            this.factory.OnlyCoverImage = true;
            var name = this.matcher.Match<Collection<Issue>>(uri).Get<string>("name");

            if (name == null)
            {
                return new Collection<Issue>();
            }

            var results = await (from m in this.context.Magazines.Include("Issues/Images")
                           where m.Name == name
                           from issue in m.Issues
                           select new
                           {
                               Entity = issue,
                               issue.Magazine,
                               HasImage = issue.Image.Bytes != null
                           }).ToListAsync();

            var issues = results.Select(i => new EntityWrapper<MagazineIssueEntity>
            {
                Entity = i.Entity,
                HasLegacyImage = i.HasImage
            }).ToList();

            return new Collection<Issue>
            {
                Id = uri,
                Members = issues.Select(this.factory.CreateMagazineIssue).ToArray(),
                TotalItems = issues.Count
            };
        }

        [return: AllowNull]
        public async Task<Issue> GetIssue(Uri identifier)
        {
            var matches = this.matcher.Match<Issue>(identifier);

            if (matches.Success == false)
            {
                return null;
            }

            var magazineName = matches.Get<string>("name");
            var issueNumber = matches.Get<int>("number");

            var result = await (from m in this.context.Magazines.Include("Issues/Images")
                          where m.Name == magazineName
                          from i in m.Issues
                          where i.MagIssueNumber == issueNumber
                          select new
                          {
                              Entity = i,
                              i.Magazine,
                              HasImage = i.Image.Bytes != null
                          }).SingleOrDefaultAsync();

            if (result == null)
            {
                return null;
            }

            var source = new EntityWrapper<MagazineIssueEntity>
            {
                Entity = result.Entity,
                HasLegacyImage = result.HasImage
            };

            return this.factory.CreateMagazineIssue(source);
        }

        private Func<IQueryable<BookEntity>, IQueryable<BookEntity>> FilterBooks(BookFilters filters)
        {
            return books =>
            {
                if (string.IsNullOrWhiteSpace(filters.Title) == false)
                {
                    books = books.Where(e => e.BookTitle.Contains(filters.Title.Trim()));
                }

                if (string.IsNullOrWhiteSpace(filters.Author) == false)
                {
                    books = books.Where(e => e.BookAuthor.Contains(filters.Author.Trim()));
                }

                if (string.IsNullOrWhiteSpace(filters.Language) == false)
                {
                    books = books.Where(b => b.Languages.Contains(filters.Language));
                }

                return books;
            };
        }

        private Func<IQueryable<BrochureEntity>, IQueryable<BrochureEntity>> FilterBrochures(BrochureFilters filters)
        {
            return brochures =>
            {
                if (string.IsNullOrWhiteSpace(filters.Title) == false)
                {
                    brochures = brochures.Where(e => e.FolderName.Contains(filters.Title.Trim()));
                }

                if (string.IsNullOrWhiteSpace(filters.Language) == false)
                {
                    brochures = brochures.Where(b => b.Languages.Contains(filters.Language));
                }

                if (filters.WithPdfOnly.Equals(true))
                {
                    brochures = brochures.Where(b => !string.IsNullOrWhiteSpace(b.ContentUrl));
                }

                if (filters.WithoutImages.Equals(true))
                {
                    brochures = brochures.Where(b => b.Image.Bytes == null && !b.Images.Any());
                }

                if (filters.Contributor != null)
                {
                    var auth0Id = Uri.UnescapeDataString(filters.Contributor.Segments.Last());

                    LogTo.Debug("Filtering brochures of user {0}", auth0Id);
                    brochures = brochures.Where(b => b.User == auth0Id);
                }

                return brochures;
            };
        }

        private Func<IQueryable<MagazineEntity>, IQueryable<MagazineEntity>> FilterMagazines(MagazineFilters filters)
        {
            return entities =>
            {
                if (string.IsNullOrWhiteSpace(filters.Title) == false)
                {
                    entities = entities.Where(m => m.Name.Contains(filters.Title.Trim()));
                }

                return entities;
            };
        }
    }
}
