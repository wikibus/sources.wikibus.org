using Argolis.Hydra.Discovery.SupportedOperations;
using Argolis.Hydra.Nancy;
using JsonLD.Entities;
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
                this.Class.SupportsPost("Create brochure", expects: (IriRef)Wbo.Brochure);
            }
        }
    }
}
