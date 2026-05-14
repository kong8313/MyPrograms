using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Timezone;

namespace Confirmit.CATI.Supervisor.Surveys
{
    internal class CallHistoryItemProvider
    {
        private readonly ICachedLocalTimezoneManager _timezoneProvider =
            ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        private BvSpCallHistory_ListEntity m_Info;
        private readonly bool _isNeedToHidePii;

        public CallHistoryItemProvider(BvSpCallHistory_ListEntity entity, bool isNeedToHidePii)
        {
            m_Info = entity;
            _isNeedToHidePii = isNeedToHidePii;
        }

        public int? CallHistoryId => m_Info.ID;

        public int? SurveyId => m_Info.SurveyID;

        public DateTime? StartTime
        {
            get
            {
                if (!EndTime.HasValue || !m_Info.Duration.HasValue)
                {
                    return null;
                }

                DateTime? interviewTime = new DateTime(1, 1, 1).AddSeconds(m_Info.Duration.Value);

                return new DateTime(EndTime.Value.Ticks - interviewTime.Value.Ticks);
            }
        }

        public DateTime? EndTime
        {
            get
            {
                return ProcessDateTime(m_Info.EndTime);
            }
        }

        public int? ITS
        {
            get { return m_Info.ITS_ID; }
        }

        public string TransientState
        {
            get { return m_Info.TransientState; }
        }

        public string Person
        {
            get { return string.IsNullOrEmpty(m_Info.Person) && m_Info.Role == "Web-respondents" ? "Web Respondent" : m_Info.Person; }
        }

        public string Role
        {
            get { return m_Info.Role; }
        }

        public string TelNumber => _isNeedToHidePii ? "***" : m_Info.TelephoneNumber;

        public string Respondent => _isNeedToHidePii ? "***" : m_Info.RespondentName;

        public int? TimeZoneId
        {
            get { return m_Info.TimeZoneID; }
        }

        public string TimeZone
        {
            get { return m_Info.TimeZone; }
        }

        public DateTime? WaitingTime
        {
            get
            {
                return m_Info.WaitingTime.HasValue
                           ? new DateTime(1, 1, 1).AddSeconds(m_Info.WaitingTime.Value)
                           : (DateTime?)null;
            }
        }

        public DateTime? InterviewTime
        {
            get
            {
                return m_Info.Duration.HasValue
                           ? new DateTime(1, 1, 1).AddSeconds(m_Info.Duration.Value)
                           : (DateTime?)null;
            }
        }

        public DateTime? ReviewTime
        {
            get
            {
                return m_Info.OpenEndReviewDuration.HasValue
                    ? new DateTime(1, 1, 1).AddSeconds(m_Info.OpenEndReviewDuration.Value)
                    : (DateTime?)null;
            }
        }

        public DateTime? PreviewTime
        {
            get
            {
                return m_Info.PreviewTime.HasValue
                    ? new DateTime(1, 1, 1).AddSeconds(m_Info.PreviewTime.Value)
                    : (DateTime?)null;
            }
        }

        public DateTime? ConnectedTime
        {
            get
            {
                return m_Info.ConnectedTime.HasValue
                    ? new DateTime(1, 1, 1).AddSeconds(m_Info.ConnectedTime.Value)
                    : (DateTime?)null;
            }
        }

        public DateTime? WrapTime
        {
            get
            {
                return m_Info.WrapTime.HasValue
                    ? new DateTime(1, 1, 1).AddSeconds(m_Info.WrapTime.Value)
                    : (DateTime?)null;
            }
        }

        public string ContactName
        {
            get { return m_Info.ContactName; }
        }

        public DateTime? TimeToCall
        {
            get
            {
                return ProcessDateTime(m_Info.TimeToCall);
            }
        }

        public DateTime? TimeToExpire
        {
            get
            {
                return ProcessDateTime(m_Info.TimeToExpire);
            }
        }

        public string CallCenterName
        {
            get { return m_Info.CallCenterName; }
        }

        public int? LinkedInterviewSessionId
        {
            get { return m_Info.LinkedInterviewSessionId; }
        }

        /// <summary>
        /// Replaces FusionDateEmpty with null and converts DateTime to local TZ.
        /// </summary>
        private DateTime? ProcessDateTime(DateTime? dateTime)
        {
            if (dateTime != null)
            {
                dateTime = _timezoneProvider.ConvertToLocalTime(dateTime.Value);
            }

            return dateTime;
        }

        public int? CallAttemptNumber
        {
            get { return m_Info.CallAttemptNumber; }
        }
    }
}