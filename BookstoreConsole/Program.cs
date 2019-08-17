using Akka.Actor;
using Bookstore;
using Bookstore.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using SimpleInjector;
using Akka.DI.SimpleInjector;
using Akka.DI.Core;
using Bookstore.Extensions;

namespace BookstoreConsole
{

    class Program
    {
        static Container _container { get; set; }
        static void Main(string[] args)
        {
            _container = new Container();


            // Create service collection and configure our services
            var services = ConfigureServices();
            // Generate a provider
            var serviceProvider = services.BuildServiceProvider();

            // Kick off our actual code
            serviceProvider.GetService<ConsoleApplication>().Run();
        }
        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            _container.Register<BooksManagerActor>();
            services.AddSingleton<ActorSystem>(sp =>
            {
                var system = ActorSystem.Create("bookstore", ConfigurationLoader.Load());
                return system;
            });

            services.AddSingleton<IDependencyResolver>(sp =>
            {
                var system = sp.GetService<ActorSystem>();
                IDependencyResolver resolver = new SimpleInjectorDependencyResolver(_container, system);
                return resolver;
            });


            //            services.AddSingleton(_ => ActorSystem.Create("bookstore", ConfigurationLoader.Load()));
            services.AddActorProvider<BooksManagerActor>("BooksManagerActor");

            services.AddTransient<ITestService, TestService>();
            // IMPORTANT! Register our application entry point
            services.AddTransient<ConsoleApplication>();
            return services;
        }
    }
}
