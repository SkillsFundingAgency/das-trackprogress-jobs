﻿using System;
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
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        Configuration = builder.GetContext().Configuration;

        var useManagedIdentity = !Configuration.IsAcceptanceOrDev();

        builder.Services.AddApplicationInsightsTelemetry();
        builder.Services.AddLogging();

        builder.Services.AddApplicationOptions();
        builder.Services.ConfigureFromOptions(f => f.TrackProgressInternalApi);
        builder.Services.AddSingleton<IApimClientConfiguration>(x => x.GetRequiredService<TrackProgressApiOptions>());

        InitialiseNServiceBus();

        builder.UseNServiceBus((IConfiguration appConfiguration) =>
        {
            var configuration = ServiceBusEndpointFactory.CreateSingleQueueConfiguration(QueueNames.TrackProgress, appConfiguration, useManagedIdentity);
            configuration.AdvancedConfiguration.UseNewtonsoftJsonSerializer();
            configuration.AdvancedConfiguration.UseMessageConventions();
            configuration.AdvancedConfiguration.EnableInstallers();
            return configuration;
        });

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

    public void InitialiseNServiceBus()
    {
        var m = new NServiceBusResourceManager(Configuration, !Configuration.IsLocalAcceptanceOrDev());
        m.CreateWorkAndErrorQueues(QueueNames.TrackProgress).GetAwaiter().GetResult();
        m.SubscribeToTopicForQueue(typeof(Startup).Assembly, QueueNames.TrackProgress).GetAwaiter().GetResult();
    }
}