using Akka.Actor;
using Bookstore.Extensions;
using System;

namespace BookstoreConsole
{
    public class ConsoleWriterActor : ReceiveActor
    {
        public ConsoleWriterActor(IActorFactory actorFactory)
        {
            ReceiveAsync<string>(async message => {
                var msg = message as string;
                var color =  ConsoleColor.Green;
                Console.ForegroundColor = color;
                Console.WriteLine(msg);
                Console.ResetColor();
            });
        }
    }
}
