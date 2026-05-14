using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.Telephony.Console;

namespace Confirmit.CATI.Core.Telephony.Console.Fakes
{
    public class StubIConsoleStartInterviewProcessor : IConsoleStartInterviewProcessor 
    {
        private IConsoleStartInterviewProcessor _inner;

        public StubIConsoleStartInterviewProcessor()
        {
            _inner = null;
        }

        public IConsoleStartInterviewProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvSurveyEntity StartinterviewBvPersonEntityBvTasksEntityStringInt32StartInterviewEventDelegate(BvPersonEntity person, BvTasksEntity task, string surveyId, int interviewId, StartInterviewEvent activityEvent);
        public StartinterviewBvPersonEntityBvTasksEntityStringInt32StartInterviewEventDelegate StartinterviewBvPersonEntityBvTasksEntityStringInt32StartInterviewEvent;

        BvSurveyEntity IConsoleStartInterviewProcessor.Startinterview(BvPersonEntity person, BvTasksEntity task, string surveyId, int interviewId, StartInterviewEvent activityEvent)
        {


            if (StartinterviewBvPersonEntityBvTasksEntityStringInt32StartInterviewEvent != null)
            {
                return StartinterviewBvPersonEntityBvTasksEntityStringInt32StartInterviewEvent(person, task, surveyId, interviewId, activityEvent);
            } else if (_inner != null)
            {
                return ((IConsoleStartInterviewProcessor)_inner).Startinterview(person, task, surveyId, interviewId, activityEvent);
            }

            return default(BvSurveyEntity);
        }

    }
}