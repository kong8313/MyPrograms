using System.Collections.Generic;
using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Telephony;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations
{
    public class DialerOperation : IDialerOperation
    {
        private readonly IMnTciTools _mnTciTools;
        private readonly ITelephony _telephony;

        public DialerOperation(
            IMnTciTools mnTciTools,
            ITelephony telephony)
        {
            _mnTciTools = mnTciTools;
            _telephony = telephony;
        }

        public void FlushCallsIfNeeded(BvSurveyEntity surveyEntity, List<CallInfo> callsToFlush)
        {
            if ((callsToFlush == null) || (callsToFlush.Count == 0))
            {
                return;
            }

            if (!_mnTciTools.DoesCompanyUseTelephony())
            {
                return;
            }

            if ((DialingMode)surveyEntity.DialMode != DialingMode.Predictive)
            {
                // Flush calls operation makes sense for predictive dial mode only
                return;
            }

            _telephony.FlushNumbers(surveyEntity.CampaignId, callsToFlush);
        }
    }
}