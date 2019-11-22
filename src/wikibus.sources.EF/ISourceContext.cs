using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Wikibus.Sources.EF
{
    public interface ISourceContext
    {
        DbSet<SourceEntity> Sources { get; }

        DbSet<ImageEntity> Images { get; }

        DbSet<BookEntity> Books { get; }

        DbSet<BrochureEntity> Brochures { get; }

        DbSet<MagazineEntity> Magazines { get; }

        Task<int> SaveChangesAsync();
    }
}
