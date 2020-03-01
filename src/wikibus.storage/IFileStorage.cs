using System;
using System.IO;
using System.Threading.Tasks;

namespace wikibus.storage
{
    public interface IFileStorage
    {
        Task<Uri> UploadFile(string name, string folder, string contentType, Stream contents);
    }
}
