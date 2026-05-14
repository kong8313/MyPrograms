using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation.DefaultQueriesImplementation
{
    internal class SentToDialerCallsQuery : ScheduledCallsQuery
    {
        public SentToDialerCallsQuery(int surveyId, string replicationTable)
            : base(surveyId, replicationTable)
        { }

        protected override string GetWhereClause(TableTypes tableType)
        {
            return "WHERE BvCall.SurveySID = " + m_SurveySid + @" AND BvCall.CallState = -2";
        }
    }
}
