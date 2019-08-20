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
        private IActorRef _badActorActor;

        public ConsoleReaderActor(IActorFactory actorFactory)
        {
            _actorFactory = actorFactory;
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
                        if (latestGuid != null)
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
                    case "4":
                        _badActorActor.Tell(new Messages.BadActorMessage.DoThrowUnknownExcpetion());
                        break;
                    case "5":
                        _badActorActor.Tell(new Messages.BadActorMessage.DoThrownArithmeticException());
                        break;
                    case "6":
                        _badActorActor.Tell(new Messages.BadActorMessage.DoThrownInsanelyBadException());
                        break;
                    case "7":
                        _badActorActor.Tell(new Messages.BadActorMessage.DoNotSupportedException());
                        break;
                    default:
                        if (!string.IsNullOrEmpty(read) &&
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
       
        protected override void PreStart()
        {
            base.PreStart();
            _booksManagerActor = _actorFactory.CreateActor<BooksManagerActor>();
            _consoleWriterActor = _actorFactory.CreateActor<ConsoleWriterActor>();
            _badActorActor = Context.ActorOf<BadActor>();

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(5),
                _badActorActor,
                new Messages.BadActorMessage.DoThrownInsanelyBadException(),
                ActorRefs.Nobody);
        }
      
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(// or AllForOneStrategy
                maxNrOfRetries: 10,
                withinTimeRange: TimeSpan.FromSeconds(30),
                localOnlyDecider: x =>
                {
                    // Maybe ArithmeticException is not application critical
                    // so we just ignore the error and keep going.
                    if (x is ArithmeticException) return Directive.Resume;

                    // Error that we have no idea what to do with
                    else if (x is InsanelyBadException) return Directive.Escalate;

                    // Error that we can't recover from, stop the failing child
                    else if (x is NotSupportedException) return Directive.Stop;

                    // otherwise restart the failing child
                    else return Directive.Restart;
                });
        }

    }
}
