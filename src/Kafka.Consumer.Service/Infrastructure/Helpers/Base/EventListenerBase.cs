using Confluent.Kafka;
using Kafka.Consumer.Service.Infrastructure.Interfaces;
using Kafka.Consumer.Service.Infrastructure.Models;
using Kafka.Consumer.Service.Infrastructure.Settings;
using Newtonsoft.Json;

namespace Kafka.Consumer.Service.Infrastructure.Helpers.Base
{
    public class EventListenerBase : IEventListener
    {
        #region Members

        private readonly TopicSettings _topicSettings;
        private readonly ILogger<EventListenerBase> _logger;
        
        private readonly IConsumer<Ignore, string> _consumer;

        private int _currentReadCount = 0;
        private List<TopicPartitionOffset> _topicPartitionOffsets;

        #endregion Members
        private long offsetInicial = 0;
        private long offsetFinal = 0;
        private readonly IConsumerConnection _connection;
        protected EventListenerBase(IConsumerConnection connection,
            TopicSettings topicSettings,
            ILogger<EventListenerBase> logger)
        {
            _topicSettings = topicSettings;
            _logger = logger;
            _connection = connection;
            _consumer = connection.GetListenerConsumer(topicSettings);
        }

        public TopicResponse RetrieveNextBatch()
        {
            TopicResponse topicResponse = new TopicResponse();

            _connection.StartReadingStatus();
            _topicPartitionOffsets = new List<TopicPartitionOffset>();
            topicResponse.Messages = RetrieveBatchMessages()
                .AsParallel()
                .Select(message => (JsonConvert.DeserializeObject<TopicEvent>(message)))
                .ToList();

            topicResponse.ConsumerSuccessfully = _connection.GetReadingStatus();
            return topicResponse;
        }
        private IEnumerable<string> RetrieveBatchMessages()
        {
            DateTime executionDate = DateTime.Now;
            _currentReadCount = 0;

            do
            {
                offsetInicial = 0;
                if (TryRetrieve(out string message))
                {
                    _currentReadCount++;
                    yield return message;
                }
            } while (CanReadNext(executionDate));
            _logger.LogInformation($"Total messages read: {_currentReadCount}");
            _logger.LogInformation("-------------------------------------");
            _logger.LogInformation($"initial offset: {offsetInicial}, final offset: {offsetFinal}");
            _logger.LogInformation("-------------------------------------");
        }

        private bool TryRetrieve(out string message)
        {
            message = null;
            try
            {
                var consumeResult = _consumer.Consume(millisecondsTimeout: _topicSettings.ConsumeTimeoutMs);
                if (!string.IsNullOrEmpty(consumeResult?.Message?.Value))
                {
                    _topicPartitionOffsets.Add(consumeResult.TopicPartitionOffset);
                    if (offsetInicial == 0)
                        offsetInicial = consumeResult.Offset.Value;
                    offsetFinal = consumeResult.Offset.Value;

                    message = consumeResult?.Message?.Value;
                    return true;
                }
            }
            catch (ConsumeException e)
            {
                _logger.LogWarning(e, "Error al intentar obtener el evento de kafka.");
            }

            return false;
        }

        private bool CanReadNext(DateTime executionDate) =>
            executionDate.AddMilliseconds(_topicSettings.RecopilationTimeMs) > DateTime.Now
            &&
            _currentReadCount < _topicSettings.MaxItemsDequeue;

        public void Commit()
        {
            try
            {
                _consumer.Commit(_topicPartitionOffsets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}