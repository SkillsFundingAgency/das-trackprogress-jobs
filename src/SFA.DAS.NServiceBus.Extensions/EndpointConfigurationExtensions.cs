using Newtonsoft.Json;
using NServiceBus;

namespace SFA.DAS.NServiceBus.Extensions;

public static class EndpointConfigurationExtensions
{
    public static EndpointConfiguration UseNewtonsoftJsonSerializer(
        this EndpointConfiguration config)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None
        };

        var serialization = config.UseSerialization<NewtonsoftJsonSerializer>();
        serialization.Settings(settings);
        
        return config;
    }

    public static EndpointConfiguration UseMessageConventions(this EndpointConfiguration endpointConfiguration)
    {
        var conventions = endpointConfiguration.Conventions(); 
        conventions.DefiningMessagesAs(IsMessage); 
        conventions.DefiningMessagesAs(IsEvent);
        conventions.DefiningMessagesAs(IsCommand);

        return endpointConfiguration;
    }

    private static bool IsMessage(Type t) => t is IMessage || IsSfaMessage(t, "Messages");

    private static bool IsEvent(Type t) => t is IEvent || IsSfaMessage(t, "Messages.Events");

    private static bool IsCommand(Type t) => t is ICommand || IsSfaMessage(t, "Messages.Commands");

    private static bool IsSfaMessage(Type t, string namespaceSuffix)
    {
        if (t.Namespace != null && t.Namespace.EndsWith(namespaceSuffix))
        {
            var x = t.FullName;
        }

        return t.Namespace != null &&
               t.Namespace.StartsWith("SFA.DAS") &&
               t.Namespace.EndsWith(namespaceSuffix);
    }
}