using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Query;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IPersonSessionHistoryRepository
    {
        /// <summary>
        /// Insert start session event to database
        /// </summary>
        /// <param name="connectionProvider"></param>
        /// <param name="callCenterId"></param>
        /// <param name="personId"></param>
        /// <returns>return SessionId</returns>
        int InsertStartSessionEvent(IConnectionProvider connectionProvider, int callCenterId, int personId);

        /// <summary>
        /// Insert stop session event to database
        /// </summary>
        /// <param name="connectionProvider"></param>
        /// <param name="sessionId"></param>
        void InsertStopSessionEvent(IConnectionProvider connectionProvider, int sessionId);

        /// <summary>
        /// Select events from database
        /// </summary>
        /// <returns></returns>
        IEnumerable<PersonSessionHistoryEntity> GetSessionEvents(int? callCenterId, int companyId, DateTime? starTime, DateTime? endTime);
    }
}