using Argolis.Hydra.Discovery.SupportedOperations;
using Argolis.Hydra.Nancy;
using JsonLD.Entities;
using Vocab;
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
            this.Class.SupportsGet()
                .Title("Gets the API entrypoint")
                .Description("The entrypoint is the the API starts");

            this.Property(e => e.Books).SupportsGet().Title("Gets the collection of books (paged)");
            this.Property(e => e.Brochures)
                .SupportsGet().Title("Gets the collection of brochures (paged)");

            if (context.Current?.CurrentUser?.HasClaim(claim => claim.Value == "write:sources") == true)
            {
                this.Property(e => e.Brochures)
                    .SupportsPost()
                    .Title("Add a new brochure")
                    .Expects((IriRef)Wbo.Brochure)
                    .Returns((IriRef)Wbo.Brochure)
                    .TypedAs((IriRef)Schema.CreateAction);
            }

            this.Property(e => e.Magazines).SupportsGet().Title("Gets the collection of magazines");
        }
    }
}
