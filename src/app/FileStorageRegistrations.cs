using Anotar.Serilog;
using Microsoft.Extensions.Configuration;
using Nancy;
using Nancy.Bootstrapper;
using wikibus.sources.pdf;
using wikibus.storage;
using wikibus.storage.azure;

namespace Brochures.Wikibus.Org
{
    public class FileStorageRegistrations : Registrations
    {
        public FileStorageRegistrations(ITypeCatalog typeCatalog, IConfiguration configuration)
            : base(typeCatalog)
        {
            this.Register<IPdfService>(typeof(NRecoPdfService));

            if (configuration["azure:storage:connectionString"] == null)
            {
                LogTo.Information("Azure Storage no set up. Using local filesystem");
                this.Register<IFileStorage>(new LocalFileStorage());
                return;
            }

            this.Register<IFileStorage>(typeof(BlobFileStorage));
        }
    }
}
