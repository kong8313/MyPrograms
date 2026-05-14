using System;

namespace Confirmit.CATI.Core.DAL.Framework
{
    public interface IDatabaseTransactionScope: IDisposable
    {
        void Commit();
    }
}