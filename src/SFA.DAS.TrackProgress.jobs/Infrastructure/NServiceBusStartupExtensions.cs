using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using SFA.DAS.NServiceBus.AzureFunction.Hosting;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.TrackProgress.Messages.Events;

namespace SFA.DAS.TrackProgress.Jobs.Infrastructure;

[ExcludeFromCodeCoverage]
public static class NServiceBusStartupExtensions
{
    public static IServiceCollection AddNServiceBus(
        this IServiceCollection serviceCollection,
        IConfiguration config)
    {
        var webBuilder = serviceCollection.AddWebJobs(x => { });
        webBuilder.AddExecutionContextBinding();
        webBuilder.AddExtension(new NServiceBusExtensionConfigProvider());

        var endpointConfiguration = new EndpointConfiguration("SFA.DAS.TrackProgress")
            .UseMessageConventions()
            .UseNewtonsoftJsonSerializer();

        if (config.NServiceBusConnectionString().Equals("UseLearningEndpoint=true", StringComparison.CurrentCultureIgnoreCase))
        {
            endpointConfiguration.UseTransport<LearningTransport>().StorageDirectory(LearningTransportLocal.Folder());
        }
        else
        {
            endpointConfiguration.UseAzureServiceBusTransport(config.NServiceBusConnectionString(), r => r.AddRouting());
        }

        if (!string.IsNullOrEmpty(config.NServiceBusLicense()))
        {
            endpointConfiguration.License(config.NServiceBusLicense());
        }

        endpointConfiguration.UseEndpointWithExternallyManagedService(serviceCollection);

        return serviceCollection;
    }
}

public static class RoutingSettingsExtensions
{
    public static void AddRouting(this RoutingSettings settings)
    {
        settings.RouteToEndpoint(typeof(NewProgressAddedEvent), QueueNames.NewProgressAdded);
    }
}

public static class QueueNames
{
    public const string NewProgressAdded = "sfa-das-trackprogress-newprogress-added";
}

public static class LearningTransportLocal
{
    private const string LearningTransportStorageDirectory = "LearningTransportStorageDirectory";

    public static string Folder()
    {
        var learningTransportFolder = Environment.GetEnvironmentVariable(LearningTransportStorageDirectory, EnvironmentVariableTarget.Process);
        if (learningTransportFolder == null)
        {
            learningTransportFolder = Path.Combine(
                Directory.GetCurrentDirectory()[
                    ..Directory.GetCurrentDirectory().IndexOf("src", StringComparison.Ordinal)],
                @"src\.learningtransport");
            SetFolder(learningTransportFolder);
        }
        return learningTransportFolder;
    }

    public static void SetFolder(string folder)
    {
        Environment.SetEnvironmentVariable(LearningTransportStorageDirectory, folder, EnvironmentVariableTarget.Process);
    }
}
