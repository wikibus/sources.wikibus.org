using System.Threading.Tasks;

namespace wikibus.storage
{
    public class NullStorageQueue : IStorageQueue
    {
        public Task AddMessage(string queueName, object message)
        {
            return Task.CompletedTask;
        }
    }
}
