using Confluent.Kafka;
using Kafka.Consumer.Service.Infrastructure.Helpers.Abstractions;
using Kafka.Consumer.Service.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Kafka.Consumer.Service.Infrastructure.Helpers
{
    public class ConsumerConnection : AgnosticConnection<IConsumer<Ignore, string>, ConsumerConfig>, IConsumerConnection
    {
        private readonly ILogger<ConsumerConnection> _logger;
        private TopicSettings _topicSettings;
        public bool RowsReadedSuccessfully { get { return _rowsReadedSuccessfully; } }
        private bool _rowsReadedSuccessfully { get; set; }

        public ConsumerConnection(IOptions<TopicSettings> settings,
            ILogger<ConsumerConnection> logger)
            : base(connectionSettings: new ConsumerConfig
            {
                BootstrapServers = settings.Value.BrokerList,
                GroupId = settings.Value.ConsumerGroup,
                StatisticsIntervalMs = settings.Value.StatisticsIntervalMs,
                SessionTimeoutMs = settings.Value.SessionTimeoutMs,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = settings.Value.AutoCommit,
                EnablePartitionEof = true,
                MaxPollIntervalMs = settings.Value.MaxPollIntervalMs,
                AutoCommitIntervalMs = 5000,
                EnableAutoOffsetStore = true,
            })
        {
            _logger = logger;
            _topicSettings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        }


        public IConsumer<Ignore, string> GetListenerConsumer(TopicSettings config)
        {
            _topicSettings = config;

            var instance = NewInstance;
            instance.Subscribe(config.TopicName);

            return instance;
        }

        public void StartReadingStatus() { _rowsReadedSuccessfully = true; }
        public bool GetReadingStatus() { return _rowsReadedSuccessfully; }

        protected override IConsumer<Ignore, string> Resolve(ConsumerConfig connectionSettings)
        {
            return new ConsumerBuilder<Ignore, string>(connectionSettings)
                 .SetErrorHandler((_, error) =>
                 {
                     _rowsReadedSuccessfully = false;
                     _logger.LogError("Error on kafka consumer: code [{code}] - Error Information: {@reason}", error.Reason, error.Code);
                 })
                 .SetPartitionsAssignedHandler((_, partitions) =>
                 {
                     _logger.LogInformation("Assigned partitions: {@partitions}", partitions.Select(p => p.Partition.Value));
                 })
                 .SetPartitionsRevokedHandler((_, partitions) =>
                 {
                     _logger.LogInformation("Revoked partitions: {@partitions}", partitions.Select(p => p.Partition.Value));
                 })
                 .Build();
        }
    }
}