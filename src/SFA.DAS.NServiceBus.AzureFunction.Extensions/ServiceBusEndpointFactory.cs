using Azure.Identity;
using Microsoft.Extensions.Configuration;
using NServiceBus;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions;

public static class ServiceBusEndpointFactory
{
    public static ServiceBusTriggeredEndpointConfiguration CreateSingleQueueConfiguration(string endpointAndQueueName, IConfiguration appConfiguration)
    {
        try
        {
            var configuration = new ServiceBusTriggeredEndpointConfiguration(
                endpointName: endpointAndQueueName,
                configuration: appConfiguration);

            configuration.AdvancedConfiguration.SendFailedMessagesTo($"{endpointAndQueueName}-error");
            configuration.AdvancedConfiguration.Pipeline.Register(new LogIncomingBehaviour(),
                nameof(LogIncomingBehaviour));
            configuration.AdvancedConfiguration.Pipeline.Register(new LogOutgoingBehaviour(),
                nameof(LogOutgoingBehaviour));

            configuration.Transport.ConnectionString(appConfiguration.GetValue<string>("AzureWebJobsServiceBus__fullyQualifiedNamespace"));
            configuration.Transport.CustomTokenCredential(new DefaultAzureCredential());
            configuration.AdvancedConfiguration.License(appConfiguration.GetValue<string>("NServiceBusLicense"));

            configuration.Transport.SubscriptionRuleNamingConvention(AzureRuleNameShortener.Shorten);

            return configuration;

        }
        catch (Exception e)
        {
            throw new Exception($"Create Single Queue Configuration {e.Message}", e);
        }

    }
}