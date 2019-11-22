namespace Wikibus.Sources.Images
{
    public class ImageUploadResult
    {
        public ImageUploadResult(string original, string thumbnail, string externalId)
        {
            this.Original = original;
            this.Thumbnail = thumbnail;
            this.ExternalId = externalId;
        }

        public string Original { get; private set; }

        public string Thumbnail { get; private set; }

        public string ExternalId { get; private set; }
    }
}
