namespace Confirmit.CATI.Core.DAL.Framework.Interfaces
{
    public interface IDatabaseConnectionProviderFactory
    {
        ITransactedConnectionProvider CreateConnectionProviderForConfirmlogDatabase();
    }
}