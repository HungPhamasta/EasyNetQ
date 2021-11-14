using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace EasyNetQ.IntegrationTests.Rpc
{
    [Collection("RabbitMQ")]
    public class When_request_and_respond_with_publish_confirms : IDisposable
    {
        public When_request_and_respond_with_publish_confirms(RabbitMQFixture fixture)
        {
            bus = RabbitHutch.CreateBus($"host={fixture.Host};prefetchCount=1;publisherConfirms=True;timeout=5");
        }

        public void Dispose()
        {
            bus.Dispose();
        }

        private readonly IBus bus;

        [Fact]
        public async Task Should_receive_exception()
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            using (
                await bus.Rpc.RespondAsync<Request, Response>(
                    x => Task.FromException<Response>(new RequestFailedException()), timeoutCts.Token
                )
            )
            {
                await Assert.ThrowsAsync<EasyNetQResponderException>(
                    () => bus.Rpc.RequestAsync<Request, Response>(new Request(42), timeoutCts.Token)
                ).ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task Should_receive_response()
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            using (await bus.Rpc.RespondAsync<Request, Response>(x => new Response(x.Id), timeoutCts.Token))
            {
                var response = await bus.Rpc.RequestAsync<Request, Response>(new Request(42), timeoutCts.Token)
                    .ConfigureAwait(false);
                response.Should().Be(new Response(42));
            }
        }
    }
}
