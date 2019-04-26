using Microsoft.EntityFrameworkCore;

namespace Wikibus.Sources.EF
{
    public interface ISourceContext
    {
        DbSet<BookEntity> Books { get; }

        DbSet<BrochureEntity> Brochures { get; }

        DbSet<MagazineEntity> Magazines { get; }
    }
}