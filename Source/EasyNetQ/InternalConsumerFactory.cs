﻿namespace EasyNetQ
{
    public interface IInternalConsumerFactory
    {
        IInternalConsumer CreateConsumer();
    }

    public class InternalConsumerFactory : IInternalConsumerFactory
    {
        private readonly IHandlerRunner handlerRunner;
        private readonly IEasyNetQLogger logger;
        private readonly IConventions conventions;
        private readonly IConnectionConfiguration connectionConfiguration;
        private readonly IConsumerDispatcherFactory consumerDispatcherFactory;

        public InternalConsumerFactory(
            IHandlerRunner handlerRunner, 
            IEasyNetQLogger logger, 
            IConventions conventions, 
            IConnectionConfiguration connectionConfiguration, 
            IConsumerDispatcherFactory consumerDispatcherFactory)
        {
            this.handlerRunner = handlerRunner;
            this.logger = logger;
            this.conventions = conventions;
            this.connectionConfiguration = connectionConfiguration;
            this.consumerDispatcherFactory = consumerDispatcherFactory;
        }

        public IInternalConsumer CreateConsumer()
        {
            var dispatcher = consumerDispatcherFactory.GetConsumerDispatcher();
            return new InternalConsumer(handlerRunner, logger, dispatcher, conventions, connectionConfiguration);
        }
    }
}