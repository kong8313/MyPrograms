using System;
using System.Collections.Generic;
using System.Data;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services
{
    public interface ICallsManagementService
    {
        void Activate(
            int? SurveySID,
            int? Mode,
            int? BatchID,
            int? Priority,
            int? PersonSID,
            int? ShiftTypeID,
            DateTime? TimeToCall,
            bool? EnableDisabledCalls,
            int? DefaultTZID,
            int? ITS);

        void MoveToIts(
            int? surveySid,
            int? batchId,
            int? stateId);

        List<CallInfo> GetCallsToFlushOnDialer(
            int surveyId,
            int batchId,
            bool isRecording);

        List<CallInfo> ReadFlushedCallInfos(
            bool isRecording,
            IDataReader dataReader);

        void RemoveFilteredCalls(
            int surveyId,
            int batchId,
            int? newIts);

        void Edit(int surveySid,
            int batchId,
            DateTime? timeToCall,
            DateTime? timeToExpire,
            int? callState,
            int? callPriority,
            int? shiftType,
            int? extendedStatus,
            byte? dialingMode);
    }
}