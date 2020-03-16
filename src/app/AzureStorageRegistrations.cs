using Anotar.Serilog;
using Microsoft.Extensions.Configuration;
using Nancy;
using Nancy.Bootstrapper;
using wikibus.storage;
using wikibus.storage.azure;

namespace Brochures.Wikibus.Org
{
    public class AzureStorageRegistrations : Registrations
    {
        public AzureStorageRegistrations(ITypeCatalog typeCatalog, IConfiguration configuration)
            : base(typeCatalog)
        {
            if (configuration["azure:storage:connectionString"] == null)
            {
                LogTo.Information("Azure Storage no set up. Using local filesystem");
                this.Register<IFileStorage>(new LocalFileStorage());
                this.Register<IStorageQueue>(new NullStorageQueue());
            }
            else
            {
                this.Register<IFileStorage>(typeof(BlobFileStorage));
                this.Register<IStorageQueue>(typeof(StorageQueue));
            }
        }
    }
}
