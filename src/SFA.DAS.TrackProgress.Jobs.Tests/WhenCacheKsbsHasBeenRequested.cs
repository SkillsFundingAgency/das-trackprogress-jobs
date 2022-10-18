using NUnit.Framework;
using AutoFixture.NUnit3;
using Moq;
using NServiceBus.Testing;
using SFA.DAS.TrackProgress.Jobs.Api;
using SFA.DAS.TrackProgress.Jobs.Handlers;
using SFA.DAS.TrackProgress.Messages.Commands;

namespace SFA.DAS.TrackProgress.Jobs.Tests;

public class WhenCacheKsbsHasBeenRequested
{
    [Test, AutoMoqData]
    public async Task Then_notify_apim(
        [Frozen] Mock<ITrackProgressOuterApi> api,
        CacheKsbsCommandHandler sut,
        CacheKsbsCommand cmd)
    {
        await sut.Handle(cmd, new TestableMessageHandlerContext());

        api.Verify(m => m.PopulateKsbs(cmd.Standard, It.Is<PopulateKsbsRequest>(p=>p.KsbIds.Length == cmd.KsbIds.Length && p.KsbIds[0] == cmd.KsbIds[0])));
    }
}