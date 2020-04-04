using Argolis.Hydra.Annotations;
using Argolis.Hydra.Models;
using Argolis.Models;
using JsonLD.Entities;
using JsonLD.Entities.Context;
using Newtonsoft.Json.Linq;
using NullGuard;
using Vocab;
using Wikibus.Common;
using Wikibus.Common.JsonLd;

namespace Wikibus.Sources
{
    [SupportedClass(Api.WishlistItem)]
    [Identifier("wishlist/{sourceId}")]
    [CollectionIdentifier("wishlist")]
    [NullGuard(ValidationFlags.ReturnValues)]
    public class WishlistItem
    {
        [Link]
        [Writeable(false)]
        public Brochure Brochure { get; set; }

        [Writeable(false)]
        public bool Done { get; set; }

        private static JObject Context => new JObject(
            Prefix.Of(typeof(Api)),
            "brochure".IsProperty(Wbo.brochure).Type().Id(),
            "title".IsProperty(Dcterms.title),
            "done".IsProperty(Api.brochureUploaded));

        private static string Type => Api.WishlistItem;
    }
}
