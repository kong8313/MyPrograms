using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    public class DialerCampaignInitializer : IDialerCampaignInitializer
    {
        private readonly ITelephony _telephony;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IDialerSettings _dialerSettings;
        private readonly IMnTciTools _mnTciTools;

        public DialerCampaignInitializer(
            ITelephony telephony,
            ISurveyRepository surveyRepository,
            IDialerSettings dialerSettings,
            IMnTciTools mnTciTools)
        {
            _telephony = telephony;
            _surveyRepository = surveyRepository;
            _dialerSettings = dialerSettings;
            _mnTciTools = mnTciTools;
        }

        /// <summary>
        /// Initializes campaigns for all open surveys.
        /// </summary>
        public void InitializeAllCampaigns()
        {
            Trace.TraceInformation("InitializeAllCampaigns ...");

            var openedSurveys = SurveyService.OpenedSurveys;

            foreach (var openedSurvey in openedSurveys)
            {
                var survey = _surveyRepository.GetById(openedSurvey.SID.Value);

                try
                {
                    OpenSurveyOnDialerIfNeeded(
                        survey.Name,
                        survey.DialMode);
                }
                catch (UserMessageException)
                {
                    // We need not show error messages here: while all campaign initialization
                    continue;
                }
            }
        }

        /// <summary>
        /// This method is called on survey open
        /// </summary>
        public ICollection<DialerStartCampaignResult> OpenSurveyOnDialerIfNeeded(
            string surveyName,
            long dialingMode)
        {
            var result = new List<DialerStartCampaignResult>();

            if ((DialingMode)dialingMode == DialingMode.Manual)
            {
                return result;
            }

            var surveyEntity = _surveyRepository.GetByName(surveyName);

            if (!_mnTciTools.DoesCompanyUseTelephony())
            {
                Trace.TraceWarning("BvCallHandlerRoot.OpenSurveyOnDialerIfNeeded: Dialing mode [{0}] is not 'Manual' but company does not use Telephony /// " +
                                   "SurveyName={1}, SurveySID={2}, CompanyId={3}, CompanyName={4}",
                    dialingMode,
                    surveyName, surveyEntity.SID,
                    BackendInstance.Current.CompanyId, BackendInstance.Current.CompanyName);

                return result;
            }

            const string campaignType = "0"; // 0 means CAMPAIGN_TYPE_OUTBOUND (see Mn docs for details)

            ApplyDefaultSurveyDialerParametersToSurveyIfNeeded(surveyEntity);

            return _telephony.StartCampaign(
                surveyEntity.CampaignId,
                surveyEntity.Description,
                (DialingMode)dialingMode,
                campaignType,
                surveyEntity.DialerParameters);
        }

        public void ApplyDefaultSurveyDialerParametersToSurveyIfNeeded(BvSurveyEntity surveyEntity)
        {
            string defaultDialerParameters = _dialerSettings.DefaultSurveyParameters;

            if (surveyEntity.DialerParameters == null && defaultDialerParameters != null)
            {
                surveyEntity.DialerParameters = defaultDialerParameters;
                SurveyRepository.Update(surveyEntity);
                Trace.TraceWarning(
                    "BvCallHandlerRoot.ApplyDefaultSurveyDialerParametersToSurveyIfNeeded: survey dialer parameters are null, " +
                    "so they are initialized with default survey dialer parameters." +
                    " /// surveySid={0}, surveyName={1}.",
                    surveyEntity.SID,
                    surveyEntity.Name);
            }
        }
    }
}