using System;
using System.Threading;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Common;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubICallQueueService : ICallQueueService 
    {
        private ICallQueueService _inner;

        public StubICallQueueService()
        {
            _inner = null;
        }

        public ICallQueueService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ExpireAllCallsCancellationTokenDelegate(CancellationToken cancellationToken);
        public ExpireAllCallsCancellationTokenDelegate ExpireAllCallsCancellationToken;

        void ICallQueueService.ExpireAllCalls(CancellationToken cancellationToken)
        {

            if (ExpireAllCallsCancellationToken != null)
            {
                ExpireAllCallsCancellationToken(cancellationToken);
            } else if (_inner != null)
            {
                ((ICallQueueService)_inner).ExpireAllCalls(cancellationToken);
            }
        }

        public delegate void ScheduleNullableOfDateTimeDelegate(DateTime? utcNow);
        public ScheduleNullableOfDateTimeDelegate ScheduleNullableOfDateTime;

        void ICallQueueService.Schedule(DateTime? utcNow)
        {

            if (ScheduleNullableOfDateTime != null)
            {
                ScheduleNullableOfDateTime(utcNow);
            } else if (_inner != null)
            {
                ((ICallQueueService)_inner).Schedule(utcNow);
            }
        }

        public delegate bool AddCallBvCallEntityDelegate(BvCallEntity call);
        public AddCallBvCallEntityDelegate AddCallBvCallEntity;

        bool ICallQueueService.AddCall(BvCallEntity call)
        {


            if (AddCallBvCallEntity != null)
            {
                return AddCallBvCallEntity(call);
            } else if (_inner != null)
            {
                return ((ICallQueueService)_inner).AddCall(call);
            }

            return default(bool);
        }

        public delegate void ForceCallDeliveryBvCallEntityDelegate(BvCallEntity call);
        public ForceCallDeliveryBvCallEntityDelegate ForceCallDeliveryBvCallEntity;

        void ICallQueueService.ForceCallDelivery(BvCallEntity call)
        {

            if (ForceCallDeliveryBvCallEntity != null)
            {
                ForceCallDeliveryBvCallEntity(call);
            } else if (_inner != null)
            {
                ((ICallQueueService)_inner).ForceCallDelivery(call);
            }
        }

        public delegate void ScheduleAndRemoveDeletedCallsCancellationTokenDelegate(CancellationToken cancellationToken);
        public ScheduleAndRemoveDeletedCallsCancellationTokenDelegate ScheduleAndRemoveDeletedCallsCancellationToken;

        void ICallQueueService.ScheduleAndRemoveDeletedCalls(CancellationToken cancellationToken)
        {

            if (ScheduleAndRemoveDeletedCallsCancellationToken != null)
            {
                ScheduleAndRemoveDeletedCallsCancellationToken(cancellationToken);
            } else if (_inner != null)
            {
                ((ICallQueueService)_inner).ScheduleAndRemoveDeletedCalls(cancellationToken);
            }
        }

        public delegate void SyncRuntimeStatisticsDeadlockPriorityDelegate(DeadlockPriority deadlockPriority);
        public SyncRuntimeStatisticsDeadlockPriorityDelegate SyncRuntimeStatisticsDeadlockPriority;

        void ICallQueueService.SyncRuntimeStatistics(DeadlockPriority deadlockPriority)
        {

            if (SyncRuntimeStatisticsDeadlockPriority != null)
            {
                SyncRuntimeStatisticsDeadlockPriority(deadlockPriority);
            } else if (_inner != null)
            {
                ((ICallQueueService)_inner).SyncRuntimeStatistics(deadlockPriority);
            }
        }

        public delegate bool IsResourceLoggedInInt32Int32Delegate(int resourceId, int surveySid);
        public IsResourceLoggedInInt32Int32Delegate IsResourceLoggedInInt32Int32;

        bool ICallQueueService.IsResourceLoggedIn(int resourceId, int surveySid)
        {


            if (IsResourceLoggedInInt32Int32 != null)
            {
                return IsResourceLoggedInInt32Int32(resourceId, surveySid);
            } else if (_inner != null)
            {
                return ((ICallQueueService)_inner).IsResourceLoggedIn(resourceId, surveySid);
            }

            return default(bool);
        }

        public delegate BvCallEntity GetCallWithTryLockInt32Int32BooleanOutDelegate(int surveySid, int interviewId, out bool isCallLocked);
        public GetCallWithTryLockInt32Int32BooleanOutDelegate GetCallWithTryLockInt32Int32BooleanOut;

        BvCallEntity ICallQueueService.GetCallWithTryLock(int surveySid, int interviewId, out bool isCallLocked)
        {
            isCallLocked = default(bool);


            if (GetCallWithTryLockInt32Int32BooleanOut != null)
            {
                return GetCallWithTryLockInt32Int32BooleanOut(surveySid, interviewId, out isCallLocked);
            } else if (_inner != null)
            {
                return ((ICallQueueService)_inner).GetCallWithTryLock(surveySid, interviewId, out isCallLocked);
            }

            return default(BvCallEntity);
        }

        public delegate BvCallEntity GetCallWithTryLockAnyInt32Int32BooleanOutDelegate(int surveySid, int interviewId, out bool isCallLocked);
        public GetCallWithTryLockAnyInt32Int32BooleanOutDelegate GetCallWithTryLockAnyInt32Int32BooleanOut;

        BvCallEntity ICallQueueService.GetCallWithTryLockAny(int surveySid, int interviewId, out bool isCallLocked)
        {
            isCallLocked = default(bool);


            if (GetCallWithTryLockAnyInt32Int32BooleanOut != null)
            {
                return GetCallWithTryLockAnyInt32Int32BooleanOut(surveySid, interviewId, out isCallLocked);
            } else if (_inner != null)
            {
                return ((ICallQueueService)_inner).GetCallWithTryLockAny(surveySid, interviewId, out isCallLocked);
            }

            return default(BvCallEntity);
        }

        public delegate BvCallEntity GetCallInt64Delegate(long callId);
        public GetCallInt64Delegate GetCallInt64;

        BvCallEntity ICallQueueService.GetCall(long callId)
        {


            if (GetCallInt64 != null)
            {
                return GetCallInt64(callId);
            } else if (_inner != null)
            {
                return ((ICallQueueService)_inner).GetCall(callId);
            }

            return default(BvCallEntity);
        }

        public delegate bool IsSurveyCallsShouldBeReassignedManuallyInt32Delegate(int surveyId);
        public IsSurveyCallsShouldBeReassignedManuallyInt32Delegate IsSurveyCallsShouldBeReassignedManuallyInt32;

        bool ICallQueueService.IsSurveyCallsShouldBeReassignedManually(int surveyId)
        {


            if (IsSurveyCallsShouldBeReassignedManuallyInt32 != null)
            {
                return IsSurveyCallsShouldBeReassignedManuallyInt32(surveyId);
            } else if (_inner != null)
            {
                return ((ICallQueueService)_inner).IsSurveyCallsShouldBeReassignedManually(surveyId);
            }

            return default(bool);
        }

        public delegate List<BvShiftTypeChange> GetShiftTypesThatNeedChangeInt32Int32Delegate(int newScheduleId, int surveySid);
        public GetShiftTypesThatNeedChangeInt32Int32Delegate GetShiftTypesThatNeedChangeInt32Int32;

        List<BvShiftTypeChange> ICallQueueService.GetShiftTypesThatNeedChange(int newScheduleId, int surveySid)
        {


            if (GetShiftTypesThatNeedChangeInt32Int32 != null)
            {
                return GetShiftTypesThatNeedChangeInt32Int32(newScheduleId, surveySid);
            } else if (_inner != null)
            {
                return ((ICallQueueService)_inner).GetShiftTypesThatNeedChange(newScheduleId, surveySid);
            }

            return default(List<BvShiftTypeChange>);
        }

    }
}