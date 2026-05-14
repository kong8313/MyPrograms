using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Surveys
{
    internal class CallEntendedHistoryItemProvider
    {
        public static readonly DateTime TimeInsteadNowTimeToCall = new DateTime(1899, 12, 30);
        private BvSpGetExtendedCallHistoryEntity m_entity;
        private string _operationTitle;
        private string _dialingModeTitle;


        public CallEntendedHistoryItemProvider(BvSpGetExtendedCallHistoryEntity entity, string operationTitle, string dialingModeTitle)
        {
            m_entity = entity;
            _operationTitle = operationTitle;
            _dialingModeTitle = dialingModeTitle;
        }

        public long? Id 
        {
            get { return m_entity.Id; }
        }

        public DateTime? FiredTime
        {
            get { return m_entity.FiredTime; }
        }

        public int? ApptId
        {
            get { return m_entity.ApptID; }
        }

        public int? ITS
        {
            get { return m_entity.ITS; }
        }

        public string TransientState
        {
            get { return m_entity.TransientState; }
        }

        public int? ShiftTypeId
        {
            get { return m_entity.ShiftTypeId; }
        }

        public string ShiftType
        {
            get
            {
                if (ShiftTypeId == (int)CallShiftType.None)
                    return  Strings.ShiftTypeNoneString;

                if (ShiftTypeId <= 0)
                    return Strings.ShiftTypeAnyString;

                return m_entity.ShiftType;
            }
        }

        public string DialingMode
        {
            get { return _dialingModeTitle; } 
        }

        public string CallStateInfo
        {
            get
            {
                string state = String.Empty;
                if (m_entity.CallState == (short) CallState.ToBeDeleted)
                    state = Strings.CallIsDeleted;
                else if (m_entity.CallState == (short) CallState.DisabledByFCD)
                    state = Strings.DisabledByFCDStateString;
                else if (m_entity.CallState == (short)CallState.DisabledByUser)
                    state = Strings.DisabledByUserStateString;
                else if (m_entity.CallState == (short) CallState.Scheduled)
                    state = Strings.Enabled;
                else if (m_entity.CallState == (short) CallState.InterviewInProgress)
                    state = Strings.OperationInProgress;
                else if (m_entity.CallState == (short) CallState.LoadedToDialerPredictively)
                    state = Strings.SentToDialer;

                return state;
            }
        }

        public int? Priority
        {
            get { return m_entity.Priority; }
        }

        public string TimeInShift
        {
            get
            {
                if (m_entity.TimeInShift == TimeInsteadNowTimeToCall)
                {
                    return Strings.Now;
                }
                if (m_entity.TimeInShift == null)
                {
                    return string.Empty;
                }
                
                return  m_entity.TimeInShift.ToString();
            }
        }

        public DateTime? ExpireTime
        {
            get { return m_entity.ExpireTime; }
        }

        public int? ExplicitSid
        {
            get { return m_entity.ExplicitSID; }
        }

        public int? ExplicitType
        {
            get { return m_entity.ExplicitType; }
        }

        public string Resource
        {
            get { return m_entity.Resource; }
        }

        public int? CellId
        {
            get { return m_entity.CellId; }
        }

        public int? OperationId
        {
            get { return m_entity.OperationId; }
        }

        public string OperationType
        {
            get { return _operationTitle; }
        }

        public string CallCenterName
        {
            get { return m_entity.CallCenterName; }
        }

        public string DialType
        {
            get { return m_entity.DialType; }
        }
    }
}