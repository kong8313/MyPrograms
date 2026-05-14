using System;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface ICallHistoryRepository
    {
        void CleanUpExpiredRecords(TimeSpan callHistoryCleanPeriod);
    }
}