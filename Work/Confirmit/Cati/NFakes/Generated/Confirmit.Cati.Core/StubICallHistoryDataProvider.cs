using System;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Query;
using Confirmit.CATI.Core.Reports;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Reports.Fakes
{
    public class StubICallHistoryDataProvider : ICallHistoryDataProvider 
    {
        private ICallHistoryDataProvider _inner;

        public StubICallHistoryDataProvider()
        {
            _inner = null;
        }

        public ICallHistoryDataProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Object[] PrepareForExportCallHistoryDataEntityDelegate(CallHistoryDataEntity x);
        public PrepareForExportCallHistoryDataEntityDelegate PrepareForExportCallHistoryDataEntity;

        Object[] ICallHistoryDataProvider.PrepareForExport(CallHistoryDataEntity x)
        {


            if (PrepareForExportCallHistoryDataEntity != null)
            {
                return PrepareForExportCallHistoryDataEntity(x);
            } else if (_inner != null)
            {
                return ((ICallHistoryDataProvider)_inner).PrepareForExport(x);
            }

            return default(Object[]);
        }

        public delegate List<CallHistoryDataEntity> GetCallHistoryDataStringNullableOfDateTimeNullableOfDateTimeArrayOfStringBooleanBooleanDelegate(string surveyIds, DateTime? startTime, DateTime? endTime, string[] variables, bool includeBreakTimes, bool includeLoginLogoutInfo);
        public GetCallHistoryDataStringNullableOfDateTimeNullableOfDateTimeArrayOfStringBooleanBooleanDelegate GetCallHistoryDataStringNullableOfDateTimeNullableOfDateTimeArrayOfStringBooleanBoolean;

        List<CallHistoryDataEntity> ICallHistoryDataProvider.GetCallHistoryData(string surveyIds, DateTime? startTime, DateTime? endTime, string[] variables, bool includeBreakTimes, bool includeLoginLogoutInfo)
        {


            if (GetCallHistoryDataStringNullableOfDateTimeNullableOfDateTimeArrayOfStringBooleanBoolean != null)
            {
                return GetCallHistoryDataStringNullableOfDateTimeNullableOfDateTimeArrayOfStringBooleanBoolean(surveyIds, startTime, endTime, variables, includeBreakTimes, includeLoginLogoutInfo);
            } else if (_inner != null)
            {
                return ((ICallHistoryDataProvider)_inner).GetCallHistoryData(surveyIds, startTime, endTime, variables, includeBreakTimes, includeLoginLogoutInfo);
            }

            return default(List<CallHistoryDataEntity>);
        }

        public delegate List<CallHistoryDataEntity> GetCallHistoryDataStringNullableOfDateTimeNullableOfDateTimeArrayOfStringDelegate(string surveySIDs, DateTime? startTime, DateTime? endTime, string[] replicatedVariables);
        public GetCallHistoryDataStringNullableOfDateTimeNullableOfDateTimeArrayOfStringDelegate GetCallHistoryDataStringNullableOfDateTimeNullableOfDateTimeArrayOfString;

        List<CallHistoryDataEntity> ICallHistoryDataProvider.GetCallHistoryData(string surveySIDs, DateTime? startTime, DateTime? endTime, string[] replicatedVariables)
        {


            if (GetCallHistoryDataStringNullableOfDateTimeNullableOfDateTimeArrayOfString != null)
            {
                return GetCallHistoryDataStringNullableOfDateTimeNullableOfDateTimeArrayOfString(surveySIDs, startTime, endTime, replicatedVariables);
            } else if (_inner != null)
            {
                return ((ICallHistoryDataProvider)_inner).GetCallHistoryData(surveySIDs, startTime, endTime, replicatedVariables);
            }

            return default(List<CallHistoryDataEntity>);
        }

        public delegate IEnumerable<CallHistoryDataEntity> GetPersonSessionHistoryDataNullableOfInt32NullableOfDateTimeNullableOfDateTimeDelegate(int? callCenterId, DateTime? startTime, DateTime? finishTime);
        public GetPersonSessionHistoryDataNullableOfInt32NullableOfDateTimeNullableOfDateTimeDelegate GetPersonSessionHistoryDataNullableOfInt32NullableOfDateTimeNullableOfDateTime;

        IEnumerable<CallHistoryDataEntity> ICallHistoryDataProvider.GetPersonSessionHistoryData(int? callCenterId, DateTime? startTime, DateTime? finishTime)
        {


            if (GetPersonSessionHistoryDataNullableOfInt32NullableOfDateTimeNullableOfDateTime != null)
            {
                return GetPersonSessionHistoryDataNullableOfInt32NullableOfDateTimeNullableOfDateTime(callCenterId, startTime, finishTime);
            } else if (_inner != null)
            {
                return ((ICallHistoryDataProvider)_inner).GetPersonSessionHistoryData(callCenterId, startTime, finishTime);
            }

            return default(IEnumerable<CallHistoryDataEntity>);
        }

        public delegate IEnumerable<CallHistoryDataEntity> GetInterviewerBreaksDataStringNullableOfDateTimeNullableOfDateTimeDelegate(string surveySIDs, DateTime? startTime, DateTime? endTime);
        public GetInterviewerBreaksDataStringNullableOfDateTimeNullableOfDateTimeDelegate GetInterviewerBreaksDataStringNullableOfDateTimeNullableOfDateTime;

        IEnumerable<CallHistoryDataEntity> ICallHistoryDataProvider.GetInterviewerBreaksData(string surveySIDs, DateTime? startTime, DateTime? endTime)
        {


            if (GetInterviewerBreaksDataStringNullableOfDateTimeNullableOfDateTime != null)
            {
                return GetInterviewerBreaksDataStringNullableOfDateTimeNullableOfDateTime(surveySIDs, startTime, endTime);
            } else if (_inner != null)
            {
                return ((ICallHistoryDataProvider)_inner).GetInterviewerBreaksData(surveySIDs, startTime, endTime);
            }

            return default(IEnumerable<CallHistoryDataEntity>);
        }

        public delegate string GetHeaderStringDelegate(string replicatedVariables);
        public GetHeaderStringDelegate GetHeaderString;

        string ICallHistoryDataProvider.GetHeader(string replicatedVariables)
        {


            if (GetHeaderString != null)
            {
                return GetHeaderString(replicatedVariables);
            } else if (_inner != null)
            {
                return ((ICallHistoryDataProvider)_inner).GetHeader(replicatedVariables);
            }

            return default(string);
        }

        private bool _IncludeReplicatedVariables;
        public Func<bool> IncludeReplicatedVariablesGet;
        public Action<bool> IncludeReplicatedVariablesSetBoolean;

        bool ICallHistoryDataProvider.IncludeReplicatedVariables
        {
            get
            {
                if (IncludeReplicatedVariablesGet != null)
                {
                    return IncludeReplicatedVariablesGet();
                } else if (_inner != null)
                {
                    return ((ICallHistoryDataProvider)_inner).IncludeReplicatedVariables;
                }

                if (IncludeReplicatedVariablesSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IncludeReplicatedVariables;
                }

                return default(bool);
            }

            set
            {
                if (IncludeReplicatedVariablesSetBoolean != null)
                {
                    IncludeReplicatedVariablesSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICallHistoryDataProvider)_inner).IncludeReplicatedVariables = value;
                    return;
                }

                if (IncludeReplicatedVariablesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IncludeReplicatedVariables = value;
                }

            }
        }

    }
}