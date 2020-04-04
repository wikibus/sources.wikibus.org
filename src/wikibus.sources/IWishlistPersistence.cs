using System.Threading.Tasks;

namespace Wikibus.Sources
{
    public interface IWishlistPersistence
    {
        Task AddWishlistItem(string userId, int brochureId);

        Task MarkDone(int brochureId);
    }
}
