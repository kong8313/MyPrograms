using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IResponseReviewerApiClient
    {
        Task<SessionModel> AddSession(SessionModel sessionModel);
    }
}