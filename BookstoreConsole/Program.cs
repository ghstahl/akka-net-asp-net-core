using Akka.Actor;
using Bookstore;
using Bookstore.Domain;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using Akka.DI.SimpleInjector;
using Akka.DI.Core;
using Bookstore.Extensions;
using Bookstore.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace BookstoreConsole
{
    class Program
    {
        static Container Container { get; set; }
        static void Main(string[] args)
        {
            Container = new Container();


            // Create service collection and configure our services
            var services = ConfigureServices();
            // Generate a provider
            var serviceProvider = services.BuildServiceProvider();

            var actorFactory = serviceProvider.GetService<IActorFactory>();
            Container.RegisterInstance(actorFactory);

            // Kick off our actual code
            serviceProvider.GetService<ConsoleApplication>().Run();
        }
        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSimpleInjector(Container);

            services.AddSingleton<ActorSystem>(sp =>
            {
                var system = ActorSystem.Create("bookstore", ConfigurationLoader.Load());
                return system;
            });

            services.AddSingleton<IDependencyResolver>(sp =>
            {
                var system = sp.GetService<ActorSystem>();
                IDependencyResolver resolver = new SimpleInjectorDependencyResolver(Container, system);
                return resolver;
            });
            services.AddSingleton<IActorFactory, MyActorFactory>();

            Container.AddInMemoryBookstoreStore();


            Container.Register<ILoggerFactory>(() =>
            {
                LoggerFactory factory = new LoggerFactory();

                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("logging.json")
                    .Build();

                //serilog provider configuration
                var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

                factory.AddSerilog(logger);

                return factory;
            }, Lifestyle.Singleton);
            Container.Register(typeof(ILogger<>), typeof(LoggingAdapter<>));


            //       services.AddActorProvider<BooksManagerActor>("BooksManagerActor");
            //       services.AddActorProvider<ConsoleReaderActor>("ConsoleReaderActor");

            // IMPORTANT! Register our application entry point
            services.AddTransient<ConsoleApplication>();
            return services;
        }

    }
}
