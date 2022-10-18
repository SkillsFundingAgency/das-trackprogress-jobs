﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.TrackProgress.Jobs.Api;
using SFA.DAS.TrackProgress.Messages.Commands;
using SFA.DAS.TrackProgress.Messages.Events;

namespace SFA.DAS.TrackProgress.Jobs.Handlers;

public class CacheKsbsCommandHandler : IHandleMessages<CacheKsbsCommand>
{
    private readonly ITrackProgressOuterApi _outerApi;
    private readonly ILogger<CacheKsbsCommandHandler> _logger;

    public CacheKsbsCommandHandler(ITrackProgressOuterApi outerApi, ILogger<CacheKsbsCommandHandler> logger)
    {
        _outerApi = outerApi;
        _logger = logger;
    }

    public async Task Handle(CacheKsbsCommand message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Started processing KSBs {name} for course standard {id}", nameof(NewProgressAddedEvent), message?.Standard);
        try
        {
            _logger.LogInformation("Processing KSBs for course standard {0}", message?.Standard);
            await _outerApi.PopulateKsbs(message.Standard, new PopulateKsbsRequest(message.KsbIds));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when processing {name} for course standard {id}", nameof(NewProgressAddedEvent), message?.Standard);
            throw;
        }
    }
}