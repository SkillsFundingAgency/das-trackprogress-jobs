using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using SFA.DAS.TrackProgress.Jobs.Api;
using SFA.DAS.TrackProgress.Jobs.Infrastructure;

namespace SFA.DAS.TrackProgress.Jobs.Handlers;

public class NewProgressAddedEventHandler
{
    private readonly ITrackProgressOuterApi _outerApi;
    private readonly ILogger<NewProgressAddedEventHandler> _logger;

    public NewProgressAddedEventHandler(ITrackProgressOuterApi outerApi, ILogger<NewProgressAddedEventHandler> logger)
    {
        _outerApi = outerApi;
        _logger = logger;
    }

    [FunctionName("HandleNewProgressAddedEvent")]
    public async Task HandleCommand([NServiceBusTrigger(Endpoint = QueueNames.NewProgressAdded)] NewProgressAddedEvent @event)
    {
        _logger.LogInformation("Started processing {name} for Commitment ApprenticeshipId {id}", nameof(NewProgressAddedEvent), @event?.CommitmentsApprenticeId);
        try
        {
            await _outerApi.CreateSnapshot(@event.CommitmentsApprenticeId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when processing {name} for Commitment ApprenticeshipId {id}", nameof(NewProgressAddedEvent), @event?.CommitmentsApprenticeId);
            throw;
        }
    }
}

