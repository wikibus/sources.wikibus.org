using System.Threading.Tasks;
using Nancy;
using Wikibus.Common;

namespace Wikibus.Sources.Nancy
{
    /// <summary>
    /// Servers images of sources over HTTP
    /// </summary>
    public class SourceImagesModule : NancyModule
    {
        private const int SmallImageSize = 200;
        private readonly ISourceImagesRepository repository;
        private readonly IImageResizer resizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceImagesModule"/> class.
        /// </summary>
        public SourceImagesModule(ISourceImagesRepository repository, IImageResizer resizer)
        {
            this.ReturnNotFoundWhenModelIsNullOr(model => model.Length == 0);

            this.repository = repository;
            this.resizer = resizer;

            this.Get("/book/{id}/image", request => this.GetImage((int)request.id));
            this.Get("/brochure/{id}/image", request => this.GetImage((int)request.id));
            this.Get("/magazine/{mag}/issue/{issue}/image", request => this.GetImage((string)request.mag, (string)request.issue));
            this.Get("/book/{id}/image/small", request => this.GetImage((int)request.id));
            this.Get("/brochure/{id}/image/small", request => this.GetImage((int)request.id));
            this.Get("/magazine/{mag}/issue/{issue}/image/small", request => this.GetImage((string)request.mag, (string)request.issue));
        }

        private byte[] GetImage(string magazineName, string issueNumber)
        {
            return this.repository.GetImageBytes(magazineName, issueNumber);
        }

        private byte[] GetImage(int sourceId)
        {
            return this.repository.GetImageBytes(sourceId);
        }
    }
}
