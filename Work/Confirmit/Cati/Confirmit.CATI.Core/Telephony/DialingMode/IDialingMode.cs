using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.DialingWorkflow
{
    public interface IDialingMode
    {
        DialerErrorCode Login(BvPersonEntity person, BvTasksEntity task, BvSurveyEntity survey, string extensionNumber, IEnumerable<KeyValuePair<string, string>> personDialerAttributes);

        void BeforeStartInterview(BvTasksEntity task, BvPersonEntity person);

        void StartInterview(
            int personId,
            int dialerId,
            BvSurveyEntity survey,
            BvInterviewEntity interview,
            int timezoneId);

        void CheckPersonCanLoginToDialer(BvPersonEntity person);
    }
}
