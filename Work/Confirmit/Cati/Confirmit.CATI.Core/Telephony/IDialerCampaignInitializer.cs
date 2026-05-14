using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Telephony
{
    public interface IDialerCampaignInitializer
    {
        /// <summary>
        /// Initializes campaigns for all open surveys.
        /// </summary>
        void InitializeAllCampaigns();

        /// <summary>
        /// This method is called on survey open
        /// </summary>
        ICollection<DialerStartCampaignResult> OpenSurveyOnDialerIfNeeded(
            string surveyName,
            long dialingMode);

        void ApplyDefaultSurveyDialerParametersToSurveyIfNeeded(BvSurveyEntity surveyEntity);
    }
}