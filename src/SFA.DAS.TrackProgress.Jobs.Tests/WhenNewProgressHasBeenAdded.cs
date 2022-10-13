using NUnit.Framework;
using AutoFixture.NUnit3;
using Moq;
using NServiceBus.Testing;
using SFA.DAS.TrackProgress.Jobs.Api;
using SFA.DAS.TrackProgress.Jobs.Handlers;
using SFA.DAS.TrackProgress.Messages.Events;

namespace SFA.DAS.TrackProgress.Jobs.Tests;

public class WhenNewProgressHasBeenAdded
    {
    [Test, AutoMoqData]
    public async Task Then_notify_apim(
        [Frozen] Mock<ITrackProgressOuterApi> api,
        NewProgressAddedEventHandler sut,
        NewProgressAddedEvent evt)
    {
        await sut.Handle(evt, new TestableMessageHandlerContext());

        api.Verify(m => m.CreateSnapshot(evt.CommitmentsApprenticeshipId));
    }
}