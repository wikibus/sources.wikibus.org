using System.IO;
using System.Threading.Tasks;
using Imageflow.Fluent;
using Wikibus.Common;

namespace Wikibus.Nancy
{
    /// <summary>
    /// Uses ImageResizer to resize images
    /// </summary>
    internal class ImageResizer : IImageResizer
    {
        /// <summary>
        /// Resizes the specified image bytes.
        /// </summary>
        public async Task<byte[]> Resize(byte[] imageBytes, int maxSize)
        {
            if (imageBytes.Length == 0)
            {
                return imageBytes;
            }

            using (var stream = new MemoryStream())
            {
                using (var b = new FluentBuildJob())
                {
                    await b.Decode(imageBytes)
                        .ConstrainWithin((uint)maxSize, (uint)maxSize)
                        .Encode(new StreamDestination(stream, true), new LibJpegTurboEncoder())
                        .FinishAsync();
                }

                return stream.GetBuffer();
            }
        }
    }
}
