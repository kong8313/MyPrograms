using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.Survey.Quota;
using System.Collections.Generic;
using System.Data;
using System.Threading;


namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IInterviewQuotaCellService
    {
        void PopulateBatch(QuotaMatcher quotaMatcher, DataTable interviewsData);
        void Delete(int surveyId, List<int> interviewIds);
        void Populate(int surveyId, int quotaId);
        void Populate(int surveyId, CancellationToken cancellationToken);
    }
}
