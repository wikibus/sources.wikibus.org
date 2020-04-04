using Argolis.Hydra.Core;
using Wikibus.Common;

namespace Wikibus.Sources
{
    public class Wishlist : SearchableCollection<WishlistItem>
    {
        public Wishlist(IriTemplate template)
        {
            this.Search = template;
            this.Title = "Brochure scan wishlist";
        }

        public string[] Types =>
            new[]
            {
                base.Type,
                Api.WishlistCollection
            };
    }
}
