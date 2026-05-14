using System.Collections.Generic;
using System.Data;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation
{
    public interface IReplicatedDataRepository
    {
        IDataReader ExecuteReplicatedDataReader(int surveyId);
        DataTable GetInterviewsData(int surveyId, List<int> interviewsIds);
        IDictionary<string, string> GetReplicationValues(int surveyId, int interviewId);
    }
}
