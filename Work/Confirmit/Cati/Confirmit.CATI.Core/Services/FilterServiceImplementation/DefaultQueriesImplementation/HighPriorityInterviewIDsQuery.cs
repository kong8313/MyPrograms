using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.TimeService;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation.DefaultQueriesImplementation
{
    internal class HighPriorityInterviewIDsQuery : ScheduledInterviewIDsQuery
    {

        private readonly ITimeService _timeService; 

        public HighPriorityInterviewIDsQuery(int surveyId)
            : base(surveyId)
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
