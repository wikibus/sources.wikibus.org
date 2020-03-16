using System.Threading.Tasks;
using Anotar.Serilog;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace wikibus.storage.azure
{
    public class StorageQueue : IStorageQueue
    {
        private readonly CloudStorageAccount account;

        public StorageQueue(IAzureSettings settings)
        {
            LogTo.Debug("Connecting to Azure Storage");
            this.account = CloudStorageAccount.Parse(settings.ConnectionString);
        }

        public async Task AddMessage(string queueName, object message)
        {
            var queueClient = this.account.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();

            await queue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(message)));
        }
    }
}
