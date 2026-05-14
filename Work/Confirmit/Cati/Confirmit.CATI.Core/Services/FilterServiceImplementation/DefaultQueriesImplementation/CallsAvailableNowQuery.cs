using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation.DefaultQueriesImplementation
{
    internal class CallsAvailableNowQuery : ScheduledCallsQuery
    {
        public CallsAvailableNowQuery(int surveySID, string replicationTable) : base(surveySID, replicationTable)
        {
        }
        
        protected override string AddActiveShiftTypeZoneTable()
        {
            return "INNER JOIN BvActiveShiftTypeZone BvShift ON BvShift.Id = BvCall.ShiftTypeID AND BvShift.SurveyId = BvCall.SurveySID";
        }
        
        protected override string GetWhereClause(TableTypes tableType)
        {
            return $@"WHERE BvCall.SurveySID = {m_SurveySid} AND BvCall.CallState = {(int)CallState.Scheduled} AND BvCall.TimeInShift <= GETUTCDATE()";
        }
    }
}