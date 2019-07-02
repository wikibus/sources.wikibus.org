using System;
using Argolis.Hydra.Discovery.SupportedOperations;
using Argolis.Hydra.Nancy;
using JsonLD.Entities;
using Wikibus.Common;

namespace Wikibus.Nancy.Hydra
{
    /// <summary>
    /// Sets up operations supported by <see cref="EntryPoint"/> class
    /// </summary>
    public class EntrypointOperations : SupportedOperations<EntryPoint>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntrypointOperations"/> class.
        /// </summary>
        public EntrypointOperations(NancyContextWrapper context)
        {
            this.Class.SupportsGet("Gets the API entrypoint", "The entrypoint is the the API starts");

            this.Property(e => e.Books).SupportsGet("Gets the collection of books (paged)");
            var brochures = this.Property(e => e.Brochures)
                .SupportsGet("Gets the collection of brochures (paged)");

            if (context.Current?.CurrentUser?.HasClaim(claim => claim.Value == "write:sources") == true)
            {
                brochures.SupportsPost("Add a new brochure", expects: (IriRef)Wbo.Brochure);
            }

            this.Property(e => e.Magazines).SupportsGet("Gets the collection of magazines");
        }
    }
}
