using Argolis.Hydra.Annotations;
using Argolis.Hydra.Resources;
using Argolis.Models;
using NullGuard;
using Wikibus.Common;

namespace Wikibus.Sources.Filters
{
    public class WishlistFilter : ITemplateParameters<Collection<WishlistItem>>
    {
        [Variable("showAll")]
        [Property(Api.showAll)]
        public bool? ShowAll { [return: AllowNull] get; set; }
    }
}
