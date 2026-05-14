using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation.DefaultQueriesImplementation
{
    class InterviewIDsQuery : BaseQuery
    {
        public InterviewIDsQuery(int surveySID)
            : base(new[] { TableTypes.CFVariables, TableTypes.Interview }, surveySID)
        { }

        override protected string AddJoinOnAppointmentTable(TableTypes baseTable)
        {
            if (baseTable == TableTypes.Interview)
                return @"LEFT JOIN BvAppointment ON BvInterview.SurveySID = BvAppointment.SurveySID
                               AND BvInterview.ID = BvAppointment.InterviewSID
                               AND BvAppointment.State = 1";

            return @"LEFT JOIN BvAppointment ON BvAppointment.SurveySID = " + m_SurveySid + @"
                            AND CFInterview.respid = BvAppointment.InterviewSID
                            AND BvAppointment.State = 1";
        }

        override protected string AddJoinOnCallTable(TableTypes baseTable)
        {
            if (baseTable == TableTypes.Interview)
                return @"LEFT JOIN BvSvySchedule AS BvCall ON BvInterview.SurveySID = BvCall.SurveySID
                               AND BvInterview.ID = BvCall.InterviewID";

            return @"LEFT JOIN BvSvySchedule AS BvCall ON BvCall.SurveySID = " + m_SurveySid + @"
                            AND CFInterview.respid = BvCall.InterviewID";
        }

        override protected string AddJoinOnShiftTypeTable()
        {
            throw new UserMessageException(FilterCanNotBeApplied);
        }

        override protected string AddJoinOnResourceTable()
        {
            throw new UserMessageException(FilterCanNotBeApplied);
        }

        protected override string AddJoinOnCFVariablesTable()
        {
            return string.Format(
                @"LEFT JOIN {0} CFinterview ON CFinterview.respid = BvInterview.[ID]",
                ReplicatedTable);
        }

        protected override string AddJoinOnInterviewTable()
        {
            return @"LEFT JOIN BvInterview ON CFinterview.respid = BvInterview.[ID] and BvInterview.surveysid = " + m_SurveySid;
        }

        protected override string AddJoinOnStateTable()
        {
            return @"INNER JOIN BvState ON BvState.StateID = BvInterview.TransientState AND BvState.StateGroupID = " +
                SurveyRepository.GetById(m_SurveySid).StateGroupID;
        }

        protected override string AddJoinOnTimezoneTable()
        {
            return @"LEFT JOIN BvTimezone ON BvTimezone.ID = BvInterview.TimezoneID";
        }     
        
        protected override string AddJoinOnPersonTable()
        {
            return @"LEFT JOIN BvPerson ON BvPerson.SID = BvInterview.LastCallPersonSID";
        }

        protected override string MapNameToColumnName(string name, TableTypes baseTableType, out TableTypes tableType)
        {
            switch (name)
            {
                case "InterviewID":
                case "ID":
                    tableType = 0;
                    return baseTableType == TableTypes.Interview ? "BvInterview.ID" : "CFInterview.respid";
                default:
                    return base.MapNameToColumnName(name, baseTableType, out tableType);
            }
        }

        protected override string GetSelectClause(TableTypes tableType)
        {
            switch (tableType)
            {
                case TableTypes.Interview:
                    return "BvInterview.ID id";
                case TableTypes.CFVariables:
                    return "CFInterview.respid id";
                default:
                    throw new ArgumentException();
            }
        }

        protected override string GetFromClause(TableTypes tableType)
        {
            switch (tableType)
            {
                case TableTypes.Interview:
                    return "from bvinterview";
                case TableTypes.CFVariables:
                    return "from " + ReplicatedTable + " CFInterview";
                default:
                    throw new ArgumentException();
            }
        }

        protected override string GetWhereClause(TableTypes tableType)
        {
            if (tableType == TableTypes.Interview)
                return @"WHERE BvInterview.SurveySID = " + m_SurveySid;

            return String.Empty;
        }
    }
}