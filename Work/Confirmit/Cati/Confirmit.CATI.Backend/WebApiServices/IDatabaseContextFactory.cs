namespace Confirmit.CATI.Backend.WebApiServices
{
    public interface IDatabaseContextFactory
    {
        IDatabaseContext CreateDatabaseContext(string connectionString);
    }
}