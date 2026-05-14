using Confirmit.Databases;

namespace Confirmit.CATI.Core.Misc
{
    public class ConfirmitDatabaseProvider : IConfirmitDatabaseProvider
    {
        public string GetSurveyDatabaseName(string projectId)
        {
            return DbLib.GetSurveyDatabaseName(projectId, DatabaseConstants.DatabaseType.Production);
        }

        public string GetSqlServerName(string projectId, bool updateLastConnectionTime = true)
        {
            if(updateLastConnectionTime)
                return DbLib.GetSqlServerName(projectId);
            
            return DbLib.GetSqlServerNameWithoutUpdatingLastConnectTime(projectId);
        }

        public string GetSchemaName(string projectId)
        {
            return DbLib.GetSurveySchemaName(projectId, DatabaseConstants.DatabaseType.Production);
        }
    }
}
