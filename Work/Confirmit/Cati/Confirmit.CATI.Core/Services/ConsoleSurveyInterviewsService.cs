using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories;
using System.Text.RegularExpressions;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Common.Security;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using Microsoft.Practices.ObjectBuilder2;

namespace Confirmit.CATI.Core.Services
{
    /// <summary>
    /// Service is responsible for retrieving interviews for certain person and certain survey
    /// Used by ConsoleService when interviewer has 'Manual' mode.
    /// </summary>
    public static class ConsoleSurveyInterviewsService
    {
        private const string VariableAliasPrefix = "var_"; 

        /// <summary>
        /// Retuns DataTable with interviews 
        /// </summary>
        ///<param name="surveyId">Survey identifier</param>
        ///<param name="personId">Person identifier</param>
        ///<param name="parameters">Search parameter array</param>
        ///<param name="assignmentListMode"> </param>
        ///<remarks>
        /// Columns availability can be changed in any time. 
        /// All search parameters that refer to unavailable columns on the current time are missed.
        /// </remarks>
        public static DataTable GetSurveyInterviews(
            int surveyId, int personId, SearchParameter[] parameters, PersonAssignmentListMode assignmentListMode = PersonAssignmentListMode.AssignedCallsOnly)
        {
            var availableColumnsNames = new OrderedSearchableFieldsRepository().GetBySurveyId(surveyId)
                    .Where(x => x.IsEnabled)
                    .Select(x => x.FieldName);

            var validParameters = parameters.Where(x => 
                                  SearchableFieldsRepository.CheckPredefinedColumnName(x.ColumnName) ||
                                  availableColumnsNames.Contains(ExtractColumnNameFromNameWithAlias(x.ColumnName))
                                                  );
                        
            string query = GetSqlSearchCondition(validParameters);

            var result = GetSurveyInterviewsTable(surveyId, personId, assignmentListMode, query);

            if (result.Columns.Count > 0)
            {
                result = AddRespondentTimeColumn(result);
            }            

            foreach (DataColumn c in result.Columns)
            {                
                if (IsStartedWithAlias(c.ColumnName))
                {
                    c.Caption = ExtractColumnNameFromNameWithAlias(c.ColumnName);
                }             
            }

            return result;
        }

        private static DataTable AddRespondentTimeColumn(DataTable table)
        {
            if (!table.Columns.Contains("TimeToCall"))
            {
                table.Columns.Remove("TimezoneID");
                return table;
            }

            table.Columns.Add("RespondentTime", typeof(DateTime));
            
            foreach (DataRow row in table.Rows)
            {
                var timeZoneId = ConvertToInt(row["TimezoneID"]);

                var timeToCall = (DateTime)row["TimeToCall"];
                /* DateTime should be converted to server timezone here
                 * Since on the client side it will be automatically converted from server to client timezone
                 * See: http://support.microsoft.com/kb/842545 */
                row["TimeToCall"] = TimeZoneInfo.ConvertTimeFromUtc(timeToCall, TimeZoneInfo.Local);

                var respondentTime = GetRespondentTime(timeZoneId, timeToCall);
                row["RespondentTime"] = TimeZoneInfo.ConvertTimeFromUtc(respondentTime, TimeZoneInfo.Local);
            }

            table.Columns.Remove("TimezoneID");
            return table;
        }

        private static int? ConvertToInt(object cellValue)
        {
            int? timezoneId = null;
            if (cellValue != DBNull.Value)
            {
                timezoneId = (int) cellValue;
            }

            return timezoneId;
        }

        private static DateTime GetRespondentTime(int? timeZoneId, DateTime timeToCall)
        {
            if (timeToCall == CallQueueService.DefaultTimeInShift)
            {
                return CallQueueService.DefaultTimeInShift;
            }

            var timezoneService = ServiceLocator.Resolve<ITimezoneService>();
            var timezoneId = timezoneService.GetTimezoneIdOrDefaultCallCenterTimezoneId(timeZoneId);

            return timezoneService.ConvertTimeFromUtc(timezoneId, timeToCall);
        }
        
        /// <summary>
        /// Calls BvSpGetSurveyInterviewsAdapter to retrieve data table 
        /// </summary>        
        private static DataTable GetSurveyInterviewsTable(int surveyId, int personId, PersonAssignmentListMode assigmentListMode, string query)
        {
            DataTable result = new DataTable("SurveyInterviews");

            if (String.IsNullOrEmpty(query))
            {
                query = null;
            }

            using (IDataReader reader = BvSpGetSurveyInterviewsAdapter.ExecuteReader(
                    surveyId,
                    personId,
                    (int)assigmentListMode,
                    VariableAliasPrefix,
                    query,
                    ServiceLocator.Resolve<ISystemSettings>().Console.InterviewsCountShownInManualMode)
                  )
            {
                result.Load(reader);
            }

            return result;
        }

        /// <summary>
        /// Generates 'where' conditions  using search parameters
        /// </summary>
        /// <param name="parameters">list of SearchParameters</param>
        /// <returns>String that represent 'where' condition for sql request</returns>
        private static string GetSqlSearchCondition(IEnumerable<SearchParameter> parameters)
        {
            var conditions = new List<string>();

            foreach (var p in parameters)
            {
                var value = p.Value;
                
                if (String.IsNullOrEmpty(value) == false)
                {
                    value = value.Trim();  
                  
                    DataValidationManager.CheckForSqlInjection(value);

                    if (Type.GetType(p.ColumnTypeName) == typeof(string))
                    {
                        conditions.Add(String.Format("{0} LIKE '%{1}%'", p.ColumnName, value));
                    }
                    else
                    {
                        conditions.Add(String.Format("{0} = {1}", p.ColumnName, value));
                    }
                }
            }

            return string.Join(" AND ", conditions.ToArray());
        }

        /// <summary>
        /// Checks does name starts with alias or not
        /// </summary>
        /// <param name="columnName">Name of column</param>        
        private static bool IsStartedWithAlias(string columnName)
        {
            return columnName.StartsWith(VariableAliasPrefix);
        }

        /// <summary>
        /// Extracts confirmit variable name from confirmit variable alias.
        /// </summary>
        /// <param name="columnNameWithAlias">Column name with alias</param>
        /// <returns>Clear Column name</returns>
        private static string ExtractColumnNameFromNameWithAlias(string columnNameWithAlias)
        {
            return Regex.Replace(columnNameWithAlias, "^" + VariableAliasPrefix, String.Empty);
        } 
    }
}
