using System;
using Confirmit.CATI.Supervisor.Core.Surveys;
using System.Data;

namespace Confirmit.CATI.Supervisor.Core.Surveys.Fakes
{
    public class StubISurveyQuotasExportInfoProvider : ISurveyQuotasExportInfoProvider 
    {
        private ISurveyQuotasExportInfoProvider _inner;

        public StubISurveyQuotasExportInfoProvider()
        {
            _inner = null;
        }

        public ISurveyQuotasExportInfoProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string[] GetQuotaNamesDelegate();
        public GetQuotaNamesDelegate GetQuotaNames;

        string[] ISurveyQuotasExportInfoProvider.GetQuotaNames()
        {


            if (GetQuotaNames != null)
            {
                return GetQuotaNames();
            } else if (_inner != null)
            {
                return ((ISurveyQuotasExportInfoProvider)_inner).GetQuotaNames();
            }

            return default(string[]);
        }

        public delegate DataTable GetQuotaInfoStringDelegate(string quotaName);
        public GetQuotaInfoStringDelegate GetQuotaInfoString;

        DataTable ISurveyQuotasExportInfoProvider.GetQuotaInfo(string quotaName)
        {


            if (GetQuotaInfoString != null)
            {
                return GetQuotaInfoString(quotaName);
            } else if (_inner != null)
            {
                return ((ISurveyQuotasExportInfoProvider)_inner).GetQuotaInfo(quotaName);
            }

            return default(DataTable);
        }

        private int _SurveyId;
        public Func<int> SurveyIdGet;
        public Action<int> SurveyIdSetInt32;

        int ISurveyQuotasExportInfoProvider.SurveyId
        {
            get
            {
                if (SurveyIdGet != null)
                {
                    return SurveyIdGet();
                } else if (_inner != null)
                {
                    return ((ISurveyQuotasExportInfoProvider)_inner).SurveyId;
                }

                if (SurveyIdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SurveyId;
                }

                return default(int);
            }

        }

    }
}