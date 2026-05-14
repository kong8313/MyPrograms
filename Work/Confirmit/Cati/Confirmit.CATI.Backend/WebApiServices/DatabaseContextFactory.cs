namespace Confirmit.CATI.Backend.WebApiServices
{
    public class DatabaseContextFactory : IDatabaseContextFactory
    {
        public IDatabaseContext CreateDatabaseContext(string connectionString)
        {
            return new DatabaseContext(connectionString);
        }
    }
}