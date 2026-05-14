using System.Collections.Generic;
using System.Net;

namespace Confirmit.CATI.Core.IpLockDown.Validation
{
    public interface IIpAddressValidator
    {
        bool IsIpInWhiteList(WhiteList whiteList, IPAddress callerAddress, Dictionary<string, List<string>> hostName2IpsList);
    }
}
