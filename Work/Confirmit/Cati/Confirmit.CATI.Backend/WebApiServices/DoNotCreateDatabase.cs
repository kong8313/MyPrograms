using System.Data.Entity;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public class DoNotCreateDatabase<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        public void InitializeDatabase(TContext context)
        {
        }
    }
}
