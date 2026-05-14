using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation.DefaultQueriesImplementation
{
    class SuspendedInterviewIDsQuery : InterviewIDsQuery
    {
        public SuspendedInterviewIDsQuery(int surveySID)
            : base(surveySID)
        { }

        override protected string AddJoinOnAppointmentTable(TableTypes tableType)
        {
            throw new UserMessageException(FilterCanNotBeApplied);
        }

        protected override string GetFromClause(TableTypes tableType)
        {
            var result = base.GetFromClause(tableType);

            if (tableType == TableTypes.Interview)
                result += @" LEFT JOIN BvSvySchedule AS BvCall ON BvInterview.SurveySID = BvCall.SurveySID  AND BvInterview.[ID] = BvCall.InterviewID";
            else
                result += @" LEFT JOIN BvSvySchedule AS BvCall ON CFInterview.respid = BvCall.InterviewID AND BvCall.SurveySID = " + m_SurveySid;

            return result;
        }

        protected override string GetWhereClause(TableTypes tableType)
        {
            var result = base.GetWhereClause(tableType);

            if(string.IsNullOrEmpty(result))
                result = @"WHERE BvCall.[InterviewID] IS NULL ";
            else
                result += @"AND BvCall.[InterviewID] IS NULL ";

            if (tableType == TableTypes.Interview)
                result += @"AND BvInterview.SurveySID = " + m_SurveySid;

            return result;
        }

        protected override string AddJoinOnCallTable(TableTypes baseTable) 
        {
            throw new UserMessageException(FilterCanNotBeApplied);
        }
    }
}