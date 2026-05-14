using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public interface IConsoleDialProcessor
    {
        bool Dial(BvPersonEntity person, BvTasksEntity task, string phoneNumber, int attemptNumber, DialEvent activityEvent);
    }
}