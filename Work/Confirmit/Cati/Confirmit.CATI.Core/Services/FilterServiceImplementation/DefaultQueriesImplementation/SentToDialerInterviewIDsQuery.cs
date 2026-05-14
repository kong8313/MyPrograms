using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation.DefaultQueriesImplementation
{
    internal class SentToDialerInterviewIDsQuery : ScheduledInterviewIDsQuery
    {
        public SentToDialerInterviewIDsQuery(int surveyId)
            : base(surveyId)
        {
        }

        protected override string GetWhereClause(TableTypes tableType)
        {
            return "WHERE BvCall.SurveySID = " + m_SurveySid + @" AND BvCall.CallState = -2";
        }
    }
}
