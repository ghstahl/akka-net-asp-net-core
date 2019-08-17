using Akka.Actor;
using Bookstore;
using Bookstore.Domain;

namespace BookstoreConsole
{
    public class ConsoleApplication
    {
        private readonly IActorRef _booksManagerActor;
        private readonly ITestService _testService;
        public ConsoleApplication(
            ActorProvider<BooksManagerActor> booksManagerActorProvider,
            ITestService testService)
        {
            _booksManagerActor = booksManagerActorProvider();
            _testService = testService;
        }

        // Application starting point
        public void Run()
        {
            
           _testService.DoSomethingUseful();
        }
    }
}
