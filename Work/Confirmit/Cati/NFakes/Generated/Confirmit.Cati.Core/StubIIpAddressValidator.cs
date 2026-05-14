using System;
using Confirmit.CATI.Core.IpLockDown;
using System.Net;
using System.Collections.Generic;
using Confirmit.CATI.Core.IpLockDown.Validation;

namespace Confirmit.CATI.Core.IpLockDown.Validation.Fakes
{
    public class StubIIpAddressValidator : IIpAddressValidator 
    {
        private IIpAddressValidator _inner;

        public StubIIpAddressValidator()
        {
            _inner = null;
        }

        public IIpAddressValidator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool IsIpInWhiteListWhiteListIPAddressDictionaryOfStringListOfStringDelegate(WhiteList whiteList, IPAddress callerAddress, Dictionary<string, List<string>> hostName2IpsList);
        public IsIpInWhiteListWhiteListIPAddressDictionaryOfStringListOfStringDelegate IsIpInWhiteListWhiteListIPAddressDictionaryOfStringListOfString;

        bool IIpAddressValidator.IsIpInWhiteList(WhiteList whiteList, IPAddress callerAddress, Dictionary<string, List<string>> hostName2IpsList)
        {


            if (IsIpInWhiteListWhiteListIPAddressDictionaryOfStringListOfString != null)
            {
                return IsIpInWhiteListWhiteListIPAddressDictionaryOfStringListOfString(whiteList, callerAddress, hostName2IpsList);
            } else if (_inner != null)
            {
                return ((IIpAddressValidator)_inner).IsIpInWhiteList(whiteList, callerAddress, hostName2IpsList);
            }

            return default(bool);
        }

    }
}