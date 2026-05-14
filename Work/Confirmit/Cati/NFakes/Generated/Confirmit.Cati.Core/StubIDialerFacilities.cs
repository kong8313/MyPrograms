using System;
using Confirmit.CATI.Core.Telephony;
using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;
using DialerCommon;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubIDialerFacilities : IDialerFacilities 
    {
        private IDialerFacilities _inner;

        public StubIDialerFacilities()
        {
            _inner = null;
        }

        public IDialerFacilities Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetDialerVersionInt32Delegate(int dialerId);
        public GetDialerVersionInt32Delegate GetDialerVersionInt32;

        string IDialerFacilities.GetDialerVersion(int dialerId)
        {


            if (GetDialerVersionInt32 != null)
            {
                return GetDialerVersionInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerFacilities)_inner).GetDialerVersion(dialerId);
            }

            return default(string);
        }

        public delegate DialerAvailableExtendedFunctionality GetAvailableExtendedFunctionalityInt32Delegate(int dialerId);
        public GetAvailableExtendedFunctionalityInt32Delegate GetAvailableExtendedFunctionalityInt32;

        DialerAvailableExtendedFunctionality IDialerFacilities.GetAvailableExtendedFunctionality(int dialerId)
        {


            if (GetAvailableExtendedFunctionalityInt32 != null)
            {
                return GetAvailableExtendedFunctionalityInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerFacilities)_inner).GetAvailableExtendedFunctionality(dialerId);
            }

            return default(DialerAvailableExtendedFunctionality);
        }

        public delegate IEnumerable<LogFileInfo> GetLogFilesInt32Delegate(int dialerId);
        public GetLogFilesInt32Delegate GetLogFilesInt32;

        IEnumerable<LogFileInfo> IDialerFacilities.GetLogFiles(int dialerId)
        {


            if (GetLogFilesInt32 != null)
            {
                return GetLogFilesInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerFacilities)_inner).GetLogFiles(dialerId);
            }

            return default(IEnumerable<LogFileInfo>);
        }

        public delegate byte[] GetLogFileBodyZippedInt32StringDelegate(int dialerId, string fileName);
        public GetLogFileBodyZippedInt32StringDelegate GetLogFileBodyZippedInt32String;

        byte[] IDialerFacilities.GetLogFileBodyZipped(int dialerId, string fileName)
        {


            if (GetLogFileBodyZippedInt32String != null)
            {
                return GetLogFileBodyZippedInt32String(dialerId, fileName);
            } else if (_inner != null)
            {
                return ((IDialerFacilities)_inner).GetLogFileBodyZipped(dialerId, fileName);
            }

            return default(byte[]);
        }

        public delegate DialerFeatures GetDialerSupportedFeaturesInt32Delegate(int dialerId);
        public GetDialerSupportedFeaturesInt32Delegate GetDialerSupportedFeaturesInt32;

        DialerFeatures IDialerFacilities.GetDialerSupportedFeatures(int dialerId)
        {


            if (GetDialerSupportedFeaturesInt32 != null)
            {
                return GetDialerSupportedFeaturesInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerFacilities)_inner).GetDialerSupportedFeatures(dialerId);
            }

            return default(DialerFeatures);
        }

        public delegate IEnumerable<DialerOverridenFeature> GetOverridenDialerSupportedFeaturesInt32Delegate(int dialerId);
        public GetOverridenDialerSupportedFeaturesInt32Delegate GetOverridenDialerSupportedFeaturesInt32;

        IEnumerable<DialerOverridenFeature> IDialerFacilities.GetOverridenDialerSupportedFeatures(int dialerId)
        {


            if (GetOverridenDialerSupportedFeaturesInt32 != null)
            {
                return GetOverridenDialerSupportedFeaturesInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerFacilities)_inner).GetOverridenDialerSupportedFeatures(dialerId);
            }

            return default(IEnumerable<DialerOverridenFeature>);
        }

        public delegate void UpdateOverridenDialerSupportedFeatureInt32StringNullableOfBooleanDelegate(int dialerId, string featureName, bool? overridenFeatureValue);
        public UpdateOverridenDialerSupportedFeatureInt32StringNullableOfBooleanDelegate UpdateOverridenDialerSupportedFeatureInt32StringNullableOfBoolean;

        void IDialerFacilities.UpdateOverridenDialerSupportedFeature(int dialerId, string featureName, bool? overridenFeatureValue)
        {

            if (UpdateOverridenDialerSupportedFeatureInt32StringNullableOfBoolean != null)
            {
                UpdateOverridenDialerSupportedFeatureInt32StringNullableOfBoolean(dialerId, featureName, overridenFeatureValue);
            } else if (_inner != null)
            {
                ((IDialerFacilities)_inner).UpdateOverridenDialerSupportedFeature(dialerId, featureName, overridenFeatureValue);
            }
        }

    }
}