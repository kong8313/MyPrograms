using System.Collections.Generic;
using System.Net;

namespace Confirmit.CATI.Core.IpLockDown
{
    public class IpFilterCacheData
    {
        public WhiteList WhiteList { get; set; }

        public List<IPAddress> ValidatedIpsCache { get; set; }

        public IpFilterCacheData(WhiteList whiteList)
        {
            WhiteList = whiteList;
            ValidatedIpsCache = new List<IPAddress>();
        } 
    }
}