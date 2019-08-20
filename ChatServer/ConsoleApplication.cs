using Akka.Actor;
using Bookstore;
using Bookstore.Domain;
using Bookstore.Extensions;

namespace ChatServer
{
    public class ConsoleApplication
    {
        private readonly ActorSystem _system;
        private readonly IActorFactory _actorFactory;
        private readonly IActorRef _chatServerActor;
        public ConsoleApplication(
            ActorSystem system,
            IActorFactory actorFactory)
        {
            _system = system;
            _actorFactory = actorFactory;
            _chatServerActor = _actorFactory.CreateActor<ChatServerActor>("ChatServer");
        }

        // Application starting point
        public void Run()
        {
            // tell console reader to begin
         //   _chatServerActor.Tell("start");
            // blocks the main thread from exiting until the actor system is shut down
            _system.WhenTerminated.Wait();
        }
    }
}
