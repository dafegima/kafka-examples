using Kafka.Consumer.Service.Infrastructure.Helpers;
using Kafka.Consumer.Service.Infrastructure.Helpers.Base;
using Kafka.Consumer.Service.Infrastructure.Interfaces;
using Kafka.Consumer.Service.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Kafka.Consumer.Service.Infrastructure.Repositories
{
    public class TopicRepository : EventListenerBase, IEventListener
    {
        public TopicRepository(IConsumerConnection connection,
           IOptions<TopicSettings> topicSettings,
           ILogger<TopicRepository> logger)
            : base(connection,
                  topicSettings.Value,
                  logger)
        { }
    }
}
