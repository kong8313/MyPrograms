using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation.DefaultQueriesImplementation
{
    class InterviewsStatesQuery : BaseQuery
    {
        public InterviewsStatesQuery(int surveySID)
            : base(TableTypes.Interview, surveySID)
        { }

        override protected string AddJoinOnAppointmentTable(TableTypes baseTable)
        {
            return @"LEFT JOIN BvAppointment ON BvInterview.SurveySID = BvAppointment.SurveySID
                           AND BvInterview.ID = BvAppointment.InterviewSID
                           AND BvAppointment.State = 1";
        }

        override protected string AddJoinOnCallTable(TableTypes baseTable)
        {
            return @"LEFT JOIN BvSvySchedule AS BvCall ON BvCall.InterviewID = BvInterview.ID AND 
                                     BvCall.SurveySID = BvInterview.SurveySID";
        }

        override protected string AddJoinOnShiftTypeTable()
        {
            return FilterQueryUtility.GetDefaultJoinOnShiftTypeTable();
        }

        override protected string AddJoinOnResourceTable()
        {
            return FilterQueryUtility.GetDefaultJoinOnResourceTable();
        }

        protected override string GetSelectClause(TableTypes tableType)
        {
            return "BvInterview.[ID], BvInterview.[TransientState], BvState.[Name] as StateName";
        }

        protected override string GetFromClause(TableTypes tableType)
        {
            return string.Format( @"FROM BvInterview
                  LEFT JOIN BvState ON BvState.StateID = BvInterview.TransientState 
                               AND BvState.StateGroupID = {0}", SurveyRepository.GetById(m_SurveySid).StateGroupID);
        }

        protected override string GetWhereClause(TableTypes tableType)
        {
            return @"WHERE BvInterview.SurveySID = " + m_SurveySid;
        }

        protected override string AddJoinOnCFVariablesTable()
        {
            return string.Format(
                @"LEFT JOIN {0} CFinterview ON CFinterview.respid = BvInterview.ID",
                ReplicatedTable);
        }
    }
}
