using System;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.TrackProgress.Jobs.Infrastructure;
public static class ConfigurationExtensions
{
    public static bool IsAcceptanceTest(this IConfiguration config)
    {
        return config["EnvironmentName"].Equals("ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool IsLocalAcceptanceOrDev(this IConfiguration config)
    {
        return config["EnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
               config["EnvironmentName"].Equals("ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase) ||
               config["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
    }

    public static string NServiceBusConnectionString(this IConfiguration config) => config["NServiceBusConnectionString"] ?? "UseLearningEndpoint=true";

    public static string NServiceBusLicense(this IConfiguration config) => config["NServiceBusLicense"];
}