using System.IO;
using System.Threading.Tasks;

namespace Wikibus.Sources.Images
{
    public interface ISourceImageService
    {
        Task AddImage(int id, string name, Stream image);

        Task DeleteImage(string externalId);
    }
}