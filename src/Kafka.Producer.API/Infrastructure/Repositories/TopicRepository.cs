using Confluent.Kafka;
using Kafka.Producer.API.DTO;
using Kafka.Producer.API.Infrastructure.Interfaces;
using Kafka.Producer.API.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Kafka.Producer.API.Infrastructure.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly ILogger<TopicRepository> _logger;
        private readonly TopicSettings _topicSettings;
        private readonly IProducer<Null, string> _producer;
        public TopicRepository(IOptions<TopicSettings> settings, ILogger<TopicRepository> logger)
        {
            _topicSettings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger;

            _producer = new ProducerBuilder<Null, string>(ConstructConfig()).SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}")).Build();
        }

        private ProducerConfig ConstructConfig() =>
            new ProducerConfig
            {
                BootstrapServers = _topicSettings.BrokerList
            };

        public void AddMessage(MessageRequest message)
        {
            string data = JsonConvert.SerializeObject(message);//, microsoftDateFormatSettings);
            var t = _producer.ProduceAsync(_topicSettings.TopicName, new Message<Null, string> { Value = data });

            t.ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    _logger.LogError("Error writing message: " + message.LicencePlate);
                }
                else
                {
                    _logger.LogInformation("Message sent successfully: " + message.LicencePlate);
                }
            });
        }
    }
}
