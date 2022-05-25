using Confluent.Kafka;
using Kafka.Consumer.Service.Infrastructure.Helpers.Abstractions;
using Kafka.Consumer.Service.Infrastructure.Settings;

namespace Kafka.Consumer.Service.Infrastructure.Helpers
{
    public interface IConsumerConnection : IAgnosticConnection<IConsumer<Ignore, string>>
    {
        IConsumer<Ignore, string> GetListenerConsumer(TopicSettings config);
        void StartReadingStatus();
        bool GetReadingStatus();
    }
}
