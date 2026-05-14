extern alias CodiV34;
using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Telephony.DialerService.Contract;

using IDialerService34 = CodiV34::Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    internal class CodiVersion34FacilitiesProxy : ICodiVersionFacilitiesProxy
    {
        private readonly IChannelFactoryWrapper<IDialerService34> _dialerChannel;

        public CodiVersion34FacilitiesProxy(IChannelFactoryWrapper<IDialerService34> dialerChannel)
        {
            _dialerChannel = dialerChannel;
        }

        public IEnumerable<LogFileInfo> GetLogFiles()
        {
            throw new System.NotImplementedException();
        }

        public byte[] GetLogFileBodyZipped(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public void ReleaseDialerChannel() => _dialerChannel.Release();
    }
}