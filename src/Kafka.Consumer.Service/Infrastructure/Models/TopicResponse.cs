namespace Kafka.Consumer.Service.Infrastructure.Models
{
    public class TopicResponse
    {
        public bool ConsumerSuccessfully { get; set; }
        public IEnumerable<TopicEvent> Messages { get; set; }
    }
}
