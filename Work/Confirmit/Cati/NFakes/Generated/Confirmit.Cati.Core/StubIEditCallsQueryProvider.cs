using System;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIEditCallsQueryProvider : IEditCallsQueryProvider 
    {
        private IEditCallsQueryProvider _inner;

        public StubIEditCallsQueryProvider()
        {
            _inner = null;
        }

        public IEditCallsQueryProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetQueryInt32Int32NullableOfDateTimeNullableOfDateTimeNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfByteInt32StringInt32Delegate(int surveySid, int batchId, DateTime? timeToCall, DateTime? timeToExpire, int? callState, int? callPriority, int? shiftType, int? extendedStatus, byte? dialingMode, int fcdBehaviorAlgorithm, string whereCondition, int stateGroupId);
        public GetQueryInt32Int32NullableOfDateTimeNullableOfDateTimeNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfByteInt32StringInt32Delegate GetQueryInt32Int32NullableOfDateTimeNullableOfDateTimeNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfByteInt32StringInt32;

        string IEditCallsQueryProvider.GetQuery(int surveySid, int batchId, DateTime? timeToCall, DateTime? timeToExpire, int? callState, int? callPriority, int? shiftType, int? extendedStatus, byte? dialingMode, int fcdBehaviorAlgorithm, string whereCondition, int stateGroupId)
        {


            if (GetQueryInt32Int32NullableOfDateTimeNullableOfDateTimeNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfByteInt32StringInt32 != null)
            {
                return GetQueryInt32Int32NullableOfDateTimeNullableOfDateTimeNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfByteInt32StringInt32(surveySid, batchId, timeToCall, timeToExpire, callState, callPriority, shiftType, extendedStatus, dialingMode, fcdBehaviorAlgorithm, whereCondition, stateGroupId);
            } else if (_inner != null)
            {
                return ((IEditCallsQueryProvider)_inner).GetQuery(surveySid, batchId, timeToCall, timeToExpire, callState, callPriority, shiftType, extendedStatus, dialingMode, fcdBehaviorAlgorithm, whereCondition, stateGroupId);
            }

            return default(string);
        }

        public delegate IEnumerable<SqlParameter> GetSqlParametersInt32Int32NullableOfDateTimeNullableOfDateTimeNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfByteInt32Int32Delegate(int surveySid, int batchId, DateTime? timeToCall, DateTime? timeToExpire, int? callState, int? callPriority, int? shiftType, int? extendedStatus, byte? dialingMode, int fcdBehaviorAlgorithm, int stateGroupId);
        public GetSqlParametersInt32Int32NullableOfDateTimeNullableOfDateTimeNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfByteInt32Int32Delegate GetSqlParametersInt32Int32NullableOfDateTimeNullableOfDateTimeNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfByteInt32Int32;

        IEnumerable<SqlParameter> IEditCallsQueryProvider.GetSqlParameters(int surveySid, int batchId, DateTime? timeToCall, DateTime? timeToExpire, int? callState, int? callPriority, int? shiftType, int? extendedStatus, byte? dialingMode, int fcdBehaviorAlgorithm, int stateGroupId)
        {


            if (GetSqlParametersInt32Int32NullableOfDateTimeNullableOfDateTimeNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfByteInt32Int32 != null)
            {
                return GetSqlParametersInt32Int32NullableOfDateTimeNullableOfDateTimeNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfByteInt32Int32(surveySid, batchId, timeToCall, timeToExpire, callState, callPriority, shiftType, extendedStatus, dialingMode, fcdBehaviorAlgorithm, stateGroupId);
            } else if (_inner != null)
            {
                return ((IEditCallsQueryProvider)_inner).GetSqlParameters(surveySid, batchId, timeToCall, timeToExpire, callState, callPriority, shiftType, extendedStatus, dialingMode, fcdBehaviorAlgorithm, stateGroupId);
            }

            return default(IEnumerable<SqlParameter>);
        }

    }
}