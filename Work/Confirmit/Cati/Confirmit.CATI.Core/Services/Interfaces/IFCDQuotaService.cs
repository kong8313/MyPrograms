using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confirmit.CATI.Core.ManagementService;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public enum QuotaCellState
    {
        /// <summary>
        /// Opened in Confirmit
        /// </summary>
        PessimisticallyOpened = 0,

        /// <summary>
        /// LikelyToBecomeClosed in Confirmit
        /// </summary>
        OptimisticallyClosed = 1,

        /// <summary>
        /// Closed in Confirmit
        /// </summary>
        PessimisticallyClosed = 2,

        /// <summary>
        /// LikelyToStayOpen in Confirmit
        /// </summary>
        OptimisticallyOpened = 3
    }
    public interface IFcdQuotaService
    {
        void OnQuotaCellChanged(int surveyId, int quotaId, int cellId, QuotaCellState state);
        void OnQuotaUpdate(int surveyId, int quotaId);
        void OnQuotaCellsChanged(int surveySid, int quotaSid, int[] openedCfCellIds, int[] closedCfCellIds, int[] optimisticallyClosedCfCellIds);
        void OnQuotaCellsStateChanged(int surveySid, int quotaSid, List<CatiQuotaCellCountersState> quotaCellsCountersStates);
        void OnLaunchSurvey(int surveyId, bool runForceImport, CancellationToken cancellationToken);
        void OnDeleteSurvey(int surveyId);
        string GetCellInfo(int surveyId, int quotaId, int cellId);
    }
}
