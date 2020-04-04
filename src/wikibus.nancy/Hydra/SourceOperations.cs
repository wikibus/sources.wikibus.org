using Argolis.Hydra.Discovery.SupportedOperations;
using Argolis.Hydra.Nancy;
using JsonLD.Entities;
using Vocab;
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
                    .SupportsPost()
                    .Title("Upload image")
                    .TypedAs((IriRef)Schema.TransferAction)
                    .Expects((IriRef)Api.ImageUpload);

                this.Property(s => s.Image)
                    .SupportsDelete()
                    .TypedAs((IriRef)Schema.DeleteAction)
                    .Title("Remove image");

                this.Property(s => s.Content)
                    .SupportsPost()
                    .Title("Upload PDF")
                    .Expects((IriRef)Api.PdfUpload)
                    .TypedAs((IriRef)Schema.TransferAction);
            }
        }
    }
}
