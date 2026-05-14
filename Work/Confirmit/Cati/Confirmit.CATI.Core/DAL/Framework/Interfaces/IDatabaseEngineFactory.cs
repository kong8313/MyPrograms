using System;

namespace Confirmit.CATI.Core.DAL.Framework.Interfaces
{
    public interface IDatabaseEngineFactory
    {
        IDatabaseEngine CreateForCurrentInstanceDatabase();
        IDatabaseEngine CreateForDefaultInstanceDatabase();
        IDatabaseEngine CreateForConfirmlogDatabase();
        IDatabaseEngine CreateForConfirmDatabase();
        IDatabaseEngine CreateForCustomConnectionProvider(IConnectionProvider connectionProvider);
    }
}