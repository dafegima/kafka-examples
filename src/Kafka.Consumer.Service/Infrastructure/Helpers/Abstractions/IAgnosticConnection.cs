namespace Kafka.Consumer.Service.Infrastructure.Helpers.Abstractions
{
    public interface IAgnosticConnection<out TInstance>
    {
        TInstance Instance { get; }

        TInstance NewInstance { get; }
    }
}
