﻿using Argolis.Hydra.Discovery.SupportedOperations;
using Argolis.Hydra.Nancy;
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
        public BrochureOperations(NancyContextWrapper context)
        {
            this.Class.SupportsGet(title: "Get brochure");
            if (context.HasPermission(Permissions.WriteSources))
            {
                this.Class.SupportsPut("Update brochure", expects: (IriRef)Wbo.Brochure);
                this.Property(b => b.Location).SupportsPut("Update location");
            }
        }
    }
}
