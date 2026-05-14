using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;
using DialerCommon;

namespace Confirmit.CATI.Core.Telephony
{
    public interface IDialerFacilities
    {
        string GetDialerVersion(int dialerId);

        DialerAvailableExtendedFunctionality GetAvailableExtendedFunctionality(int dialerId);

        IEnumerable<LogFileInfo> GetLogFiles(int dialerId);
        byte[] GetLogFileBodyZipped(int dialerId, string fileName);

        DialerFeatures GetDialerSupportedFeatures(int dialerId);

        IEnumerable<DialerOverridenFeature> GetOverridenDialerSupportedFeatures(int dialerId);

        void UpdateOverridenDialerSupportedFeature(int dialerId, string featureName, bool? overridenFeatureValue);
    }
}