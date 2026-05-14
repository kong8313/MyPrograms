using System.Text;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.TimeService;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation.DefaultQueriesImplementation
{
    internal class HighPriorityCallsQuery : ScheduledCallsQuery
    {
        private readonly ITimeService _timeService;

        public HighPriorityCallsQuery(int surveyId, string replicationTable)
            : base(surveyId, replicationTable)
        {
            _timeService = ServiceLocator.Resolve<ITimeService>();
        }
        
        protected override string GetFromClause(TableTypes tableType)
        {
            return new StringBuilder("FROM [dbo].[GetHighPriorityCalls]( ").
                Append(m_SurveySid).
                Append(",'").
                Append(_timeService.GetUtcNow().ToString("yyyy-MM-dd HH:mm:ss")).
                Append("', 20) AS BvCall").ToString();
        }
     }
}
