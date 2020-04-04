using System.Collections.Generic;
using System.Threading.Tasks;
using Anotar.Serilog;
using Argolis.Hydra;
using Argolis.Hydra.Resources;
using Argolis.Models;
using Argolis.Nancy;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using Wikibus.Common;
using Wikibus.Sources.Filters;

namespace Wikibus.Sources.Nancy
{
    public sealed class WishlistModule : ArgolisModule
    {
        private readonly IIriTemplateFactory templateFactory;
        private readonly IWishlistRepository wishlistRepository;
        private readonly IWishlistPersistence wishlistPersistence;

        public WishlistModule(
            IModelTemplateProvider provider,
            IIriTemplateFactory templateFactory,
            IWishlistRepository wishlistRepository,
            IWishlistPersistence wishlistPersistence)
            : base(provider)
        {
            this.templateFactory = templateFactory;
            this.wishlistRepository = wishlistRepository;
            this.wishlistPersistence = wishlistPersistence;

            using (this.Templates)
            {
                this.Put<WishlistItem>(this.PutWishlistItem);
                this.Get<Collection<WishlistItem>>(this.GetWishlist);
            }
        }

        private async Task<object> PutWishlistItem(dynamic route)
        {
            this.RequiresAuthentication();

            var userId = this.Context.CurrentUser.GetNameClaim();
            await this.wishlistPersistence.AddWishlistItem(userId, route.sourceId);

            return HttpStatusCode.NoContent;
        }

        private async Task<object> GetWishlist(dynamic o)
        {
            var searchTemplate = this.templateFactory.CreateIriTemplate<WishlistFilter, Collection<WishlistItem>>();
            var uriTemplate = new UriTemplate.Core.UriTemplate(searchTemplate.Template);

            var templateParams = new Dictionary<string, object>((DynamicDictionary)this.Context.Request.Query);

            var collectionId = uriTemplate.BindByName(templateParams);
            var canonical = uriTemplate.BindByName(templateParams).ToString();
            var filter = this.Bind<WishlistFilter>();

            WishlistItem[] wishlistItems = new WishlistItem[0];
            if (this.Context.CurrentUser.HasPermission(Permissions.WriteSources))
            {
                LogTo.Debug("Getting all unique wishlist items");
                wishlistItems = await this.wishlistRepository.FindAdminWishlist(filter.ShowAll.GetValueOrDefault(false));
            }
            else if (this.Context.CurrentUser.IsAuthenticated())
            {
                var user = this.Context.CurrentUser.GetNameClaim();
                LogTo.Debug("Getting items for user {0}", user);
                wishlistItems = await this.wishlistRepository.FindUsersWishlist(user, filter.ShowAll.GetValueOrDefault(true));
            }

            var collection = new Wishlist(searchTemplate)
            {
                Id = collectionId,
                Members = wishlistItems,
                TotalItems = wishlistItems.Length,
                CurrentMappings = templateParams.ToTemplateMappings(searchTemplate),
            };

            return this.Negotiate.WithModel(collection).WithHeader("Link", $"<{canonical}>; rel=\"canonical\"");
        }
    }
}
