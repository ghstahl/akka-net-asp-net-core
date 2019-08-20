using Akka.Actor;
using ChatMessages;
using System;

namespace ChatClient
{
    class ChatClientActor : ReceiveActor, ILogReceive
    {
        private string _nick = "Roggan";
        private readonly ActorSelection _server = Context.ActorSelection("akka.tcp://ChatServer@localhost:8081/user/ChatServer");

        public ChatClientActor()
        {
            Receive<ConnectRequest>(cr =>
            {
                Console.WriteLine("Connecting....");
                _server.Tell(cr);
            });

            Receive<ConnectResponse>(rsp =>
            {
                Console.WriteLine("Connected!");
                Console.WriteLine(rsp.Message);
            });

            Receive<NickRequest>(nr =>
            {
                nr.OldUsername = _nick;
                Console.WriteLine("Changing nick to {0}", nr.NewUsername);
                _nick = nr.NewUsername;
                _server.Tell(nr);
            });

            Receive<NickResponse>(nrsp =>
            {
                Console.WriteLine("{0} is now known as {1}", nrsp.OldUsername, nrsp.NewUsername);
            });

            Receive<SayRequest>(sr =>
            {
                sr.Username = _nick;
                _server.Tell(sr);
            });

            Receive<SayResponse>(srsp =>
            {
                Console.WriteLine("{0}: {1}", srsp.Username, srsp.Text);
            });
        }
    }
}
