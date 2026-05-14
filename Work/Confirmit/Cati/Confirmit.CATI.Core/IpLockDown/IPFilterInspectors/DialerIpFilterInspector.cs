using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.IpLockDown.IPFilterInspectors
{
    public class DialerIpFilterInspector : IDispatchMessageInspector
    {
        private readonly IDialersRepository _dialersRepository;
        private readonly IBaseIpFilterInspector _baseIpFilterInspector;
        private readonly ISystemSettingRepository _systemSettingRepository;
        private readonly IpFilterCache _ipFilterCache;

        public DialerIpFilterInspector(
            IDialersRepository dialersRepository,
            IBaseIpFilterInspector baseIpFilterInspector,
            ISystemSettingRepository systemSettingRepository,
            IpFilterCache ipFilterCache)
        {
            _dialersRepository = dialersRepository;
            _baseIpFilterInspector = baseIpFilterInspector;
            _systemSettingRepository = systemSettingRepository;
            _ipFilterCache = ipFilterCache;
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return _baseIpFilterInspector.AfterReceiveRequest(
                request, 
                _ipFilterCache.LoadDialerWsServiceIpCacheIfEmpty(ReadDialerWsServiceIpCacheData));
        }

        public IpFilterCacheData ReadDialerWsServiceIpCacheData()
        {
            string accessAllowedIpAddresses = _systemSettingRepository.Get(SystemSettingConstants.Server.AccessAllowedIPAddresses, BackendInstance.Current.CompanyId);
            var ipAddressStrings = new List<string> { accessAllowedIpAddresses };
            ipAddressStrings.AddRange(_dialersRepository.GetAll().Select(d => d.WhiteList));

            return new IpFilterCacheData(_baseIpFilterInspector.ParseWhiteList(ipAddressStrings));
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }
    }
}
