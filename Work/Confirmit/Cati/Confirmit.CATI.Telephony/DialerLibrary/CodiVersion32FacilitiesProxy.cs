extern alias CodiV32;
using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.WcfTools;
using IDialerService32 = CodiV32::Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    internal class CodiVersion32FacilitiesProxy : ICodiVersionFacilitiesProxy
    {
        private readonly IChannelFactoryWrapper<IDialerService32> _dialerChannel;

        public CodiVersion32FacilitiesProxy(IChannelFactoryWrapper<IDialerService32> dialerChannel)
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