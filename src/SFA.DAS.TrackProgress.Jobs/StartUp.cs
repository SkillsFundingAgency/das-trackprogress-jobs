using System;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using RestEase.HttpClientFactory;
using SFA.DAS.Http.Configuration;
using SFA.DAS.NServiceBus.AzureFunction.Extensions;
using SFA.DAS.NServiceBus.Extensions;
using SFA.DAS.TrackProgress.Jobs.Api;
using SFA.DAS.TrackProgress.Jobs.Infrastructure;

[assembly: FunctionsStartup(typeof(SFA.DAS.TrackProgress.Jobs.Startup))]
namespace SFA.DAS.TrackProgress.Jobs;

public class Startup : FunctionsStartup
{
    public IConfiguration Configuration { get; set; }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        builder.ConfigureConfiguration();
        builder.ConfigureServiceBusManagedIdentity();
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        Configuration = builder.GetContext().Configuration;

        var useManagedIdentity = !Configuration.IsLocalAcceptanceOrDev();

        builder.Services.AddApplicationInsightsTelemetry();
        builder.Services.AddLogging();

        builder.Services.AddApplicationOptions();
        builder.Services.ConfigureFromOptions(f => f.TrackProgressInternalApi);
        builder.Services.AddSingleton<IApimClientConfiguration>(x => x.GetRequiredService<TrackProgressApiOptions>());

        typeof(Startup).Assembly.AutoSubscribeToQueuesWithReflection(Configuration!, useManagedIdentity).GetAwaiter().GetResult();

        builder.UseNServiceBus((IConfiguration appConfiguration) =>
        {
            try
            {
                var configuration = ServiceBusEndpointFactory.CreateSingleQueueConfiguration(QueueNames.TrackProgress, appConfiguration, useManagedIdentity);
                configuration.AdvancedConfiguration.UseNewtonsoftJsonSerializer();
                configuration.AdvancedConfiguration.UseMessageConventions();
                configuration.AdvancedConfiguration.EnableInstallers();
                return configuration;
            }
            catch (Exception e)
            {
                throw new Exception($"Problem configuring NSB {e.Message}", e);
            }
        });

        if (!Configuration.IsLocalAcceptanceOrDev())
        {
            builder.Services.AddSingleton<ServiceBusClient>(new ServiceBusClient(Configuration["AzureWebJobsServiceBus__fullyQualifiedNamespace"], new DefaultAzureCredential()));
        }

        builder.Services.AddSingleton<IApimClientConfiguration>(x => x.GetRequiredService<TrackProgressApiOptions>());
        builder.Services.AddTransient<Http.MessageHandlers.DefaultHeadersHandler>();
        builder.Services.AddTransient<Http.MessageHandlers.LoggingMessageHandler>();
        builder.Services.AddTransient<Http.MessageHandlers.ApimHeadersHandler>();

        var url = builder.Services
            .BuildServiceProvider()
            .GetRequiredService<TrackProgressApiOptions>()
            .ApiBaseUrl;

        builder.Services.AddRestEaseClient<ITrackProgressOuterApi>(url)
            .AddHttpMessageHandler<Http.MessageHandlers.DefaultHeadersHandler>()
            .AddHttpMessageHandler<Http.MessageHandlers.ApimHeadersHandler>()
            .AddHttpMessageHandler<Http.MessageHandlers.LoggingMessageHandler>();
    }
}