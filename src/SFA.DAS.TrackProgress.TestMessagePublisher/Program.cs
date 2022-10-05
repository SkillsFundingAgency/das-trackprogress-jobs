using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NServiceBus;
using SFA.DAS.TrackProgress;
using SFA.DAS.TrackProgress.TestMessagePublisher;

IConfiguration config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(loggingBuilder =>
        loggingBuilder.AddFilter<ConsoleLoggerProvider>(level =>
            level == LogLevel.None))
    .ConfigureServices((_, services) =>
    {
        NServiceBusLocalHelper.Add(services);
    })
    .Build();

while (true)
{
    Console.Clear();
    Console.WriteLine("To Publish an Event please select the option...");
    Console.WriteLine("1. Publish NewProgressAddedEvent");
    Console.WriteLine("X. Exit");

    var choice = Console.ReadLine()?.ToLower();
    switch (choice)
    {
        case "1":
            await PublishNewProgressAddedEvent(host.Services);
            break;
        case "x":
            return; 
    }
}

async Task PublishNewProgressAddedEvent(IServiceProvider services)
{
    using IServiceScope serviceScope = services.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;
    var messagePublisher = provider.GetRequiredService<IMessageSession>();

    await messagePublisher.Publish(new NewProgressAddedEvent { CommitmentsApprenticeId = 34254 });

    Console.WriteLine("Message published.");
    Console.WriteLine("Press enter to continue");
    Console.ReadLine();
}