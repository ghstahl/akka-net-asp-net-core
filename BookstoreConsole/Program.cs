using Akka.Actor;
using Bookstore;
using Bookstore.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using SimpleInjector;
using Akka.DI.SimpleInjector;
using Akka.DI.Core;
using Bookstore.Extensions;
using Bookstore.Contracts;

namespace BookstoreConsole
{

    class Program
    {
        static Container _simpleInjectorContainer { get; set; }
        static void Main(string[] args)
        {
            _simpleInjectorContainer = new Container();


            // Create service collection and configure our services
            var services = ConfigureServices();
            // Generate a provider
            var serviceProvider = services.BuildServiceProvider();

            var actorFactory = serviceProvider.GetService<IActorFactory>();
            _simpleInjectorContainer.RegisterInstance(actorFactory);

            // Kick off our actual code
            serviceProvider.GetService<ConsoleApplication>().Run();
        }
        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSimpleInjector(_simpleInjectorContainer);
          
            services.AddSingleton<ActorSystem>(sp =>
            {
                var system = ActorSystem.Create("bookstore", ConfigurationLoader.Load());
                return system;
            });

            services.AddSingleton<IDependencyResolver>(sp =>
            {
                var system = sp.GetService<ActorSystem>();
                IDependencyResolver resolver = new SimpleInjectorDependencyResolver(_simpleInjectorContainer, system);
                return resolver;
            });
            services.AddSingleton<IActorFactory, MyActorFactory>();
         
            _simpleInjectorContainer.AddInMemoryBookstoreStore();

     //       services.AddActorProvider<BooksManagerActor>("BooksManagerActor");
     //       services.AddActorProvider<ConsoleReaderActor>("ConsoleReaderActor");
            
            // IMPORTANT! Register our application entry point
            services.AddTransient<ConsoleApplication>();
            return services;
        }
    }
}
