using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.SimpleInjector;
using Bookstore.Domain;
using Bookstore.Extensions;
using Bookstore.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using System;

namespace Bookstore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _simpleInjectorContainer = new SimpleInjector.Container();
        }

        public IConfiguration Configuration { get; }

        private SimpleInjector.Container _simpleInjectorContainer;

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {      // Integrate with Simple Injector
            services.AddSimpleInjector(_simpleInjectorContainer);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Register ActorSystem
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
            services.AddActorProvider<BooksManagerActor>("BooksManagerActor");
            _simpleInjectorContainer.AddInMemoryBookstoreStore();

            var serviceProvider = services
                // Save yourself pain and agony and always use "validateScopes: true"
                .BuildServiceProvider(validateScopes: true)
                // Ensures framework components can be retrieved
                .UseSimpleInjector(_simpleInjectorContainer);
            return serviceProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            lifetime.ApplicationStarted.Register(() =>
            {
                app.ApplicationServices.GetService<ActorSystem>(); // start Akka.NET
            });
            lifetime.ApplicationStopping.Register(() =>
            {
                app.ApplicationServices.GetService<ActorSystem>().Terminate().Wait();
            });
        }
    }
}
