using System;
using System.IO;
using System.Threading.Tasks;
using Anotar.Serilog;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace wikibus.storage.azure
{
    public class BlobFileStorage : IFileStorage
    {
        private readonly CloudStorageAccount account;

        public BlobFileStorage(IAzureSettings settings)
        {
            LogTo.Debug("Connecting to Azure Storage");
            this.account = CloudStorageAccount.Parse(settings.ConnectionString);
        }

        public async Task<Uri> UploadFile(string name, string container, string contentType, Stream contents)
        {
            LogTo.Debug("Creating blob client");
            var client = account.CreateCloudBlobClient();

            LogTo.Debug("Getting blob container {0}", container);
            var blobRef = client.GetContainerReference(container);
            if (await blobRef.CreateIfNotExistsAsync()) {

                LogTo.Information("Creating blob container {0}", container);
                await blobRef.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob,
                });

            }

            var blob = blobRef.GetBlockBlobReference(name);
            blob.Properties.ContentType = contentType;

            LogTo.Information("Uploading file {)}", name);
            await blob.UploadFromStreamAsync(contents);

            return blob.Uri;
        }
    }
}
