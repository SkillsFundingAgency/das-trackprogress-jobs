using System.Threading.Tasks;
using RestEase;

namespace SFA.DAS.TrackProgress.Jobs.Api;
public interface ITrackProgressOuterApi
{
    [Post("apprenticeship/{id}/snapshot")]
    Task CreateSnapshot([Path]long id);
}