using Microsoft.EntityFrameworkCore;

namespace Wikibus.Sources.EF
{
    public class SourceContext : DbContext, ISourceContext
    {
        private readonly ISourcesDatabaseSettings configuration;

        public SourceContext(ISourcesDatabaseSettings configuration)
        {
            this.configuration = configuration;
        }

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
                .HasOne(e => e.Image).WithOne().HasForeignKey<ImageData>(e => e.Id);

            modelBuilder.Entity<ImageData>()
                .Property(img => img.Bytes).HasColumnName("Image").IsRequired();

            modelBuilder.Entity<ImageData>()
                .ToTable("Source", "Sources");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                this.configuration.ConnectionString,
                builder => builder.UseRowNumberForPaging());
        }
    }
}
