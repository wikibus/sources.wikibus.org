using Argolis.Hydra.Discovery.SupportedOperations;
using JsonLD.Entities;
using Wikibus.Common;
using Wikibus.Sources;

namespace Wikibus.Nancy.Hydra
{
    /// <summary>
    /// Sets up operations supported by <see cref="Brochure"/> class
    /// </summary>
    public class BrochureOperations : SupportedOperations<Brochure>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrochureOperations"/> class.
        /// </summary>
        public BrochureOperations()
        {
            this.Class.SupportsGet(title: "Get brochure");
            this.Class.SupportsPut("Update brochure", expects: (IriRef)Wbo.Brochure);
        }
    }
}
