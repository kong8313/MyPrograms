using System.Collections.Generic;
using System.ServiceModel.Channels;

namespace Confirmit.CATI.Core.IpLockDown.IPFilterInspectors
{
    public interface IBaseIpFilterInspector
    {
        object AfterReceiveRequest(Message request, IpFilterCacheData ipFilterCacheData);

        WhiteList ParseWhiteList(List<string> whiteAddressList);
    }
}