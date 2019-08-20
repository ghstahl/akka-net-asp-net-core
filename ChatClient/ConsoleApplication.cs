using Akka.Actor;
using Bookstore;
using Bookstore.Domain;
using Bookstore.Extensions;
using ChatMessages;
using System;
using System.Linq;

namespace ChatClient
{
    public class ConsoleApplication
    {
        private readonly ActorSystem _system;
        private readonly IActorFactory _actorFactory;
        private readonly IActorRef _chatClientActor;
        public ConsoleApplication(
            ActorSystem system,
            IActorFactory actorFactory)
        {
            _system = system;
            _actorFactory = actorFactory;
            _chatClientActor = _actorFactory.CreateActor<ChatClientActor>("ChatClient");
        }

        // Application starting point
        public void Run()
        {
            // tell console reader to begin
            //   _chatServerActor.Tell("start");
            // blocks the main thread from exiting until the actor system is shut down

            _chatClientActor.Tell(new ConnectRequest()
            {
                Username = "Roggan",
            });

            while (true)
            {
                var input = Console.ReadLine();
                if (input.StartsWith("/"))
                {
                    var parts = input.Split(' ');
                    var cmd = parts[0].ToLowerInvariant();
                    var rest = string.Join(" ", parts.Skip(1));

                    if (cmd == "/nick")
                    {
                        _chatClientActor.Tell(new NickRequest
                        {
                            NewUsername = rest
                        });
                    }
                    if (cmd == "/exit")
                    {
                        Console.WriteLine("exiting");
                        break;
                    }
                }
                else
                {
                    _chatClientActor.Tell(new SayRequest()
                    {
                        Text = input,
                    });
                }
            }

            _system.WhenTerminated.Wait();
        }
    }
}
