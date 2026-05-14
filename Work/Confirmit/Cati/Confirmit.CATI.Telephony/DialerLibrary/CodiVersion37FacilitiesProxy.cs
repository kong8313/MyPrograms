using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Telephony.DialerService.Contract;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    internal class CodiVersion37FacilitiesProxy : ICodiVersionFacilitiesProxy
    {
        private IChannelFactoryWrapper<IDialerService> _dialerChannel;

        public CodiVersion37FacilitiesProxy(IChannelFactoryWrapper<IDialerService> dialerChannel)
        {
            _dialerChannel = dialerChannel;
        }

        public IEnumerable<LogFileInfo> GetLogFiles() => _dialerChannel.Execute(x => x.GetLogFiles());

        public byte[] GetLogFileBodyZipped(string fileName) => _dialerChannel.Execute(x => x.GetLogFileBodyZipped(fileName));

        public void ReleaseDialerChannel() => _dialerChannel.Release();
    }
}