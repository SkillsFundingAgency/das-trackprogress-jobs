using SFA.DAS.Http.Configuration;

namespace SFA.DAS.TrackProgress.Jobs.Infrastructure;

public class ApplicationSettings
{
    public TrackProgressApiOptions TrackProgressInternalApi { get; set; } = null!;
}

public class TrackProgressApiOptions : IApimClientConfiguration
{
    public const string TrackProgressInternalApi = "TrackProgressInternalApi";
    public string ApiBaseUrl { get; set; } = null!;
    public string SubscriptionKey { get; set; } = null!;
    public string ApiVersion { get; set; } = null!;
}

