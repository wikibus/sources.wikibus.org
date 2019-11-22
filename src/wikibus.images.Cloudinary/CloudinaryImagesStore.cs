using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Wikibus.Sources.Images;
using ImageUploadResult = Wikibus.Sources.Images.ImageUploadResult;

namespace wikibus.images.Cloudinary
{
    public class CloudinaryImagesStore : IImageStorage
    {
        private readonly CloudinaryDotNet.Cloudinary cloudinary;
        private readonly ICloudinarySettings settings;

        public CloudinaryImagesStore(CloudinaryDotNet.Cloudinary cloudinary, ICloudinarySettings settings)
        {
            this.cloudinary = cloudinary;
            this.settings = settings;
        }

        public async Task<ImageUploadResult> UploadImage(string name, Stream fileStream)
        {
            var result = await this.cloudinary.UploadAsync(new ImageUploadParams
            {
                File = new FileDescription(name, fileStream),
                Folder = this.settings.BrochuresFolder,
                EagerTransforms = new List<Transformation>
                {
                    new Transformation().Named(this.settings.ThumbnailTransformation),
                    new Transformation().Named(this.settings.DefaultTransformation),
                }
            });

            var image = await this.cloudinary.GetResourceAsync(result.PublicId);

            var thumbUrl = image.Derived
                .Where(derived => derived.Transformation == $"t_{this.settings.ThumbnailTransformation}")
                .Select(derived => derived.SecureUrl)
                .FirstOrDefault();

            var reasonablySizedUrl = image.Derived
                .Where(derived => derived.Transformation == $"t_{this.settings.DefaultTransformation}")
                .Select(derived => derived.SecureUrl)
                .FirstOrDefault();

            return new ImageUploadResult(
                reasonablySizedUrl ?? image.SecureUrl,
                thumbUrl ?? image.SecureUrl,
                image.PublicId);
        }

        public async Task<bool> DeleteImage(string externalId)
        {
            try
            {
                var result = await this.cloudinary.DeleteResourcesAsync(externalId);
                return result.Deleted.Count == 1;
            }
            catch
            {
                return false;
            }
        }
    }
}
