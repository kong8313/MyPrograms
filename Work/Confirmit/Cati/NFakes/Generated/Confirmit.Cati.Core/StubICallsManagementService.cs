using System;
using Confirmit.CATI.Core.Services;
using System.Collections.Generic;
using ConfirmitDialerInterface;
using System.Data;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubICallsManagementService : ICallsManagementService 
    {
        private ICallsManagementService _inner;

        public StubICallsManagementService()
        {
            _inner = null;
        }

        public ICallsManagementService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ActivateNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfDateTimeNullableOfBooleanNullableOfInt32NullableOfInt32Delegate(int? SurveySID, int? Mode, int? BatchID, int? Priority, int? PersonSID, int? ShiftTypeID, DateTime? TimeToCall, bool? EnableDisabledCalls, int? DefaultTZID, int? ITS);
        public ActivateNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfDateTimeNullableOfBooleanNullableOfInt32NullableOfInt32Delegate ActivateNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfDateTimeNullableOfBooleanNullableOfInt32NullableOfInt32;

        void ICallsManagementService.Activate(int? SurveySID, int? Mode, int? BatchID, int? Priority, int? PersonSID, int? ShiftTypeID, DateTime? TimeToCall, bool? EnableDisabledCalls, int? DefaultTZID, int? ITS)
        {

            if (ActivateNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfDateTimeNullableOfBooleanNullableOfInt32NullableOfInt32 != null)
            {
                ActivateNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfDateTimeNullableOfBooleanNullableOfInt32NullableOfInt32(SurveySID, Mode, BatchID, Priority, PersonSID, ShiftTypeID, TimeToCall, EnableDisabledCalls, DefaultTZID, ITS);
            } else if (_inner != null)
            {
                ((ICallsManagementService)_inner).Activate(SurveySID, Mode, BatchID, Priority, PersonSID, ShiftTypeID, TimeToCall, EnableDisabledCalls, DefaultTZID, ITS);
            }
        }

        public delegate void MoveToItsNullableOfInt32NullableOfInt32NullableOfInt32Delegate(int? surveySid, int? batchId, int? stateId);
        public MoveToItsNullableOfInt32NullableOfInt32NullableOfInt32Delegate MoveToItsNullableOfInt32NullableOfInt32NullableOfInt32;

        void ICallsManagementService.MoveToIts(int? surveySid, int? batchId, int? stateId)
        {

            if (MoveToItsNullableOfInt32NullableOfInt32NullableOfInt32 != null)
            {
                MoveToItsNullableOfInt32NullableOfInt32NullableOfInt32(surveySid, batchId, stateId);
            } else if (_inner != null)
            {
                ((ICallsManagementService)_inner).MoveToIts(surveySid, batchId, stateId);
            }
        }

        public delegate List<CallInfo> GetCallsToFlushOnDialerInt32Int32BooleanDelegate(int surveyId, int batchId, bool isRecording);
        public GetCallsToFlushOnDialerInt32Int32BooleanDelegate GetCallsToFlushOnDialerInt32Int32Boolean;

        List<CallInfo> ICallsManagementService.GetCallsToFlushOnDialer(int surveyId, int batchId, bool isRecording)
        {


            if (GetCallsToFlushOnDialerInt32Int32Boolean != null)
            {
                return GetCallsToFlushOnDialerInt32Int32Boolean(surveyId, batchId, isRecording);
            } else if (_inner != null)
            {
                return ((ICallsManagementService)_inner).GetCallsToFlushOnDialer(surveyId, batchId, isRecording);
            }

            return default(List<CallInfo>);
        }

        public delegate List<CallInfo> ReadFlushedCallInfosBooleanIDataReaderDelegate(bool isRecording, IDataReader dataReader);
        public ReadFlushedCallInfosBooleanIDataReaderDelegate ReadFlushedCallInfosBooleanIDataReader;

        List<CallInfo> ICallsManagementService.ReadFlushedCallInfos(bool isRecording, IDataReader dataReader)
        {


            if (ReadFlushedCallInfosBooleanIDataReader != null)
            {
                return ReadFlushedCallInfosBooleanIDataReader(isRecording, dataReader);
            } else if (_inner != null)
            {
                return ((ICallsManagementService)_inner).ReadFlushedCallInfos(isRecording, dataReader);
            }

            return default(List<CallInfo>);
        }

        public delegate void RemoveFilteredCallsInt32Int32NullableOfInt32Delegate(int surveyId, int batchId, int? newIts);
        public RemoveFilteredCallsInt32Int32NullableOfInt32Delegate RemoveFilteredCallsInt32Int32NullableOfInt32;

        void ICallsManagementService.RemoveFilteredCalls(int surveyId, int batchId, int? newIts)
        {

            if (RemoveFilteredCallsInt32Int32NullableOfInt32 != null)
            {
                RemoveFilteredCallsInt32Int32NullableOfInt32(surveyId, batchId, newIts);
            } else if (_inner != null)
            {
                ((ICallsManagementService)_inner).RemoveFilteredCalls(surveyId, batchId, newIts);
            }
        }

        public delegate void EditInt32Int32NullableOfDateTimeNullableOfDateTimeNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfByteDelegate(int surveySid, int batchId, DateTime? timeToCall, DateTime? timeToExpire, int? callState, int? callPriority, int? shiftType, int? extendedStatus, byte? dialingMode);
        public EditInt32Int32NullableOfDateTimeNullableOfDateTimeNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfByteDelegate EditInt32Int32NullableOfDateTimeNullableOfDateTimeNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfByte;

        void ICallsManagementService.Edit(int surveySid, int batchId, DateTime? timeToCall, DateTime? timeToExpire, int? callState, int? callPriority, int? shiftType, int? extendedStatus, byte? dialingMode)
        {

            if (EditInt32Int32NullableOfDateTimeNullableOfDateTimeNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfByte != null)
            {
                EditInt32Int32NullableOfDateTimeNullableOfDateTimeNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfByte(surveySid, batchId, timeToCall, timeToExpire, callState, callPriority, shiftType, extendedStatus, dialingMode);
            } else if (_inner != null)
            {
                ((ICallsManagementService)_inner).Edit(surveySid, batchId, timeToCall, timeToExpire, callState, callPriority, shiftType, extendedStatus, dialingMode);
            }
        }

    }
}