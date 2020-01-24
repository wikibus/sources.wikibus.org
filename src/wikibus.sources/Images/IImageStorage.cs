using System.IO;
using System.Threading.Tasks;

namespace Wikibus.Sources.Images
{
    public interface IImageStorage
    {
        Task<ImageUploadResult> UploadImage(string name, Stream fileStream);

        Task<bool> DeleteImage(string externalId);
    }
}
