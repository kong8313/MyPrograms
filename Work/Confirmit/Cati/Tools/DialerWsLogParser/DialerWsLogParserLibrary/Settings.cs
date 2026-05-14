using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace DialerWsLogParserLibrary
{
    [DataContract]
    public class Settings
    {
        [DataMember]
        public bool IsConditionalOperatorAnd { get; private set; }
        [DataMember]
        public bool IsCoincidenceOperatorPositive { get; private set; }
        [DataMember]
        public GroupsFilter Filter { get; private set; }
        [DataMember]
        public ColumnDisplayHandler ColumnHandler { get; private set; }
        [DataMember]
        public List<string> RecentFiles { get; private set; }

        public Settings()
        {
            IsConditionalOperatorAnd = true;
            IsCoincidenceOperatorPositive = true;
        }

        public void SetColumnsFilter (string name, string startTime, string finishTime, string companyId, string dialerId, string campaignId, string agentId,
            string callId, string interviewId, string duration, string allInfo)
        {
            Filter = new GroupsFilter(name, startTime, finishTime, companyId, dialerId, campaignId, agentId,
                callId, interviewId, duration, allInfo);
        }

        public void SetColumnsVisibility (bool startTime, bool finishTime, bool companyId, bool dialerId, bool campaignId, bool agentId,
            bool callId, bool interviewId, bool duration)
        {
            ColumnHandler = new ColumnDisplayHandler(startTime, finishTime, companyId, dialerId, campaignId, agentId, callId, interviewId, duration);
        }

        public void SetRecentFiles (List<string> files)
        {
            RecentFiles = files;
        }

        public bool IsParametersMatchCondition(EventsGroup entry)
        {
            if (IsConditionalOperatorAnd)
                return IsParameterSatisfyCondition(entry.Name, Filter.Name) &&
                       IsStartTimeSatisfyCondition(entry.StartTime, Filter.StartTime) &&
                       IsFinishTimeSatisfyCondition(entry.FinishTime, Filter.FinishTime) &&
                       IsParameterSatisfyCondition(entry.CompanyId, Filter.CompanyId) &&
                       IsParameterSatisfyCondition(entry.DialerId, Filter.DialerId) &&
                       IsParameterSatisfyCondition(entry.CampaignId, Filter.CampaignId) &&
                       IsParameterSatisfyCondition(entry.AgentId, Filter.AgentId) &&
                       IsParameterSatisfyCondition(entry.CallId, Filter.CallId) &&
                       IsParameterSatisfyCondition(entry.InterviewId, Filter.InterviewId) &&
                       IsParameterSatisfyCondition(entry.Duration, Filter.Duration);
            else
                return Filter.Name == string.Empty && Filter.StartTime == string.Empty && Filter.FinishTime == string.Empty &&
                       Filter.CompanyId == string.Empty && Filter.DialerId == string.Empty && Filter.CampaignId == string.Empty &&
                       Filter.AgentId == string.Empty && Filter.CallId == string.Empty && Filter.InterviewId == string.Empty && 
                       Filter.Duration == string.Empty ||

                       IsParameterSatisfyCondition(entry.Name, Filter.Name) ||
                       IsStartTimeSatisfyCondition(entry.StartTime, Filter.StartTime) ||
                       IsFinishTimeSatisfyCondition(entry.FinishTime, Filter.FinishTime) ||
                       IsParameterSatisfyCondition(entry.CompanyId, Filter.CompanyId) ||
                       IsParameterSatisfyCondition(entry.DialerId, Filter.DialerId) ||
                       IsParameterSatisfyCondition(entry.CampaignId, Filter.CampaignId) ||
                       IsParameterSatisfyCondition(entry.AgentId, Filter.AgentId) ||
                       IsParameterSatisfyCondition(entry.CallId, Filter.CallId) ||
                       IsParameterSatisfyCondition(entry.InterviewId, Filter.InterviewId) ||
                       IsParameterSatisfyCondition(entry.Duration, Filter.Duration);
        }

        public void SetConditionalOperatorAnd()
        {
            IsConditionalOperatorAnd = true;
        }

        public void SetConditionalOperatorOr()
        {
            IsConditionalOperatorAnd = false;
        }

        public void SetCoincidenceOperatorPos()
        {
            IsCoincidenceOperatorPositive = true;
        }

        public void SetCoincidenceOperatorNeg()
        {
            IsCoincidenceOperatorPositive = false;
        }

        private bool IsParameterSatisfyCondition(string entryParameter, string textBoxParameter)
        {
            Regex regex = new Regex(textBoxParameter);

            if (IsConditionalOperatorAnd)
            {
                if (IsCoincidenceOperatorPositive)
                    return textBoxParameter == string.Empty || regex.IsMatch(entryParameter);
                else
                    return textBoxParameter == string.Empty || !regex.IsMatch(entryParameter);
            }
            else
            {
                if (IsCoincidenceOperatorPositive)
                    return textBoxParameter != string.Empty && regex.IsMatch(entryParameter);
                else
                    return textBoxParameter != string.Empty && !regex.IsMatch(entryParameter);
            }
                
        }

        private bool IsStartTimeSatisfyCondition(string entryTime, string textBoxTime)
        {
            if (IsConditionalOperatorAnd)
            {
                if (IsCoincidenceOperatorPositive)
                    return textBoxTime == string.Empty || string.Compare(textBoxTime, entryTime) <= 0;
                else
                    return textBoxTime == string.Empty || !(string.Compare(textBoxTime, entryTime) <= 0);
            }
            else
            {
                if (IsCoincidenceOperatorPositive)
                    return textBoxTime != string.Empty && string.Compare(textBoxTime, entryTime) <= 0;
                else
                    return textBoxTime != string.Empty && !(string.Compare(textBoxTime, entryTime) <= 0);
            }
        }

        private bool IsFinishTimeSatisfyCondition(string entryTime, string textBoxTime)
        {
            if (IsConditionalOperatorAnd)
            {
                if (IsCoincidenceOperatorPositive)
                    return textBoxTime == string.Empty || string.Compare(textBoxTime, entryTime) >= 0;
                else
                    return textBoxTime == string.Empty || !(string.Compare(textBoxTime, entryTime) >= 0);
            }
            else
            {
                if (IsCoincidenceOperatorPositive)
                    return textBoxTime != string.Empty && string.Compare(textBoxTime, entryTime) >= 0;
                else
                    return textBoxTime != string.Empty && !(string.Compare(textBoxTime, entryTime) >= 0);
            }
        }
    }
}
