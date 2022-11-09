using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace SFA.DAS.TrackProgress.Jobs.Api;
public interface ITrackProgressOuterApi
{
    [Post("apprenticeships/{id}/snapshot")]
    Task CreateSnapshot([Path]long id);

    [Post("courses/{standard}/ksbs")]
    Task PopulateKsbs([Path] string standard);
}