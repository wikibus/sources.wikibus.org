using Argolis.Hydra.Discovery.SupportedOperations;
using Argolis.Hydra.Nancy;
using Wikibus.Common;
using Wikibus.Sources;

namespace Wikibus.Nancy.Hydra
{
    /// <summary>
    /// Sets up operations supported by <see cref="Source"/> class
    /// </summary>
    public class SourceOperations : SupportedOperations<Source>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceOperations"/> class.
        /// </summary>
        public SourceOperations(NancyContextWrapper context)
        {
            if (context.HasPermission(Permissions.WriteSources))
            {
                this.Property(s => s.Images)
                .SupportsPost("Upload image");
            }
        }
    }
}
