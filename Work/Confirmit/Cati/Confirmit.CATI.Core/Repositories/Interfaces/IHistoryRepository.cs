using System.Data;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IHistoryRepository
    {
        int Insert(BvHistoryEntity history);

        void Delete(int id);

        BvHistoryEntity GetById(int id);

        void Update(BvHistoryEntity entity);

        DataTable GetCallAttemptsForInterview(int surveyId, int interviewId);
    }
}
