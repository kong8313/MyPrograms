extern alias CodiV36;

using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.WcfTools;

using IDialerService36 = CodiV36::Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    internal class CodiVersion36FacilitiesProxy : ICodiVersionFacilitiesProxy
    {
        private IChannelFactoryWrapper<IDialerService36> _dialerChannel;

        public CodiVersion36FacilitiesProxy(IChannelFactoryWrapper<IDialerService36> dialerChannel)
        {
            _dialerChannel = dialerChannel;
        }

        public IEnumerable<LogFileInfo> GetLogFiles() => _dialerChannel.Execute(x => x.GetLogFiles());

        public byte[] GetLogFileBodyZipped(string fileName) => _dialerChannel.Execute(x => x.GetLogFileBodyZipped(fileName));

        public void ReleaseDialerChannel() => _dialerChannel.Release();
    }
}