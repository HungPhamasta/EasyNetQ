#if !NETFX

using System;
using EasyNetQ.DI.Microsoft;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyNetQ.DI.Tests
{
    public class MicrosoftDependencyScopeTests
    {
        [Theory]
        [InlineData(Lifetime.Transient, true)]
        [InlineData(Lifetime.Singleton, false)]
        public void CreateScope_TransientService_ShouldBeDisposed(Lifetime lifetime, bool shouldBeDisposed)
        {
            var resolver = CreateResolver(c => c.Register<IService, Service>(lifetime));

            var service = ResolveFromScope(resolver);

            Assert.Equal(shouldBeDisposed, service.Disposed);
        }

        private static IService ResolveFromScope(IServiceResolver resolver)
        {
            using (var scope = resolver.CreateScope())
            {
                return scope.Resolve<IService>();
            }
        }

        private static IServiceResolver CreateResolver(Action<IServiceRegister> configure)
        {
            var services = new ServiceCollection();
            var adapter = new ServiceCollectionAdapter(services);
            configure.Invoke(adapter);
            return services.BuildServiceProvider().GetService<IServiceResolver>();
        }

        private interface IService : IDisposable
        {
            bool Disposed { get; set; }
        }

        private class Service : IService
        {
            public bool Disposed { get; set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }

        [Fact]
        public void CreateScope_TransientServiceInDifferentScopes_ShouldNotBeSame()
        {
            var resolver = CreateResolver(c => c.Register<IService, Service>(Lifetime.Transient));

            var service1 = ResolveFromScope(resolver);
            var service2 = ResolveFromScope(resolver);

            Assert.NotSame(service1, service2);
        }

        [Fact]
        public void CreateScope_TransientServiceInSameScope_ShouldBeSingleton()
        {
            var services = new ServiceCollection();
            services.AddScoped<IService, Service>();
            var resolver = services.BuildServiceProvider().GetService<IServiceResolver>();

            IService service1;
            IService service2;
            using (var scope = resolver.CreateScope())
            {
                service1 = scope.Resolve<IService>();
                service2 = scope.Resolve<IService>();
            }

            Assert.Same(service1, service2);
        }
    }
}

#endif
