using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace wikibus.storage.azure
{
    public class BlobFileStorage : IFileStorage
    {
        private readonly IAzureSettings settings;

        public BlobFileStorage(IAzureSettings settings)
        {
            this.settings = settings;
        }

        public async Task<Uri> UploadFile(string name, string container, string contentType, Stream contents)
        {
            var account = CloudStorageAccount.Parse(this.settings.ConnectionString);
            var client = account.CreateCloudBlobClient();

            var blobRef = client.GetContainerReference(container);
            if (await blobRef.CreateIfNotExistsAsync()) {

                await blobRef.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob,
                });

            }

            var blob = blobRef.GetBlockBlobReference(name);
            blob.Properties.ContentType = contentType;

            await blob.UploadFromStreamAsync(contents);

            return blob.Uri;
        }
    }
}
