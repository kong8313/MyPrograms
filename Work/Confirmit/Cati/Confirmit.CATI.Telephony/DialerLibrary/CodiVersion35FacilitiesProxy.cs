extern alias CodiV35;

using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.WcfTools;

using IDialerService35 = CodiV35::Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    internal class CodiVersion35FacilitiesProxy : ICodiVersionFacilitiesProxy
    {
        private readonly IChannelFactoryWrapper<IDialerService35> _dialerChannel;

        public CodiVersion35FacilitiesProxy(IChannelFactoryWrapper<IDialerService35> dialerChannel)
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