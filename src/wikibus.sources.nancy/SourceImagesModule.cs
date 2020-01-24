using Nancy;

namespace Wikibus.Sources.Nancy
{
    /// <summary>
    /// Servers images of sources over HTTP
    /// </summary>
    public class SourceImagesModule : NancyModule
    {
        private readonly ISourceImagesRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceImagesModule"/> class.
        /// </summary>
        public SourceImagesModule(ISourceImagesRepository repository)
        {
            this.ReturnNotFoundWhenModelIsNullOr(model => model.Length == 0);

            this.repository = repository;

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
