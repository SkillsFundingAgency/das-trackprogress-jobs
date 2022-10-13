using Microsoft.Extensions.Configuration;
using NServiceBus;
using SFA.DAS.NServiceBus.Extensions;
using SFA.DAS.TrackProgress.Messages.Events;

IConfiguration config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.development.json", optional: true)
    .Build();

var connectionString = config["NServiceBusConnection"];
if (connectionString is null)
    throw new Exception("NServiceBusConnection should contain ServiceBus connection string");


var endpointConfiguration = new EndpointConfiguration("SFA.DAS.TrackProgress");
endpointConfiguration.EnableInstallers();
endpointConfiguration.UseMessageConventions();
endpointConfiguration.UseNewtonsoftJsonSerializer();

endpointConfiguration.SendOnly();

var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();

transport.ConnectionString(connectionString);

var endpointInstance = await Endpoint.Start(endpointConfiguration)
    .ConfigureAwait(false);

try
{
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
                await PublishMessage(endpointInstance, new NewProgressAddedEvent {CommitmentsApprenticeshipId = 7887});
                break;
            case "x":
                await endpointInstance.Stop();
                return;
        }
    }
}
catch (Exception e)
{
    throw new Exception("Console failed", e);
}

async Task PublishMessage(IMessageSession messageSession, object message)
{
    await messageSession.Publish(message);

    Console.WriteLine("Message published.");
    Console.WriteLine("Press enter to continue");
    Console.ReadLine();
}