using Kafka.Producer.API.DTO;

namespace Kafka.Producer.API.Infrastructure.Interfaces
{
    public interface ITopicRepository
    {
        void AddMessage(MessageRequest message);
    }
}
