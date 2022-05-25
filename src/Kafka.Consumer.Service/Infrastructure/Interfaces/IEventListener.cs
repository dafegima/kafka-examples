using Kafka.Consumer.Service.Infrastructure.Models;

namespace Kafka.Consumer.Service.Infrastructure.Interfaces
{
    public interface IEventListener
    {
        TopicResponse RetrieveNextBatch();
        void Commit();
    }
}
