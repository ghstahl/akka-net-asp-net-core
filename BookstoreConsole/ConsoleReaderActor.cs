using Akka.Actor;
using Bookstore;
using Bookstore.Domain;
using Bookstore.Dto;
using Bookstore.Extensions;
using Bookstore.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookstoreConsole
{
    public class ConsoleReaderActor : ReceiveActor
    {

        public const string ExitCommand = "exit";
        public const string CreateCommand = "create";
        public const string GetCommand = "get";
        public const string GetAllCommand = "get-all";
        private IActorFactory _actorFactory;
        private IActorRef _booksManagerActor;
        private IActorRef _consoleWriterActor;
        public ConsoleReaderActor(IActorFactory actorFactory)
        {
            _actorFactory = actorFactory;
            _booksManagerActor = _actorFactory.CreateActor<BooksManagerActor>("BooksManagerActor");
            _consoleWriterActor = _actorFactory.CreateActor<ConsoleWriterActor>("ConsoleWriterActor");
            

            ReceiveAsync<object>(async _ =>
            {
                _consoleWriterActor.Tell(new Messages.ConsoleWriterMessages.PrintInstructions());
                var read = Console.ReadLine();
                
                switch (read)
                {
                    case "1":
                        CreateBook createBook = new CreateBook("Domain-driven design", "Eric J. Evans", 500, 20);
                        _booksManagerActor.Tell(createBook);
                        break;
                    case "2":
                        var books = await _booksManagerActor.Ask<IEnumerable<BookDto>>(GetBooks.Instance);

                        string json = JsonConvert.SerializeObject(books, Formatting.Indented);
                        _consoleWriterActor.Tell(new Messages.ConsoleWriterMessages.PrintMessage { Message = json });
                        break;
                    case "3":
                        BookDto bookDto = null;
                        var latestGuid = await _booksManagerActor.Ask<Guid>(new GetLatestGuid());
                        if(latestGuid != null)
                        {
                            bookDto = await _booksManagerActor.Ask<BookDto>(new GetBookById(latestGuid));
                        }
                        if (bookDto == null)
                        {
                            _consoleWriterActor.Tell(new Messages.ConsoleWriterMessages.PrintMessage { Message = "Not found" });
                        }
                        else
                        {
                            string jsonBookDto = JsonConvert.SerializeObject(bookDto, Formatting.Indented);
                            _consoleWriterActor.Tell(new Messages.ConsoleWriterMessages.PrintMessage { Message = jsonBookDto });

                        }

                        break;
                    default:
                        if ( !string.IsNullOrEmpty(read) && 
                        String.Equals(read, ExitCommand, StringComparison.OrdinalIgnoreCase))
                        {
                            // shut down the system (acquire handle to system via
                            // this actors context)
                            Context.System.Terminate();
                            return;
                        }
                        break;
                }
 
                Self.Tell("continue");
            });

        }
       
    }
}
