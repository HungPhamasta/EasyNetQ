﻿// ReSharper disable InconsistentNaming

using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.Consumer;
using EasyNetQ.Events;
using NUnit.Framework;
using RabbitMQ.Client;
using NSubstitute;

namespace EasyNetQ.Tests.HandlerRunnerTests
{
    [TestFixture]
    public class When_a_user_handler_is_executed
    {
        private IHandlerRunner handlerRunner;

        byte[] deliveredBody = null;
        MessageProperties deliveredProperties = null;
        MessageReceivedInfo deliveredInfo = null;

        readonly MessageProperties messageProperties = new MessageProperties
            {
                CorrelationId = "correlation_id"
            };
        readonly MessageReceivedInfo messageInfo = new MessageReceivedInfo("consumer_tag", 123, false, "exchange", "routingKey", "queue");
        readonly byte[] messageBody = new byte[0];

        private IModel channel;

        [SetUp]
        public void SetUp()
        {
            //var logger = new ConsoleLogger();
            var logger = Substitute.For<IEasyNetQLogger>();
            var consumerErrorStrategy = Substitute.For<IConsumerErrorStrategy>();
            var eventBus = new EventBus();

            handlerRunner = new HandlerRunner(logger, consumerErrorStrategy, eventBus);

            Func<byte[], MessageProperties, MessageReceivedInfo, Task> userHandler = (body, properties, info) => 
                Task.Factory.StartNew(() =>
                    {
                        deliveredBody = body;
                        deliveredProperties = properties;
                        deliveredInfo = info;
                    });

            var consumer = Substitute.For<IBasicConsumer>();
            channel = Substitute.For<IModel>();
            consumer.Model.Returns(channel);

            var context = new ConsumerExecutionContext(
                userHandler, messageInfo, messageProperties, messageBody, consumer);

            var autoResetEvent = new AutoResetEvent(false);
            eventBus.Subscribe<AckEvent>(x => autoResetEvent.Set());

            handlerRunner.InvokeUserMessageHandler(context);

            autoResetEvent.WaitOne(1000);
        }

        [Test]
        public void Should_deliver_body()
        {
            deliveredBody.ShouldBeTheSameAs(messageBody);
        }

        [Test]
        public void Should_deliver_properties()
        {
            deliveredProperties.ShouldBeTheSameAs(messageProperties);
        }

        [Test]
        public void Should_deliver_info()
        {
            deliveredInfo.ShouldBeTheSameAs(messageInfo);
        }

        [Test]
        public void Should_ACK_message()
        {
            channel.Received().BasicAck(123, false);
        }
    }
}

// ReSharper restore InconsistentNaming