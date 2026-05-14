namespace Confirmit.CATI.DatabaseUpdateLibrary.PowerShellApi
{
    public class ApiHost
    {
        public ApiHost(ApiConfiguration configuration)
        {
            Databases = new ApiDatabases(configuration);
        }

        public ApiDatabases Databases { get; }
    }
}
