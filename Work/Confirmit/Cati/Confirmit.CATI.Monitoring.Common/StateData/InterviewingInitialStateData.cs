using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    /// <summary>
    /// Represents initial state data of interviewing form as a whole. Contains state of WebBrowser and KeyboardInputControl.
    /// </summary>
    [Serializable]
    public class InterviewingInitialStateData : BaseStateData
    {
        public InterviewingInitialStateData()
            : base()
        {
        }

        public string PageContent
        {
            get;
            set;
        }

        public string KeyboardInputValue
        {
            get;
            set;
        }
        public int ActiveQuestionIndex
        {
            get;
            set;
        }

        public int InterviewId
        {
            get;
            set;
        }

        public string SurveyId
        {
            get;
            set;
        }
        public string SurveyName
        {
            get;
            set;
        }

        public ConsoleState ConsoleState
        {
            get;
            set;
        }

        public bool IsNewSurvey
        {
            get; 
            set;
        }

        public bool IsTestMode
        {
            get;
            set;
        }

        public bool IsInboundCall
        {
            get;
            set;
        }
    }
}
