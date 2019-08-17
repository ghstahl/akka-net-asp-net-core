using Akka.Actor;
using Akka.DI.Core;
using Bookstore.Contracts;
using Bookstore.Stores;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Extensions
{
    public static class SimpleInjectorExtensions
    {
        public static void AddInMemoryBookstoreStore(this Container container)
        {
            container.Register<IBookstoreStore, InMemoryBookstoreStore>();
        }
    }
    public static class ActorProviderExtensions
    {
        
        public static IServiceCollection AddActorProvider<T>(
            this IServiceCollection services, string name)
            where T : UntypedActor
        {
            services.AddSingleton<ActorProvider<T>>(provider =>
            {
                var system = provider.GetService<ActorSystem>();
                var resolver = provider.GetService<IDependencyResolver>();
                var actor = system.ActorOf(resolver.Create<T>(), name);
                return () => actor;
            });
            return services;
        }
    }
}
