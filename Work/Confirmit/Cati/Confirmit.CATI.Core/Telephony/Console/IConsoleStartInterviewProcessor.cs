using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public interface IConsoleStartInterviewProcessor
    {
        BvSurveyEntity Startinterview(BvPersonEntity person, BvTasksEntity task, string surveyId, int interviewId, StartInterviewEvent activityEvent);
    }
}