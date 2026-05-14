using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Services.Survey;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.DAL.Generated.Entity.Table
{
    public partial class BvSurveyEntity
    {
        public string ProjectId
        {
            get
            {
                return Name;
            }
        }

        public long CampaignId
        {
            get
            {
                return ProjectIdConverter.ProjectIdToCampaignId(ProjectId);
            }
        }

        public bool IsWholeInterviewRecordingEnabled { get { return RecWholeInt != 0; } }

        public DialingMode DialingMode
        {
            get
            {
                return (DialingMode)DialMode;
            }
        }

        public string LogInfo
        {
            get { return string.Format("{0}({1})-'{2}'", Name, SID, Description); }
        }

        public InboundSurveyBehavior InboundBehavior
        {
            get
            {
                return (InboundSurveyBehavior)InboundCallBehavior;
            }

            set
            {
                InboundCallBehavior = (byte)value;
            }
        }
    }
}
