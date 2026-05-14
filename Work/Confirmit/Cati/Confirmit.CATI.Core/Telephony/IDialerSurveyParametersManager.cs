using System.Collections.Generic;
using DialerCommon.DialerParameters;

namespace Confirmit.CATI.Core.Telephony
{
    public interface IDialerSurveyParametersManager
    {
        bool DoesDialerHaveSurveyParameters { get; }

        IEnumerable<DialerParameter> GetDialerDefaultSurveyParameters();
        string GetDialerDefaultSurveyParametersAsXml();
        IEnumerable<DialerParameter> GetDialerSurveyParameters(int surveySid);
        void ResetSurveyDialerParametersToDefaultValues(int surveySid);
        void SetDialerDefaultSurveyParameters(IEnumerable<DialerParameter> parameters);
        void SetDialerSurveyParameters(int surveySid, IEnumerable<DialerParameter> parameters);
        void ValidateDialerSurveyParameters(IEnumerable<DialerParameter> parameters);
    }
}