using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public interface IConsoleLoginToDialerProcessor
    {
        BvSurveyEntity LoginToDialer(
            BvPersonEntity person,
            BvTasksEntity task,
            string extensionNumber,
            BvSurveyEntity survey,
            out bool isPredictive);
    }
}