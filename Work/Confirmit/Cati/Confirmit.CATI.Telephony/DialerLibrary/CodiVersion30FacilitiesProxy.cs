extern alias CodiV30;

using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.WcfTools;

using IDialerService30 = CodiV30::Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    internal class CodiVersion30FacilitiesProxy : ICodiVersionFacilitiesProxy
    {
        private readonly IChannelFactoryWrapper<IDialerService30> _dialerChannel;

        public CodiVersion30FacilitiesProxy(IChannelFactoryWrapper<IDialerService30> dialerChannel)
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