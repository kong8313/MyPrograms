namespace Confirmit.CATI.Core.Misc
{
    public class SurveyConnectionInfo
    {
        public SurveyConnectionInfo(string connectionString, string schemaName = "dbo")
        {
            ConnectionString = connectionString;
            SchemaName = schemaName;
        }

        public string SchemaName { get; private set; }
        public string ConnectionString { get; private set; }
    }
}
