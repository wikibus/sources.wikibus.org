using System.IO;
using System.Threading.Tasks;

namespace Wikibus.Sources
{
    public interface IPdfService
    {
        Task UploadResourcePdf<T>(T resource, string name, Stream stream)
            where T : Source;

        Task NotifyPdfUploaded<T>(T resource, string name)
            where T : Source;
    }
}
