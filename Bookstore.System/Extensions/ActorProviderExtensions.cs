using Akka.Actor;
using Akka.DI.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Bookstore.Extensions
{
    public interface IActorFactory
    {
        IActorRef CreateActor<T>(string name = null) where T : ActorBase;
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
        public IActorRef CreateActor<TActor>(Expression<Func<TActor>> factory,string name = null) where TActor : ActorBase
        {
            return _system.ActorOf(Props.Create(factory), name);
        }

        public IActorRef CreateActor<TActor>(string name = null) where TActor : ActorBase
    {
            if (ActorMap.ContainsKey(typeof(ActorBase)))
            {
                return ActorMap[typeof(ActorBase)];
            }
            else
            {
                var actor = _system.ActorOf(_dependencyResolver.Create<TActor>(),name);
                ActorMap[typeof(ActorBase)] = actor;
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
