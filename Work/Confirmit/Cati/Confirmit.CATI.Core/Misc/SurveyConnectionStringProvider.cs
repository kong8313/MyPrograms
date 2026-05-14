using System.Data.SqlClient;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Misc
{
    public class SurveyConnectionStringProvider : ISurveyConnectionStringProvider
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IConfirmitDatabaseProvider _confirmitDatabaseProvider;
        private readonly IConnectionStrings _connectionStrings;
        private readonly ICompanyInfo _companyInfo;

        public SurveyConnectionStringProvider(
            ISurveyRepository surveyRepository, 
            IConfirmitDatabaseProvider confirmitDatabaseProvider, 
            IConnectionStrings connectionStrings, 
            ICompanyInfo companyInfo)
        {
            _surveyRepository = surveyRepository;
            _confirmitDatabaseProvider = confirmitDatabaseProvider;
            _connectionStrings = connectionStrings;
            _companyInfo = companyInfo;
        }

        public SurveyConnectionInfo GetConnectionInfo(int surveyId, bool updateLastConnectionTime = true)
        {            
            var projectId = _surveyRepository.GetById(surveyId).ProjectId;
            var dbName = _confirmitDatabaseProvider.GetSurveyDatabaseName(projectId);
            var dataSource = _confirmitDatabaseProvider.GetSqlServerName(projectId, updateLastConnectionTime);
            var schemaName = _confirmitDatabaseProvider.GetSchemaName(projectId);

            var catiConnectionString = new SqlConnectionStringBuilder(_connectionStrings.GetConnectionStringForSpecificCompany(_companyInfo.CompanyId));
            var surveyDbConnectionString = new SqlConnectionStringBuilder
            {
                DataSource = dataSource,
                InitialCatalog = dbName,
                UserID = catiConnectionString.UserID,
                Password = catiConnectionString.Password,
                MaxPoolSize = catiConnectionString.MaxPoolSize
            };

            return new SurveyConnectionInfo(surveyDbConnectionString.ToString(), schemaName);
        }
    }
}