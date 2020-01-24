using Argolis.Hydra.Discovery.SupportedOperations;
using Argolis.Hydra.Nancy;
using JsonLD.Entities;
using Vocab;
using Wikibus.Common;
using Wikibus.Sources;

namespace Wikibus.Nancy.Hydra
{
    public class BrochureCollectionOperations : SupportedOperations<BrochureCollection>
    {
        public BrochureCollectionOperations(NancyContextWrapper context)
        {
            if (context.Current?.CurrentUser.HasPermission(Permissions.WriteSources) == true)
            {
                this.Class.SupportsPost()
                    .Title("Create brochure")
                    .Expects((IriRef)Wbo.Brochure)
                    .Returns((IriRef)Wbo.Brochure)
                    .TypedAs((IriRef)Schema.CreateAction);
            }
        }
    }
}
