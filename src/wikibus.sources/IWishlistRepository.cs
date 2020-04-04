using System.Threading.Tasks;

namespace Wikibus.Sources
{
    public interface IWishlistRepository
    {
        Task<WishlistItem[]> FindUsersWishlist(string userId, bool showAll);

        Task<WishlistItem[]> FindAdminWishlist(bool showAll);
    }
}
