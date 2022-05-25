namespace Kafka.Consumer.Service.Infrastructure.Helpers.Abstractions
{
    public class AgnosticConnection<T>
        : AgnosticConnection<T, T>
    {
        protected AgnosticConnection(T connection)
            : base(connection)
        {
        }

        protected override T Resolve(T connectionSettings) => connectionSettings;
    }

    public abstract class AgnosticConnection<TInstance, TConnectionSettings>
        : IAgnosticConnection<TInstance>
    {
        public TInstance Instance => Get();

        public TInstance NewInstance => Get(flush: true);

        private TInstance _instance;

        private readonly TConnectionSettings _connectionSettings;

        protected AgnosticConnection(TConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;
        }

        protected TInstance Get(bool flush = false)
        {
            if (_instance == null || flush)
            {
                _instance = Resolve(_connectionSettings);
            }

            return _instance;
        }

        protected abstract TInstance Resolve(TConnectionSettings connectionSettings);
    }
}