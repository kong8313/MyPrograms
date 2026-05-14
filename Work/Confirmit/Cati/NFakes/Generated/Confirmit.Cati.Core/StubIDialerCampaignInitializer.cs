using System;
using Confirmit.CATI.Core.Telephony;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubIDialerCampaignInitializer : IDialerCampaignInitializer 
    {
        private IDialerCampaignInitializer _inner;

        public StubIDialerCampaignInitializer()
        {
            _inner = null;
        }

        public IDialerCampaignInitializer Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InitializeAllCampaignsDelegate();
        public InitializeAllCampaignsDelegate InitializeAllCampaigns;

        void IDialerCampaignInitializer.InitializeAllCampaigns()
        {

            if (InitializeAllCampaigns != null)
            {
                InitializeAllCampaigns();
            } else if (_inner != null)
            {
                ((IDialerCampaignInitializer)_inner).InitializeAllCampaigns();
            }
        }

        public delegate ICollection<DialerStartCampaignResult> OpenSurveyOnDialerIfNeededStringInt64Delegate(string surveyName, long dialingMode);
        public OpenSurveyOnDialerIfNeededStringInt64Delegate OpenSurveyOnDialerIfNeededStringInt64;

        ICollection<DialerStartCampaignResult> IDialerCampaignInitializer.OpenSurveyOnDialerIfNeeded(string surveyName, long dialingMode)
        {


            if (OpenSurveyOnDialerIfNeededStringInt64 != null)
            {
                return OpenSurveyOnDialerIfNeededStringInt64(surveyName, dialingMode);
            } else if (_inner != null)
            {
                return ((IDialerCampaignInitializer)_inner).OpenSurveyOnDialerIfNeeded(surveyName, dialingMode);
            }

            return default(ICollection<DialerStartCampaignResult>);
        }

        public delegate void ApplyDefaultSurveyDialerParametersToSurveyIfNeededBvSurveyEntityDelegate(BvSurveyEntity surveyEntity);
        public ApplyDefaultSurveyDialerParametersToSurveyIfNeededBvSurveyEntityDelegate ApplyDefaultSurveyDialerParametersToSurveyIfNeededBvSurveyEntity;

        void IDialerCampaignInitializer.ApplyDefaultSurveyDialerParametersToSurveyIfNeeded(BvSurveyEntity surveyEntity)
        {

            if (ApplyDefaultSurveyDialerParametersToSurveyIfNeededBvSurveyEntity != null)
            {
                ApplyDefaultSurveyDialerParametersToSurveyIfNeededBvSurveyEntity(surveyEntity);
            } else if (_inner != null)
            {
                ((IDialerCampaignInitializer)_inner).ApplyDefaultSurveyDialerParametersToSurveyIfNeeded(surveyEntity);
            }
        }

    }
}