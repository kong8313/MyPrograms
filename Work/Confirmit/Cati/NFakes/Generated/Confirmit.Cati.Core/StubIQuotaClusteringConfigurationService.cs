using System;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIQuotaClusteringConfigurationService : IQuotaClusteringConfigurationService 
    {
        private IQuotaClusteringConfigurationService _inner;

        public StubIQuotaClusteringConfigurationService()
        {
            _inner = null;
        }

        public IQuotaClusteringConfigurationService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate QuotaClusteringConfiguration GetConfigurationInt32Delegate(int surveyId);
        public GetConfigurationInt32Delegate GetConfigurationInt32;

        QuotaClusteringConfiguration IQuotaClusteringConfigurationService.GetConfiguration(int surveyId)
        {


            if (GetConfigurationInt32 != null)
            {
                return GetConfigurationInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IQuotaClusteringConfigurationService)_inner).GetConfiguration(surveyId);
            }

            return default(QuotaClusteringConfiguration);
        }

        public delegate void ConfigureInt32QuotaClusteringConfigurationDelegate(int surveyId, QuotaClusteringConfiguration configuration);
        public ConfigureInt32QuotaClusteringConfigurationDelegate ConfigureInt32QuotaClusteringConfiguration;

        void IQuotaClusteringConfigurationService.Configure(int surveyId, QuotaClusteringConfiguration configuration)
        {

            if (ConfigureInt32QuotaClusteringConfiguration != null)
            {
                ConfigureInt32QuotaClusteringConfiguration(surveyId, configuration);
            } else if (_inner != null)
            {
                ((IQuotaClusteringConfigurationService)_inner).Configure(surveyId, configuration);
            }
        }

        public delegate void ResetInt32Delegate(int surveyId);
        public ResetInt32Delegate ResetInt32;

        void IQuotaClusteringConfigurationService.Reset(int surveyId)
        {

            if (ResetInt32 != null)
            {
                ResetInt32(surveyId);
            } else if (_inner != null)
            {
                ((IQuotaClusteringConfigurationService)_inner).Reset(surveyId);
            }
        }

        public delegate bool IsEnabledInt32Delegate(int surveyId);
        public IsEnabledInt32Delegate IsEnabledInt32;

        bool IQuotaClusteringConfigurationService.IsEnabled(int surveyId)
        {


            if (IsEnabledInt32 != null)
            {
                return IsEnabledInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IQuotaClusteringConfigurationService)_inner).IsEnabled(surveyId);
            }

            return default(bool);
        }

        public delegate bool IsEnabledBvSurveyEntityDelegate(BvSurveyEntity survey);
        public IsEnabledBvSurveyEntityDelegate IsEnabledBvSurveyEntity;

        bool IQuotaClusteringConfigurationService.IsEnabled(BvSurveyEntity survey)
        {


            if (IsEnabledBvSurveyEntity != null)
            {
                return IsEnabledBvSurveyEntity(survey);
            } else if (_inner != null)
            {
                return ((IQuotaClusteringConfigurationService)_inner).IsEnabled(survey);
            }

            return default(bool);
        }

    }
}