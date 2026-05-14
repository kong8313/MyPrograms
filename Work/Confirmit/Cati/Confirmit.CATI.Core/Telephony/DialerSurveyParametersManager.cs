using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;

using DialerCommon.DialerParameters;

namespace Confirmit.CATI.Core.Telephony
{
    public class DialerSurveyParametersManager : IDialerSurveyParametersManager
    {
        private readonly ITelephony _telephony;
        private readonly IDialerSettings _dialerSettings;
        private readonly IDialerCampaignInitializer _dialerCampaignInitializer;

        public DialerSurveyParametersManager(
            ITelephony telephony,
            IDialerSettings dialerSettings,
            IDialerCampaignInitializer dialerCampaignInitializer)
        {
            _telephony = telephony;
            _dialerSettings = dialerSettings;
            _dialerCampaignInitializer = dialerCampaignInitializer;
        }

        /// <summary>
        /// Returns if dialer has survey parameters.
        /// </summary>
        public bool DoesDialerHaveSurveyParameters
        {
            get
            {
                return GetDialerDefaultSurveyParameters().Any();
            }
        }

        /// <summary>
        /// Gets dialer default survey parameters.
        /// </summary>
        /// <returns>Returns the list of default dialer survey parameters,
        /// i.e. survey level dialer parameters set by default to all new surveys</returns>
        public IEnumerable<DialerParameter> GetDialerDefaultSurveyParameters()
        {
            var defaultSurveyParameters = _dialerSettings.DefaultSurveyParameters;
            return DialerParametersSerializer.DeserializeDialerParameters(defaultSurveyParameters)
                .ToList();
        }

        /// <summary>
        /// Gets dialer default survey parameters as xml (i.e. as is from DB).
        /// </summary>
        /// <returns>Returns default dialer survey parameters as unparsed xml</returns>
        public string GetDialerDefaultSurveyParametersAsXml()
        {
            return _dialerSettings.DefaultSurveyParameters;
        }

        /// <summary>
        /// Sets default dialer survey parameters.
        /// </summary>
        /// <param name="parameters">parameters to set</param>
        public void SetDialerDefaultSurveyParameters(IEnumerable<DialerParameter> parameters)
        {
            var xmlParametersString = DialerParametersSerializer.SerializeDialerParameters(parameters);

            _dialerSettings.DefaultSurveyParameters = xmlParametersString;
        }

        /// <summary>
        /// Validates dialer survey parameters.
        /// </summary>
        /// <param name="parameters">parameters to validate</param>
        public void ValidateDialerSurveyParameters(IEnumerable<DialerParameter> parameters)
        {
            var xmlParametersString = DialerParametersSerializer.SerializeDialerParameters(parameters);
            _telephony.ValidateCampaignParameters(xmlParametersString);
        }

        /// <summary>
        /// Gets dialer parameters for the specified survey.
        /// </summary>
        /// <param name="surveySid">
        /// The survey Sid.
        /// </param>
        /// <returns>
        /// Returns the list of dialer survey parameters for the survey
        /// </returns>
        public IEnumerable<DialerParameter> GetDialerSurveyParameters(int surveySid)
        {
            if (!DoesDialerHaveSurveyParameters)
            {
                return new List<DialerParameter>();
            }

            var surveyEntity = SurveyRepository.GetById(surveySid);
            var surveyParametersXml = surveyEntity.DialerParameters;

            if (string.IsNullOrEmpty(surveyParametersXml))
            {
                _dialerCampaignInitializer.ApplyDefaultSurveyDialerParametersToSurveyIfNeeded(surveyEntity);
                surveyParametersXml = surveyEntity.DialerParameters;
            }

            return DialerParametersSerializer.DeserializeDialerParameters(surveyParametersXml).ToList();
        }

        /// <summary>
        /// Sets dialer parameters for the specified survey.
        /// </summary>
        /// <param name="surveySid">
        /// The survey Sid.
        /// </param>
        /// <param name="parameters">
        /// List of parameters to set
        /// </param>
        public void SetDialerSurveyParameters(int surveySid, IEnumerable<DialerParameter> parameters)
        {
            var xmlParametersString = DialerParametersSerializer.SerializeDialerParameters(parameters);

            BvSurveyEntity survey;
            using (var transaction = new DatabaseTransactionScope("SetDialerSurveyParams", DeadlockPriority.Supervisor))
            {
                survey = SurveyRepository.GetById(surveySid);
                survey.DialerParameters = xmlParametersString;

                SurveyRepository.Update(survey);

                transaction.Commit();
            }

            // For open surveys we need immediately apply new parameters
            if (survey.State == (int)SurveyState.Open)
            {
                _telephony.SetCampaignParameters(survey.CampaignId, survey.DialingMode, xmlParametersString);
            }
        }

        /// <summary>
        /// Resets dialer survey parameters values for the specified survey to default values.
        /// </summary>
        /// <param name="surveySid">
        /// The survey Sid.
        /// </param>
        public void ResetSurveyDialerParametersToDefaultValues(
            int surveySid)
        {
            var defaultDialerSurveyParameters = ServiceLocator.Resolve<ISystemSettings>().Dialer.DefaultSurveyParameters;

            var survey = SurveyRepository.GetById(surveySid);
            survey.DialerParameters = defaultDialerSurveyParameters;
            SurveyRepository.Update(survey);
        }
    }
}
