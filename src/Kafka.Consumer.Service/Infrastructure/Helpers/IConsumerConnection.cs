using Confluent.Kafka;
using Kafka.Consumer.Service.Infrastructure.Settings;

namespace Kafka.Consumer.Service.Infrastructure.Helpers
{
    public interface IConsumerConnection
    {
        IConsumer<Ignore, string> GetListenerConsumer(TopicSettings config);
        void StartReadingStatus();
        bool GetReadingStatus();
    }
}
