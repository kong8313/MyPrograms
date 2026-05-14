using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using DialerCommon;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    public class DialerFeaturesFactory
    {
        public static DialerFeatures CreateDefault()
        {
            var toggleSettings = ServiceLocator.Resolve<IToggleSettings>();

            var features = new DialerFeatures()
            {
                IsBargingSupported = toggleSettings.EnableMonitoringBargingMode,
                IsCoachingSupported = toggleSettings.EnableMonitoringCoachingMode,
                IsMonitoringMuteSupported = false,
                IsInternalTransferSupported = toggleSettings.EnableInternalTransfer,
                IsExternalTransferSupported = toggleSettings.EnableExternalTransfer,
                IsInboundSupported = toggleSettings.EnableInbound,
                IsIVRSupported = toggleSettings.EnableIVR,
                IsSoftphoneSingleSignOnSupported = false,
                IsAudioContentDownloadSupported = false,
                CustomIvrPipeline = false
            };

            return features;
        }
    }
}
