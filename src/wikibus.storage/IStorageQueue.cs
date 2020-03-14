using System.Threading.Tasks;

namespace wikibus.storage
{
    public interface IStorageQueue
    {
        Task AddMessage(string queueName, object message);
    }
}
