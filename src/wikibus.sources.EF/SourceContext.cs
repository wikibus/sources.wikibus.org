using Microsoft.EntityFrameworkCore;

namespace Wikibus.Sources.EF
{
    public class SourceContext : DbContext, ISourceContext
    {
        public DbSet<BookEntity> Books { get; set; }

        public DbSet<BrochureEntity> Brochures { get; set; }

        public DbSet<MagazineEntity> Magazines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SourceEntity>()
                .HasDiscriminator<string>("SourceType")
                .HasValue<BookEntity>("book")
                .HasValue<BrochureEntity>("folder")
                .HasValue<MagazineIssueEntity>("magissue");

            modelBuilder.Entity<MagazineEntity>()
                .HasMany(t => t.Issues).WithOne(issue => issue.Magazine).IsRequired()
                .HasForeignKey(issue => issue.MagIssueMagazine);

            modelBuilder.Entity<SourceEntity>()
                .Property(e => e.Image).IsRequired();

            modelBuilder.Entity<ImageData>().ToTable("Source", "Sources");
        }
    }
}