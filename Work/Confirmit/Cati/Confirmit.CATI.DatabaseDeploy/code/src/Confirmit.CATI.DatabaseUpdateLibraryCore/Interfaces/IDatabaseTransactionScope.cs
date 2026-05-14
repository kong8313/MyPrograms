using System;

namespace Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces
{
    public interface IDatabaseTransactionScope: IDisposable
    {
        void Commit();
    }
}