using System.Collections.Generic;
using NetTools;

namespace Confirmit.CATI.Core.IpLockDown
{
    public class WhiteList
    {
        public IList<IPAddressRange> IpRanges { get; set; }

        public IList<string> HostNames { get; set; }

        public WhiteList()
        {
            IpRanges = new List<IPAddressRange>();
            HostNames = new List<string>();
        }
    }
}
