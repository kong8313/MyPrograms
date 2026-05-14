using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.CompanyService;
using Confirmit.Configuration;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation
{
    public class ProjectsActivityService : IProjectsActivityService
    {
        private readonly IConnectionStrings _connectionStrings;
        private readonly ICompanyInfo _companyInfo;
        private readonly ICompanyInformationService _companyInformationService;
        
        public ProjectsActivityService(IConnectionStrings connectionStrings, ICompanyInfo companyInfo, ICompanyInformationService companyInformationService)
        {
            _connectionStrings = connectionStrings;
            _companyInfo = companyInfo;
            _companyInformationService = companyInformationService;
        }

        public IEnumerable<string> GetActiveProjectIds(IEnumerable<string> surveyNames)
        {
            var activeSurveys = GetActiveProjectIds();
            return activeSurveys.Intersect(surveyNames);
        }

        private string GetCompanyIdCondition()
        {
            var childCompanyIds = _companyInformationService.GetChildCompanyIds(_companyInfo.CompanyId);

            if (childCompanyIds.Any())
            {
                return $"[company_id] IN ({_companyInfo.CompanyId},{string.Join(",", childCompanyIds)})";
            }

            return $"[company_id] = {_companyInfo.CompanyId}";
        }

        private IEnumerable<string> GetActiveProjectIds()
        {
            using (var connection = new SqlConnection(_connectionStrings.ConfirmConnectionString))
            {
                connection.Open();
                var query = $@"
                            SELECT [project_cde] FROM [projects]
                            WHERE [LastDatabaseConnect] > @OneDayAgo AND {GetCompanyIdCondition()}";

                using (var sqlCommand = new SqlCommand(query, connection))
                {
                    sqlCommand.Parameters.AddWithValue("OneDayAgo", DateTime.UtcNow.AddDays(-1));

                    var reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        var projectId = (string)reader["project_cde"];
                        yield return projectId;
                    }
                }
            }
        }
    }
}