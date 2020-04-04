using System.Linq;
using System.Threading.Tasks;
using Argolis.Models;
using JsonLD.Entities;
using Microsoft.EntityFrameworkCore;

namespace Wikibus.Sources.EF
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ISourceContext context;
        private readonly IUriTemplateExpander expander;

        public WishlistRepository(ISourceContext context, IUriTemplateExpander expander)
        {
            this.context = context;
            this.expander = expander;
        }

        public async Task<WishlistItem[]> FindUsersWishlist(string userId, bool showAll)
        {
            var entities = await (from wishlistItem in this.WishlistItems(showAll)
                where wishlistItem.User == userId
                join brochure in this.context.Brochures on wishlistItem.SourceId equals brochure.Id
                orderby wishlistItem.Id descending
                select new
                {
                    brochure,
                    done = wishlistItem.Done
                }).ToArrayAsync();

            return entities.Select(this.ToWishlistItem).ToArray();
        }

        public async Task<WishlistItem[]> FindAdminWishlist(bool showAll)
        {
            var entities = await (from wishlistItem in this.WishlistItems(showAll)
                join brochure in this.context.Brochures on wishlistItem.SourceId equals brochure.Id
                orderby wishlistItem.Id descending
                select new
                {
                    brochure,
                    done = wishlistItem.Done
                }).Distinct().ToArrayAsync();

            return entities.Select(this.ToWishlistItem).ToArray();
        }

        private IQueryable<WishlistItemEntity> WishlistItems(bool all)
        {
            if (all)
            {
                return this.context.WishlistItems;
            }

            return this.context.WishlistItems.Where(item => item.Done == false);
        }

        private WishlistItem ToWishlistItem(dynamic item)
        {
            return new WishlistItem
            {
                Brochure = new Brochure
                {
                    Id = this.expander.ExpandAbsolute<Brochure>(new { id = item.brochure.Id }),
                    Title = item.brochure.FolderName,
                },
                Done = item.done
            };
        }
    }
}
