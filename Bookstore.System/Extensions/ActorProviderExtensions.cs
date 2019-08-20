using Akka.Actor;
using Akka.DI.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Extensions
{
    public interface IActorFactory
    {
        IActorRef CreateActor<T>() where T : UntypedActor;
    }
    public class MyActorFactory : IActorFactory
    {
        private ActorSystem _system;
        private IDependencyResolver _dependencyResolver;
        Dictionary<Type, IActorRef> _actorMap;
        private Dictionary<Type, IActorRef> ActorMap
        {
            get
            {
                if(_actorMap == null)
                {
                    _actorMap  = new Dictionary<Type, IActorRef>();
                }
                return _actorMap;
            }
        }
        public MyActorFactory(ActorSystem system, IDependencyResolver dependencyResolver)
        {
            _system = system;
            _dependencyResolver = dependencyResolver;
        }
        public IActorRef CreateActor<T>() where T : UntypedActor
        {
            if (ActorMap.ContainsKey(typeof(T)))
            {
                return ActorMap[typeof(T)];
            }
            else
            {
                var actor = _system.ActorOf(_dependencyResolver.Create<T>());
                ActorMap[typeof(T)] = actor;
                return actor;
            }
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
