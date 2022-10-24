using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.TrackProgress.Jobs.Api;
using SFA.DAS.TrackProgress.Messages.Events;

namespace SFA.DAS.TrackProgress.Jobs.Handlers;

public class NewProgressAddedEventHandler : IHandleMessages<NewProgressAddedEvent>
{
    private readonly ITrackProgressOuterApi _outerApi;
    private readonly ILogger<NewProgressAddedEventHandler> _logger;

    public NewProgressAddedEventHandler(ITrackProgressOuterApi outerApi, ILogger<NewProgressAddedEventHandler> logger)
    {
        _outerApi = outerApi;
        _logger = logger;
    }

    public async Task Handle(NewProgressAddedEvent message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Started processing {name} for Commitment ApprenticeshipId {id}", nameof(NewProgressAddedEvent), message?.CommitmentsApprenticeshipId);
        try
        {
            _logger.LogInformation("Processing Message {0}", message?.CommitmentsApprenticeshipId);
            await _outerApi.CreateSnapshot(message.CommitmentsApprenticeshipId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when processing {name} for Commitment ApprenticeshipId {id}", nameof(NewProgressAddedEvent), message?.CommitmentsApprenticeshipId);
            throw;
        }
    }
}