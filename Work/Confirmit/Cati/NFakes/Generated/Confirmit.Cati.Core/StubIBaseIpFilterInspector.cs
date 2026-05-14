using System;
using System.ServiceModel.Channels;
using Confirmit.CATI.Core.IpLockDown;
using Confirmit.CATI.Core.IpLockDown.IPFilterInspectors;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.IpLockDown.IPFilterInspectors.Fakes
{
    public class StubIBaseIpFilterInspector : IBaseIpFilterInspector 
    {
        private IBaseIpFilterInspector _inner;

        public StubIBaseIpFilterInspector()
        {
            _inner = null;
        }

        public IBaseIpFilterInspector Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Object AfterReceiveRequestMessageIpFilterCacheDataDelegate(Message request, IpFilterCacheData ipFilterCacheData);
        public AfterReceiveRequestMessageIpFilterCacheDataDelegate AfterReceiveRequestMessageIpFilterCacheData;

        Object IBaseIpFilterInspector.AfterReceiveRequest(Message request, IpFilterCacheData ipFilterCacheData)
        {


            if (AfterReceiveRequestMessageIpFilterCacheData != null)
            {
                return AfterReceiveRequestMessageIpFilterCacheData(request, ipFilterCacheData);
            } else if (_inner != null)
            {
                return ((IBaseIpFilterInspector)_inner).AfterReceiveRequest(request, ipFilterCacheData);
            }

            return default(Object);
        }

        public delegate WhiteList ParseWhiteListListOfStringDelegate(List<string> whiteAddressList);
        public ParseWhiteListListOfStringDelegate ParseWhiteListListOfString;

        WhiteList IBaseIpFilterInspector.ParseWhiteList(List<string> whiteAddressList)
        {


            if (ParseWhiteListListOfString != null)
            {
                return ParseWhiteListListOfString(whiteAddressList);
            } else if (_inner != null)
            {
                return ((IBaseIpFilterInspector)_inner).ParseWhiteList(whiteAddressList);
            }

            return default(WhiteList);
        }

    }
}