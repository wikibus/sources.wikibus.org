using System.Linq;
using System.Threading.Tasks;
using Anotar.Serilog;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Wikibus.Sources.EF
{
    public class WishlistPersistence : IWishlistPersistence
    {
        private readonly SourceContext context;

        public WishlistPersistence(SourceContext context)
        {
            this.context = context;
        }

        public async Task AddWishlistItem(string userId, int brochureId)
        {
            LogTo.Information("User {0} adds source {1} to wishlist", userId, brochureId);

            var existingItems = from wi in this.context.WishlistItems
                where (wi.SourceId == brochureId && wi.User == userId) || wi.Brochure.ContentSize != null
                select wi;

            if (await existingItems.AnyAsync())
            {
                LogTo.Information("Wishlist item already exists or brochure has been already uploaded");
                return;
            }

            this.context.WishlistItems.Add(new WishlistItemEntity
            {
                SourceId = brochureId,
                User = userId
            });

            await this.context.SaveChangesAsync();
        }

        public async Task MarkDone(int brochureId)
        {
            LogTo.Debug("Marking wishlist items for brochure {0} as done", brochureId);

            await this.context.WishlistItems
                .Where(item => item.SourceId == brochureId)
                .ForEachAsync(item => { item.Done = true; });

            await this.context.SaveChangesAsync();
        }
    }
}
