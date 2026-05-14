using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Adapter.TableType;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.TableType;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.LinkedInterviews;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Services.Database.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Microsoft.Practices.ObjectBuilder2;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.Telephony.LinkedSurveys
{
    public class LinkedInterviewProvider : ILinkedInterviewProvider
    {
        private readonly IDatabaseExpressionService _databaseExpressionService;
        private readonly ISurveyMetadataCacheService _surveyMetadataCacheService;
        private readonly IDatabaseEngineFactory _databaseEngineFactory;
        private readonly IPersonRepository _personRepository;
        private readonly IConsoleSettingsGroup _consoleSettings;

        public LinkedInterviewProvider(
            IDatabaseExpressionService databaseExpressionService,
            ISurveyMetadataCacheService surveyMetadataCacheService,
            IDatabaseEngineFactory databaseEngineFactory, IPersonRepository personRepository,
            IConsoleSettingsGroup consoleSettings)
        {
            _databaseExpressionService = databaseExpressionService;
            _surveyMetadataCacheService = surveyMetadataCacheService;
            _databaseEngineFactory = databaseEngineFactory;
            _personRepository = personRepository;
            _consoleSettings = consoleSettings;
        }

        public List<CatiInterview> Find(int interviewerId, string[] projectIds, string telephoneNumber, string respondentName, string filter)
        {
            var parsedFilter = _databaseExpressionService.Parse(filter);

            var surveys = GetSurveys(interviewerId, projectIds);

            if (surveys.Count == 0)
            {
                return new List<CatiInterview>();
            }

            var parameters = new List<SqlParameter>();
            var query = string.IsNullOrEmpty(filter) 
                ? GenerateQueryWithoutFilter(interviewerId, telephoneNumber, respondentName, surveys, parameters) 
                : GenerateQuery(interviewerId, telephoneNumber, respondentName, parsedFilter, surveys, parameters);

            var result = new List<CatiInterview>();
            var databaseEngine = _databaseEngineFactory.CreateForCurrentInstanceDatabase();
            using (var reader = databaseEngine.ExecuteReaderInNewConnection(query, CommandType.Text, parameters.ToArray()))
            {
                while(reader.Read())
                {
                    result.Add(new CatiInterview
                    {
                        ProjectId = (string)reader["ProjectId"],
                        RespondentId = (int)reader["InterviewId"],
                        RespondentName = ConvertDbValue<string>(reader["RespondentName"]),
                        TelephoneNumber = ConvertDbValue<string>(reader["TelephoneNumber"]),
                        Filters = parsedFilter.Select(e => string.Format("{0}={1}", e.ColumnName, reader[e.ColumnName])).JoinInString(",")
                    });
                }
            }
            return result;
        }

        public List<CatiInterview> GetLinkedInterviews(string linkedChain)
        {
            var interviewsList = new List<CatiInterview>();

            if (!string.IsNullOrWhiteSpace(linkedChain))
            {
                var chain = JsonConvert.DeserializeObject<List<LinkedChainItem>>(linkedChain);

                if (chain != null)
                {
                    var interviews = BvInterviewTypeOrderedAdapter.CreateTable(chain.Select(x =>
                        new BvInterviewTypeOrderedEntity()
                            {OrderId = x.Id, IID = x.InterviewId, SurveySid = x.SurveyId}));
                    interviewsList = BvSpGetInterviewsAdapter.ExecuteEntityList(interviews)
                        .Select(x => new CatiInterview
                        {
                            ProjectId = x.ProjectId, RespondentId = (int) x.InterviewId,
                            RespondentName = x.RespondentName, TelephoneNumber = x.TelephoneNumber, Filters = null
                        }).ToList();
                }
            }

            return interviewsList;
        }


        private T ConvertDbValue<T>(object dbValue) 
        {
            if (dbValue is DBNull)
                return default(T);
            return (T) dbValue;
        }
        
        private string GenerateQueryWithoutFilter(int interviewerId, string telephoneNumber, string respondentName, 
            List<BvSurveyEntity> surveys, List<SqlParameter> parameters)
        {
            var topCount = Math.Min(100, _consoleSettings.LinkedInterviewsLimit * surveys.Count).ToString();
            var surveyQueryTemplate = GenerateSurveyQueryTemplate(interviewerId, telephoneNumber, respondentName,
                null, parameters);
            
            var surveyIds = surveys.Select(x => x.SID);
            parameters.Add(BvIntArrayTypeAdapter.CreateSqlParameter("@SurveyIds", surveyIds));
            return surveyQueryTemplate.Replace("#TopCount#", topCount);
        }
        
        private string GenerateQuery(int interviewerId, string telephoneNumber, string respondentName, List<DatabaseExpression> parsedFilter,
            List<BvSurveyEntity> surveys, List<SqlParameter> parameters)
        {
            var topCount = _consoleSettings.LinkedInterviewsLimit.ToString();
            var surveyQueryTemplate = GenerateSurveyQueryTemplate(interviewerId, telephoneNumber, respondentName,
                parsedFilter, parameters);

            var query = surveys.Select(survey => (surveyQueryTemplate + GenerateFilterColumnsAndWhereClause(survey, parsedFilter, out var filterColumns))
                    .Replace("#TopCount#", topCount)
                    .Replace("#FilterColumns#", filterColumns)
                    .Replace("#SurveyId#", survey.SID.ToString())
                    .Replace("#ProjectId#", survey.ProjectId)
                    .Replace("#DestinationTableName#", survey.DestinationTableName)
                )
                .JoinStrings("\r\nUNION ALL\r\n");
            return query;
        }

        private string GenerateFilterColumnsAndWhereClause(BvSurveyEntity survey, List<DatabaseExpression> parsedFilter, out string filterColumns)
        {
            var whereClause = new StringBuilder();
            var columns = new List<string>();
            filterColumns = string.Empty;

            int paramIndex = 0;

            var metadataCache = _surveyMetadataCacheService.Get(survey.SID);

            foreach (var expression in parsedFilter)
            {
                var parameterName = "@P" + (++paramIndex);
                if (metadataCache.GetReplFormDesc(expression.ColumnName) != null)
                {
                    whereClause.AppendFormat(" AND r.{0} = {1}", expression.EscapedSqlColumnName, parameterName);
                    columns.Add(expression.EscapedSqlColumnName);
                }
                else
                {
                    columns.Add("NULL AS " + expression.EscapedSqlColumnName);
                }
            }
            if (columns.Count > 0)
            {
                filterColumns = "," + string.Join(",", columns);
            }

            return whereClause.ToString();
        }

        private string GenerateSurveyQueryTemplate(
            int interviewerId, string telephoneNumber, string respondentName, 
            List<DatabaseExpression> filter, List<SqlParameter> parameters)
        {
            //var additionalColumnList = filter.Select(e => string.Format(", [{0}]", e.Column)).JoinStrings("");
            var checkAssignmentsJoin = string.Empty;
            var whereClause = new StringBuilder();
            
            if (!string.IsNullOrEmpty(telephoneNumber))
            {
                whereClause.AppendFormat(" AND i.TelephoneNumber LIKE( @telephoneNumber )");
                parameters.Add(new SqlParameter("@telephoneNumber", telephoneNumber + "%"));
            }

            if (!string.IsNullOrEmpty(respondentName))
            {
                whereClause.AppendFormat(" AND i.RespondentName LIKE( @respondentName )");
                parameters.Add(new SqlParameter("@respondentName", respondentName + "%"));
            }

            var personAssignmentListMode = (PersonAssignmentListMode)_personRepository.GetById(interviewerId).AssignmentsListMode;
            if (personAssignmentListMode == PersonAssignmentListMode.AssignedCallsOnly)
            {
                checkAssignmentsJoin = "INNER JOIN BvPersonRel p ON p.ObjectSID = c.ExplicitSID and p.PersonSID = @PersonSID";
                parameters.Add(new SqlParameter("@PersonSID", interviewerId));
            }

            if (filter == null || !filter.Any())
            {
                return $@"SELECT TOP(#TopCount#) s.Name as ProjectId, i.ID as InterviewId, i.TelephoneNumber, i.RespondentName
                        FROM BvSvySchedule c 
                        INNER JOIN BvInterview i ON c.SurveySID = i.SurveySID AND c.InterviewId = i.ID
                        INNER JOIN BvSurvey s ON s.SID = c.SurveySID  
                        {checkAssignmentsJoin}
                        INNER JOIN @SurveyIds a ON i.SurveySID = a.Value
                        WHERE c.CallState <> 0{whereClause}";
            }

            int paramIndex = 0;
            foreach (var expression in filter)
            {
                var parameterName = "@P" + (++paramIndex);
                parameters.Add(new SqlParameter(parameterName, expression.Value));
            }

            return $@"SELECT TOP(#TopCount#) '#ProjectId#' as ProjectId, i.ID as InterviewId, i.TelephoneNumber, i.RespondentName #FilterColumns#
                    FROM BvSvySchedule c 
                    INNER JOIN BvInterview i ON c.SurveySID = i.SurveySID AND c.InterviewId = i.ID
                    {(paramIndex > 0 ? "INNER JOIN #DestinationTableName# as r ON i.ID = r.respid \r\n" : " \r\n")}
                    {checkAssignmentsJoin}
                    WHERE c.CallState <> 0 AND i.SurveySID = #SurveyId#{whereClause}";
        }

        private List<BvSurveyEntity> GetSurveys(int interviewerId, string[] projectIds)
        {
            return BvSurveyAdapter.ReadList(BvSpPerson_GetSurveysAdapter.ExecuteReader(interviewerId, 
                BvStringArrayTypeAdapter.CreateTable(projectIds ?? new string[]{})));
        }
    }
}
