// ReSharper disable InconsistentNaming
using System;
using System.Threading;
using NUnit.Framework;
using RabbitMQ.Client;

namespace EasyNetQ.Tests
{
    [TestFixture]
    public class RequestResponseTests
    {
        private IBus bus;

        [SetUp]
        public void SetUp()
        {
            bus = RabbitHutch.CreateBus("host=localhost");
            while(!bus.IsConnected) Thread.Sleep(10);
        }

        [TearDown]
        public void TearDown()
        {
            bus.Dispose();
        }

        // First start the EasyNetQ.Tests.SimpleService console app.
        // Run this test. You should see the SimpleService report that it's
        // responding and the response should appear here.
        [Test, Explicit("Needs a Rabbit instance on localhost to work")]
        public void Should_be_able_to_do_simple_request_response()
        {
            var request = new TestRequestMessage {Text = "Hello from the client! "};

            Console.WriteLine("Making request");
            bus.Request<TestRequestMessage, TestResponseMessage>(request, response => 
                Console.WriteLine("Got response: '{0}'", response.Text));

            Thread.Sleep(2000);
        }

        // First start the EasyNetQ.Tests.SimpleService console app.
        // Run this test. You should see the SimpleService report that it's
        // responding to 1000 messages and you should see the messages return here.
        [Test, Explicit("Needs a Rabbit instance on localhost to work")]
        public void Should_be_able_to_do_simple_request_response_lots()
        {
            for (int i = 0; i < 1000; i++)
            {
                var request = new TestRequestMessage { Text = "Hello from the client! " + i.ToString() };
                bus.Request<TestRequestMessage, TestResponseMessage>(request, response =>
                    Console.WriteLine("Got response: '{0}'", response.Text));
            }

            Thread.Sleep(3000);
        }

        // First start the EasyNetQ.Tests.SimpleService console app.
        // Run this test. You should see the SimpleService report that it's
        // responding and the response should appear here.
        [Test, Explicit("Needs a Rabbit instance on localhost to work")]
        public void Should_be_able_to_make_a_request_that_runs_async_on_the_server()
        {
            var request = new TestAsyncRequestMessage {Text = "Hello async from the client!"};

            Console.Out.WriteLine("Making request");
            bus.Request<TestAsyncRequestMessage, TestAsyncResponseMessage>(request, 
                response => Console.Out.WriteLine("response = {0}", response.Text));

            Thread.Sleep(2000);
        }

        // First start the EasyNetQ.Tests.SimpleService console app.
        // Run this test. You should see 1000 response messages on the SimpleService
        // and then 1000 messages appear back here.
        [Test, Explicit("Needs a Rabbit instance on localhost to work")]
        public void Should_be_able_to_make_many_async_requests()
        {
            for (int i = 0; i < 1000; i++)
            {
                var request = new TestAsyncRequestMessage { Text = "Hello async from the client! " + i };

                bus.Request<TestAsyncRequestMessage, TestAsyncResponseMessage>(request,
                    response =>
                    Console.Out.WriteLine("response = {0}", response.Text));
            }
            Thread.Sleep(5000);
        }

        /// <summary>
        /// First start the EasyNetQ.Tests.SimpleService console app.
        /// Run this test. You should see an error message written to the error queue
        /// and an error logged
        /// </summary>
        [Test, Explicit("Needs a Rabbit instance on localhost to work")]
        public void Service_should_handle_sychronous_message_of_the_wrong_type()
        {
            const string routingKey = "EasyNetQ_Tests_TestRequestMessage:EasyNetQ_Tests_Messages";
            const string type = "not_the_type_you_are_expecting";

            MakeRpcRequest(type, routingKey);
        }

        /// <summary>
        /// First start the EasyNetQ.Tests.SimpleService console app.
        /// Run this test. You should see an error message written to the error queue
        /// and an error logged
        /// </summary>
        [Test, Explicit("Needs a Rabbit instance on localhost to work")]
        public void Service_should_handle_asychronous_message_of_the_wrong_type()
        {
            const string routingKey = "EasyNetQ_Tests_TestAsyncRequestMessage:EasyNetQ_Tests_Messages";
            const string type = "not_the_type_you_are_expecting";

            MakeRpcRequest(type, routingKey);
        }

        private static void MakeRpcRequest(string type, string routingKey)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            using (var connection = connectionFactory.CreateConnection())
            using (var model = connection.CreateModel())
            {
                var properties = model.CreateBasicProperties();
                properties.Type = type;
                model.BasicPublish(
                    "easy_net_q_rpc", // exchange
                    routingKey, // routing key
                    false, // manditory
                    false, // immediate
                    properties,
                    new byte[0]);
            }
        }
    }
}

// ReSharper restore InconsistentNaming