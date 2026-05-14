using System;
using System.Collections.Generic;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;

namespace Confirmit.CATI.Core.Services
{
    public interface ICallQueueService
    {
        void ExpireAllCalls(CancellationToken cancellationToken = default(CancellationToken));
        void Schedule(DateTime? utcNow = null);

        bool AddCall([NotNull] BvCallEntity call);
        void ForceCallDelivery(BvCallEntity call = null);

        void ScheduleAndRemoveDeletedCalls(CancellationToken cancellationToken = default(CancellationToken));
        void SyncRuntimeStatistics(DeadlockPriority deadlockPriority);
        bool IsResourceLoggedIn(int resourceId, int surveySid);
        BvCallEntity GetCallWithTryLock(int surveySid, int interviewId, out bool isCallLocked);
        BvCallEntity GetCallWithTryLockAny(int surveySid, int interviewId, out bool isCallLocked);

        BvCallEntity GetCall(long callId);
        bool IsSurveyCallsShouldBeReassignedManually(int surveyId);
        List<BvShiftTypeChange> GetShiftTypesThatNeedChange(int newScheduleId, int surveySid);
    }
}