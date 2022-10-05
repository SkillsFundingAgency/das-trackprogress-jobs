using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.TrackProgress.Jobs.Infrastructure;

namespace SFA.DAS.TrackProgress.TestMessagePublisher
{
    internal class NServiceBusLocalHelper
    {
        internal static void Add(IServiceCollection serviceCollection)
        {
            var endpointName = "SFA.DAS.TrackProgress";

            var endpointConfiguration = new EndpointConfiguration(endpointName)
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer();

            endpointConfiguration.UseTransport<LearningTransport>().StorageDirectory(LearningTransportLocal.Folder());
            endpointConfiguration.UseLearningTransport(s => s.AddRouting());

            var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            serviceCollection.AddSingleton(p => endpoint)
                .AddSingleton<IMessageSession>(p => p.GetService<IEndpointInstance>())
                .AddHostedService<NServiceBusHostedService>();
        }

    }
}
