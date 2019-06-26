using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using Shared;

namespace HostApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddScoped<SharedContext>();

            //NServiceBus:
            var endpointConfiguration = new EndpointConfiguration("Host");

            //Transport:
            endpointConfiguration.UseTransport<LearningTransport>();

            //Container:
            var builder = new ContainerBuilder();

            builder.Populate(services);

            var container = builder.Build();

            endpointConfiguration.UseContainer<AutofacBuilder>(customizations => customizations.ExistingLifetimeScope(container));

            var pipeline = endpointConfiguration.Pipeline;
            pipeline.Register(typeof(MyOutgoingBehavior), "Maps outgoing contexts");
            pipeline.Register(typeof(MyIncomingBehaviour), "Maps incoming contexts");

            //Endpoint:
            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
