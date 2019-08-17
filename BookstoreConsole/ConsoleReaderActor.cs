using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookstoreConsole
{
    public class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "exit";
        private IActorRef _booksActorController;

        public ConsoleReaderActor(IActorRef booksActorController)
        {
            _booksActorController = booksActorController;
        }
        protected override void OnReceive(object message)
        {
            var read = Console.ReadLine();
            if (
                   !string.IsNullOrEmpty(read) 
                && String.Equals(read, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                // shut down the system (acquire handle to system via
                // this actors context)
                Context.System.Terminate();
                return;
            }

            // send input to the console writer to process and print
            _booksActorController.Tell(read);

            // continue reading messages from the console
            Self.Tell("continue");
        }
    }
}
