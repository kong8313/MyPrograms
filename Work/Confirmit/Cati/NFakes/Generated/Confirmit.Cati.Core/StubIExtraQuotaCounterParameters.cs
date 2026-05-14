using System;
using Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation;

namespace Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation.Fakes
{
    public class StubIExtraQuotaCounterParameters : IExtraQuotaCounterParameters 
    {
        private IExtraQuotaCounterParameters _inner;

        public StubIExtraQuotaCounterParameters()
        {
            _inner = null;
        }

        public IExtraQuotaCounterParameters Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _SurveyId;
        public Func<int> SurveyIdGet;
        public Action<int> SurveyIdSetInt32;

        int IExtraQuotaCounterParameters.SurveyId
        {
            get
            {
                if (SurveyIdGet != null)
                {
                    return SurveyIdGet();
                } else if (_inner != null)
                {
                    return ((IExtraQuotaCounterParameters)_inner).SurveyId;
                }

                if (SurveyIdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SurveyId;
                }

                return default(int);
            }

        }

        private string[] _QuotaFields;
        public Func<string[]> QuotaFieldsGet;
        public Action<string[]> QuotaFieldsSetArrayOfString;

        string[] IExtraQuotaCounterParameters.QuotaFields
        {
            get
            {
                if (QuotaFieldsGet != null)
                {
                    return QuotaFieldsGet();
                } else if (_inner != null)
                {
                    return ((IExtraQuotaCounterParameters)_inner).QuotaFields;
                }

                if (QuotaFieldsSetArrayOfString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _QuotaFields;
                }

                return default(string[]);
            }

        }

    }
}