using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.SimpleInjector;
using Bookstore.Extensions;
using Bookstore.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SimpleInjector;

namespace ChatClient
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
                var system = ActorSystem.Create("ChatClient", ConfigurationLoader.Load());
                return system;
            });

            services.AddSingleton<IDependencyResolver>(sp =>
            {
                var system = sp.GetService<ActorSystem>();
                IDependencyResolver resolver = new SimpleInjectorDependencyResolver(Container, system);
                return resolver;
            });
            services.AddSingleton<IActorFactory, MyActorFactory>();


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



            // IMPORTANT! Register our application entry point
            services.AddTransient<ConsoleApplication>();
            return services;
        }
    }
}
