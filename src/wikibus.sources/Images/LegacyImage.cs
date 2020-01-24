namespace Wikibus.Sources.Images
{
    public class LegacyImage : Image
    {
        /// <summary>
        /// Gets the thumbnail image.
        /// </summary>
        public override Image Thumbnail =>
            new LegacyImage
            {
                ContentUrl = this.ContentUrl + "/small",
                IsThumbnail = true,
            };
    }
}
