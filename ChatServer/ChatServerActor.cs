using Akka.Actor;
using ChatMessages;
using System.Collections.Generic;

namespace ChatServer
{
    class ChatServerActor : ReceiveActor, ILogReceive
    {
        private readonly HashSet<IActorRef> _clients = new HashSet<IActorRef>();

        public ChatServerActor()
        {
            Receive<SayRequest>(message =>
            {
                var response = new SayResponse
                {
                    Username = message.Username,
                    Text = message.Text,
                };
                foreach (var client in _clients) client.Tell(response, Self);
            });

            Receive<ConnectRequest>(message =>
            {
                _clients.Add(Sender);
                Sender.Tell(new ConnectResponse
                {
                    Message = "Hello and welcome to Akka.NET chat example",
                }, Self);
            });

            Receive<NickRequest>(message =>
            {
                var response = new NickResponse
                {
                    OldUsername = message.OldUsername,
                    NewUsername = message.NewUsername,
                };

                foreach (var client in _clients) client.Tell(response, Self);
            });
        }
    }
}
