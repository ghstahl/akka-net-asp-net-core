using Akka.Actor;
using Akka.DI.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Bookstore
{
    public delegate IActorRef ActorProvider<T>() where T: UntypedActor;

  
}