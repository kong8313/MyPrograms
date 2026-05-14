extern alias CodiV33;

using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.WcfTools;
using IDialerService33 = CodiV33::Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    internal class CodiVersion33FacilitiesProxy : ICodiVersionFacilitiesProxy
    {
        private readonly IChannelFactoryWrapper<IDialerService33> _dialerChannel;

        public CodiVersion33FacilitiesProxy(IChannelFactoryWrapper<IDialerService33> dialerChannel)
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