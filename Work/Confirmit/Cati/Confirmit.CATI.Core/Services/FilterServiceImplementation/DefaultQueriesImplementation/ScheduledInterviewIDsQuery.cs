using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation.DefaultQueriesImplementation
{
    class ScheduledInterviewIDsQuery : BaseQuery
    {
        public ScheduledInterviewIDsQuery(int surveySID)
            : base(TableTypes.Call, surveySID)
        { }

        override protected string AddJoinOnAppointmentTable(TableTypes baseTable)
        {
            return FilterQueryUtility.GetDefaultJoinOnAppointmentTable();
        }

        override protected string AddJoinOnShiftTypeTable()
        {
            return FilterQueryUtility.GetDefaultJoinOnShiftTypeTable();
        }

        override protected string AddJoinOnResourceTable()
        {
            return FilterQueryUtility.GetDefaultJoinOnResourceTable();
        }

        protected override string AddJoinOnCFVariablesTable()
        {
            return FilterQueryUtility.GetDefaultJoinOnCFVariablesTable(ReplicatedTable);
        }

        protected override string AddJoinOnInterviewTable()
        {
            return FilterQueryUtility.GetDefaultJoinOnInterviewTable();
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

        protected override string MapNameToColumnName(string name, TableTypes baseTableType, out TableTypes tableType)
        {
            switch (name)
            {
                case "ID":
                case "InterviewID":
                    tableType = TableTypes.Call;
                    return "BvCall.InterviewID";
                case "Time":
                    tableType = TableTypes.Call;
                    return "BvCall.TimeInShift";
                case "Priority":
                    tableType = TableTypes.Call;
                    return "BvCall.Priority";
                case "Resource":
                    tableType = TableTypes.Resource;
                    return "BvViewPersonAndGroup.Name";
                case "InnerShiftTypeId":
                    tableType = TableTypes.InnerShiftType;
                    return "BvViewInnerShiftType.ShiftTypeId";
                case "ShiftType":
                    tableType = TableTypes.InnerShiftType;
                    return "BvViewInnerShiftType.ShiftTypeName";
                default:
                    return base.MapNameToColumnName(name, baseTableType, out tableType);
            }
        }

        protected override string GetSelectClause(TableTypes tableType)
        {
            return @"BvCall.InterviewID id";
        }

        protected override string GetFromClause(TableTypes tableType)
        {
            return @"FROM BvSvySchedule AS BvCall";
        }

        protected override string GetWhereClause(TableTypes tableType)
        {
            return "WHERE BvCall.SurveySID = " + m_SurveySid + @" AND BvCall.CallState > 0";
        }

        protected override string GetOrderByClause(TableTypes baseTableType)
        {
            if (orderByData == "ShiftType")
            {
                TableTypes tableType;
                var result = "ORDER BY " + MapNameToColumnName(orderByData, baseTableType, out tableType) + " " + sortingType.ToString();
                result += ", " + MapNameToColumnName("InnerShiftTypeId", baseTableType, out tableType);
                result += ", " + MapNameToColumnName("ID", baseTableType, out tableType);

                return result;
            }

            return base.GetOrderByClause(baseTableType);
        }

        protected override string AddJoinOnInnerShiftType()
        {
            return @"INNER JOIN [BvViewInnerShiftType] ON [BvViewInnerShiftType].[ShiftTypeId] = [BvCall].[ShiftTypeId]";
        }

        protected override string AddJoinOnPersonTable()
        {
            return @"LEFT JOIN BvPerson ON BvPerson.SID = BvInterview.LastCallPersonSID";
        }
    }
}