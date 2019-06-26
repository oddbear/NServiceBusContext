using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using Shared;

namespace ApiApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(mvcOptions => mvcOptions.Filters.Add(typeof(MySampleActionFilter)))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddScoped<SharedContext>();

            return SetupNServiceBus(services);
        }

        private IServiceProvider SetupNServiceBus(IServiceCollection services)
        {
            IMessageSession endpointInstance = null;
            services.AddSingleton<IMessageSession>(p => endpointInstance);

            var endpointConfiguration = new EndpointConfiguration("Api");

            //Transport:
            endpointConfiguration.UseTransport<LearningTransport>();

            endpointConfiguration.SendOnly();

            //Container:
            var builder = new ContainerBuilder();

            builder.Populate(services);

            var container = builder.Build();

            endpointConfiguration.UseContainer<AutofacBuilder>(customizations => customizations.ExistingLifetimeScope(container));

            var pipeline = endpointConfiguration.Pipeline;
            pipeline.Register(typeof(MyOutgoingBehavior), "Maps outgoing contexts");
            pipeline.Register(typeof(MyIncomingBehaviour), "Maps incoming contexts");

            //Endpoint:
            endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
