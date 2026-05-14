using System;
using System.Text;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation
{
    abstract internal class BaseQuery
    {
        protected const string FilterCanNotBeApplied = "Filter can not be applied. It references the variables that are not available in the selected state.";
        private string paging = String.Empty;
        protected string orderByData = String.Empty;
        protected SortingType sortingType = SortingType.Asc;
        private SqlFilter filter;
        private string selectClause;
        private ReplicatedColumn[] selectColumns;
        
        internal string SelectClause
        {
            set
            {
                selectClause = value;
            }
        }

        private TableTypes m_UsedTables = 0;
        private TableTypes[] m_BasedTables;
        protected int m_SurveySid;

   
        protected BaseQuery(TableTypes basedTables, int surveySid)
        {
            m_BasedTables = new[]{basedTables};
            m_SurveySid = surveySid;
        }

        protected BaseQuery(TableTypes[] basedTables, int surveySid)
        {
            m_BasedTables = basedTables;
            m_SurveySid = surveySid;
        }

        public override string ToString()
        {
            var baseTableType = ChooseBase();
            var joinClause = CreateJoin(baseTableType);

            return new StringBuilder("SELECT ").
                Append(GetFullSelectClause(baseTableType)).
                Append(Environment.NewLine).
                Append(GetFromClause(baseTableType)).
                Append(Environment.NewLine).
                Append(joinClause).
                Append(GetFullWhereClause(baseTableType)).
                Append(Environment.NewLine).
                Append(GetOrderByClause(baseTableType)).
                Append(Environment.NewLine).
                Append(paging).ToString();
        }

        public static implicit operator string(BaseQuery query)
        {
            return query.ToString();
        }

        public void AddPaging(int startIndex, int objectsCount)
        {
            paging = "OFFSET " + (startIndex - 1) + " ROWS FETCH NEXT " + objectsCount + " ROWS ONLY;";
        }

        protected StringBuilder GenerateStringForAditionalSelectParameters(ReplicatedColumn[] selectColumns)
        {
            var selectClause = new StringBuilder();
            bool first = true;
            foreach (var column in selectColumns)
            {
                if (!first)
                {
                    selectClause.Append(", ");
                }
                else
                {
                    first = false;
                }

                // we need to put Confirmit valiables columns in brackets to avoid problems with reserved sql words
                // we need to double quotas symbols in the names to avoid sql-injections
                string selectDataInBrackets = String.Format("[{0}]", DoubleQuotas(column.Name));
                string selectAliasInBrackets = String.Format("[{0}]", DoubleQuotas(column.Alias));

                //all confirmit variables are in CFInterview table
                selectClause.Append("CFInterview.").Append(selectDataInBrackets)
                            .Append(" AS ").Append((column.Alias == String.Empty ? selectDataInBrackets : selectAliasInBrackets));
            }

            return selectClause;
        }

        /// <summary>
        /// Add some confirmit parameters for output with aliases
        /// </summary>
        /// <param name="selectColumns">shouldn't be null. array columns for output</param>
        virtual public void AddSelectParameter(ReplicatedColumn[] selectColumns)
        {
            if (selectColumns == null)
                throw new ArgumentNullException("selectColumns");

            this.selectColumns = selectColumns;
        }

        /// <summary>
        /// Doubles every quota occurrence in the input string
        /// </summary>
        /// <param name="inputString">String to process</param>
        /// <returns>String with doubled quotas</returns>
        private string DoubleQuotas(string inputString)
        {
            return inputString.Replace("'", "''");
        }

        private TableTypes ChooseBase()
        {
            var baseTable = m_BasedTables[0];
            foreach (var table in m_BasedTables)
            {
                if ((table & m_UsedTables) == table)
                {
                    baseTable = table;
                    break;
                }
            }

            return baseTable;
        }

        private string CreateJoin(TableTypes baseTable)
        {
            var result = String.Empty;
            var necessaryFusionTables = m_UsedTables & (~baseTable);
            if ((necessaryFusionTables & TableTypes.Appointment) == TableTypes.Appointment)
            {
                result += AddJoinOnAppointmentTable(baseTable) + Environment.NewLine;
            }
            if ((necessaryFusionTables & TableTypes.Call) == TableTypes.Call)
            {
                result += AddJoinOnCallTable(baseTable) + Environment.NewLine;
            }
            if ((necessaryFusionTables & TableTypes.ShiftType) == TableTypes.ShiftType)
            {
                result += AddJoinOnShiftTypeTable() + Environment.NewLine;
            }

            if ((necessaryFusionTables & TableTypes.Resource) == TableTypes.Resource)
            {
                result += AddJoinOnResourceTable() + Environment.NewLine;
            }

            if ((necessaryFusionTables & TableTypes.CFVariables) == TableTypes.CFVariables)
            {
                result += AddJoinOnCFVariablesTable() + Environment.NewLine;
            }

            if ((necessaryFusionTables & TableTypes.Interview) == TableTypes.Interview)
            {
                result += AddJoinOnInterviewTable() + Environment.NewLine;
            }

            if (((necessaryFusionTables|TableTypes.Interview) & TableTypes.State) == TableTypes.State)
            {
                result += AddJoinOnStateTable() + Environment.NewLine;
            }

            if (((necessaryFusionTables | TableTypes.Interview) & TableTypes.Timezone) == TableTypes.Timezone)
            {
                result += AddJoinOnTimezoneTable() + Environment.NewLine;
            }

            if ((necessaryFusionTables & TableTypes.InnerShiftType) == TableTypes.InnerShiftType)
            {
                result += AddJoinOnInnerShiftType() + Environment.NewLine;
            }

            if ((necessaryFusionTables & TableTypes.Person) == TableTypes.Person)
            {
                result += AddJoinOnPersonTable() + Environment.NewLine;
            }

            result += AddActiveShiftTypeZoneTable() + Environment.NewLine;

            return result;
        }

        /// <summary>
        /// Method check if all necessary joins is existed.
        /// And add missing joins
        /// </summary>
        /// <param name="necessaryFusionTables">used in query tables</param>
        public virtual void AddMissingJoin(TableTypes necessaryFusionTables)
        {
            const TableTypes suportedTables = TableTypes.Appointment |
                                              TableTypes.Call |
                                              TableTypes.Interview |
                                              TableTypes.ShiftType |
                                              TableTypes.Resource |
                                              TableTypes.CFVariables |
                                              TableTypes.Expression |
                                              TableTypes.State |
                                              TableTypes.Timezone |
                                              TableTypes.InnerShiftType |
                                              TableTypes.Person;

            if (((~suportedTables) & necessaryFusionTables) != 0)
            {
                throw new IndexOutOfRangeException(String.Format(
                                                       "At this time only next table types are supported: {0}. But next types are used: {1}",
                                                       suportedTables,
                                                       necessaryFusionTables));
            }

            if ((necessaryFusionTables & TableTypes.Resource) != 0)
            {
                necessaryFusionTables |= TableTypes.Call;
            }
            
            if ((necessaryFusionTables & TableTypes.Person) != 0)
            {
                necessaryFusionTables |= TableTypes.Interview;
            }

            m_UsedTables |= necessaryFusionTables;
        }

        protected virtual string AddJoinOnAppointmentTable(TableTypes baseTable){return String.Empty;}
        protected virtual string AddJoinOnCallTable(TableTypes baseTable) { return String.Empty; }
        protected virtual string AddJoinOnShiftTypeTable() { return String.Empty; }
        protected virtual string AddJoinOnResourceTable() { return String.Empty; }
        protected virtual string AddJoinOnCFVariablesTable() { return String.Empty; }
        protected virtual string AddJoinOnInterviewTable() { return String.Empty; }
        protected virtual string AddJoinOnStateTable() { return String.Empty; }
        protected virtual string AddJoinOnTimezoneTable() { return String.Empty; }
        protected virtual string AddJoinOnInnerShiftType() { return String.Empty; }
        protected virtual string AddJoinOnPersonTable() { return String.Empty; }
        protected virtual string AddActiveShiftTypeZoneTable() { return String.Empty; }
        protected abstract string GetSelectClause(TableTypes tableType);
        protected abstract string GetFromClause(TableTypes tableType);
        protected abstract string GetWhereClause(TableTypes tableType);

        protected virtual string MapNameToColumnName(string name, TableTypes baseTableType, out TableTypes tableType)
        {
            if (ConfirmitVariablesHelper.IsComfirmitVariableAlias(name))
            {
                tableType = TableTypes.CFVariables;
                return String.Format( "CFInterview.[{0}]", ConfirmitVariablesHelper.ExtractNameFromConfirmitVariableAlias(name));
            }

            switch (name)
            {
                case "ID":
                    tableType = TableTypes.Interview;
                    return "BvInterview.ID";
                case "AttemptNumber":
                    tableType = TableTypes.CFVariables;
                    return "CFinterview.CallAttemptCount";
                case "StateName":
                    tableType = TableTypes.Interview | TableTypes.State;
                    return "BvState.Name";
                case "RespondentName":
                    tableType = TableTypes.Interview;
                    return "BvInterview.RespondentName";     
                case "ReviewStatus":
                    tableType = TableTypes.Interview;
                    return "BvInterview.ReviewStatus";
                case "DialTypeId":
                    tableType = TableTypes.Interview;
                    return "BvInterview.DialTypeId";
                case "TelephoneNumber":
                    tableType = TableTypes.Interview;
                    return "BvInterview.TelephoneNumber";
                case "TimezoneName":
                    tableType = TableTypes.Timezone;
                    return "BvTimezone.Name";
                case "ExpireTime":
                    tableType = TableTypes.Call;
                    return "BvCall.ExpireTime";
                case "LastCallTime":
                    tableType = TableTypes.Interview;
                    return "BvInterview.LastCallTime";
                case "DialingMode":
                    tableType = TableTypes.Interview;
                    return "BvInterview.DialingMode";
                case "ExpTime":
                    tableType = TableTypes.Appointment;
                    return "BvAppointment.ExpTime";
                case "ApptTime":
                    tableType = TableTypes.Appointment;
                    return "BvAppointment.Time";
                case "CallState":
                    tableType = TableTypes.Call;
                    return "BvCall.CallState";
                case "ShiftType":
                    tableType = TableTypes.ShiftType;
                    return "ShiftType";          
                case "LastInterviewerName":
                    tableType = TableTypes.Person;
                    return "Name";
                case "Time":
                    tableType = TableTypes.Call;
                    return "BvCall.TimeInShift";

                default:
                    tableType = TableTypes.CFVariables;
                    return String.Format( "CFInterview.[{0}]", name);
            }
        }

        private string GetFullSelectClause(TableTypes tableType)
        {
            if (!String.IsNullOrEmpty(selectClause))
                return selectClause;

            var result = GetSelectClause(tableType);
            var additionalSelect = String.Empty;
            
            if (selectColumns != null)
            {
                additionalSelect = GenerateStringForAditionalSelectParameters(selectColumns).ToString();
            }

            if ((result.Length > 0) &&
                (additionalSelect.Length > 0))
            {
                result += ", ";
            }

            return result + additionalSelect;
        }

        private string GetFullWhereClause(TableTypes tableType)
        {
            var result = GetWhereClause(tableType);
            string additionalWhereZone = filter == null ? String.Empty : filter.ToString();

            if (String.IsNullOrEmpty(additionalWhereZone))
            {
                return result;
            }

            if (result.Length == 0)
            {
                result += "WHERE ";
            }
            else
            {
                result += " AND ";
            }

            return result + additionalWhereZone;
        }

        public void AddWhereParameter(SqlFilter filter)
        {
            this.filter = filter;
        }
        
        protected virtual string GetOrderByClause(TableTypes baseTableType)
        {
            if (orderByData == String.Empty)
                return String.Empty;

            return GetOrderBy(baseTableType);
        }

        private string GetOrderBy(TableTypes baseTableType)
        {
            TableTypes tableType;
            var result = "ORDER BY " + MapNameToColumnName(orderByData, baseTableType, out tableType) + " " +
                         sortingType.ToString();

            if (orderByData != "ID" && orderByData != "InterviewID")
                result += ", " + MapNameToColumnName("ID", baseTableType, out tableType);

            return result;
        }

        public void AddOrderByParameter(SortingArgs sortingArgs)
        {
            if (sortingArgs.PropertyName == null)
                throw new ArgumentNullException("orderByData");

            TableTypes tableType;
            MapNameToColumnName(sortingArgs.PropertyName, 0, out tableType);

            this.orderByData = sortingArgs.PropertyName;
            this.sortingType = sortingArgs.IsAscending ? SortingType.Asc : SortingType.Desc;

            AddMissingJoin(tableType);
        }

        protected string ReplicatedTable
        {
            get
            {
                return ReplicationSchemaService.GetDestinationTableName(m_SurveySid);
            }
        }
    }
}