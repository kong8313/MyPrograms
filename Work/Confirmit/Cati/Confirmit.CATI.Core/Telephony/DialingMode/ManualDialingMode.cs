using System.Collections.Generic;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.DialingWorkflow
{
    public class ManualDialingMode : DialingMode
    {
        public ManualDialingMode() : base(ConfirmitDialerInterface.DialingMode.Manual)
        {
            
        }

        public override void CheckPersonCanLoginToDialer(BvPersonEntity person)
        {
            // We must not login to dialer in this case, but suggest user either to continue without dialer or logout
            throw new SurveyInManualDialingModeException(
                "LoginToDialer. This survey is set to be dialed manually so it is not possible to log into the dialer.");
        }

        public override DialerErrorCode Login(BvPersonEntity person, BvTasksEntity task, BvSurveyEntity survey, string extensionNumber, IEnumerable<KeyValuePair<string, string>> personDialerAttributes)
        {
            CheckPersonCanLoginToDialer(person);

            return DialerErrorCode.Exception;
        }
    }
}
