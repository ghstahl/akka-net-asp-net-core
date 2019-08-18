using Akka.Actor;
using Bookstore.Extensions;
using System;

namespace BookstoreConsole
{
    public partial class Messages
    {
        public class ConsoleWriterMessages
        {
            public class PrintInstructions { }
            public class PrintMessage
            {
                public string Message { get; set; }
            }
        }
    }

    public class ConsoleWriterActor : ReceiveActor
    {
        public ConsoleWriterActor(IActorFactory actorFactory)
        {
            ReceiveAsync<Messages.ConsoleWriterMessages.PrintInstructions>(async _ => {
                DoPrintInstructions();
            });
            ReceiveAsync<Messages.ConsoleWriterMessages.PrintMessage>(async cmd => {
                var color =  ConsoleColor.Green;
                Console.ForegroundColor = color;
                Console.WriteLine(cmd.Message);
                Console.ResetColor();
            });
        }
        private void DoPrintInstructions()
        {
            Console.WriteLine("Commands!");
            Console.WriteLine("1 => Creates a new book.");
            Console.WriteLine("2 => gets all the books.");
            Console.WriteLine("3 => Reads a single book.");
            Console.WriteLine(" ");

            Console.WriteLine("Type 'exit' to quit this application at any time.\n");
        }
    }
}
