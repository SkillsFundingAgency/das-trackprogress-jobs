using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.TrackProgress.Jobs.Api;
using SFA.DAS.TrackProgress.Messages.Commands;

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
        _logger.LogInformation("Started processing KSBs {name} for course standard {id}", nameof(CacheKsbsCommand), message?.StandardUid);
        try
        {
            _logger.LogInformation("Processing KSBs for course standard {0}", message?.StandardUid);
            await _outerApi.PopulateKsbs(message.StandardUid);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when processing {name} for course standard {id}", nameof(CacheKsbsCommand), message?.StandardUid);
            throw;
        }
    }
}