namespace wikibus.images.Cloudinary
{
    public interface ICloudinarySettings
    {
        string BrochuresFolder { get; }

        string ThumbnailTransformation { get; }

        string DefaultTransformation { get; }
    }
}
