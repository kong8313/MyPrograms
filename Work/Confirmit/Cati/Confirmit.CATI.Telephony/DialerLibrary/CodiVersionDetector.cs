using System;
using System.Diagnostics;
using System.ServiceModel;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Telephony.DialerService.Contract;
using DialerCommon;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    public class CodiVersionDetector
    {
        public CodiVersionInfoCommon Version(IChannelFactoryWrapper<IDialerService> dialerChannel)
        {
            try
            {
                var versionInfo = dialerChannel.Execute(x => x.Version());

                return new CodiVersionInfoCommon(
                    versionInfo[0], // CodiMajorVersion
                    versionInfo[1], // CodiFullVersion
                    versionInfo[2]  // DialerDriverNameAndVersion
                );
            }
            catch (ActionNotSupportedException)
            {
                // That means the WS has no Version method
                // In this case we assume the WS is "pre-versioned" (17.5.4.0/18.5.5.0/3.0) version

                Trace.TraceWarning("CodiVersionDetector.Version, An older version is detected. It will be treated as (17.5.4.0/18.5.5.0/3.0) version.");

                string dialerDllName;
                string dialerDllVersion;

                try
                {
                    dialerDllName = dialerChannel.Execute(x => x.GetName());
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning("CodiVersionDetector.Version: Unable to get dialer driver dll name. /// {0}" , ex);
                    dialerDllName = "Unknown(Exception)";
                }

                try
                {
                    dialerDllVersion = dialerChannel.Execute(x => x.GetVersion());
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning("CodiVersionDetector.Version: Unable to get dialer driver dll version. /// {0}", ex);
                    dialerDllVersion = "Unknown(Exception)";
                }

                return new CodiVersionInfoCommon(
                    "3.0", // CodiMajorVersion
                    "0.0.0.0", // CodiFullVersion
                    dialerDllName + "#" + dialerDllVersion  // DialerDriverNameAndVersion
                );
            }
        }
    }
}