using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestEase.HttpClientFactory;
using SFA.DAS.Http.Configuration;
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
        var serviceProvider = builder.Services.BuildServiceProvider();
        Configuration = serviceProvider.GetService<IConfiguration>();

        // use when sharing bertween applications
        // LearningTransportLocal.SetFolder(@"c:\scratch\.learningtransport");

        builder.Services.AddApplicationInsightsTelemetry();
        builder.Services.AddLogging();

        builder.Services.AddApplicationOptions();
        builder.Services.ConfigureFromOptions(f => f.TrackProgressApi);
        builder.Services.AddSingleton<IApimClientConfiguration>(x => x.GetRequiredService<TrackProgressApiOptions>());
        builder.Services.AddNServiceBus(Configuration);

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
            .AddHttpMessageHandler<Http.MessageHandlers.LoggingMessageHandler>()
            //.AddTypedClient<>
            ;


    }
}