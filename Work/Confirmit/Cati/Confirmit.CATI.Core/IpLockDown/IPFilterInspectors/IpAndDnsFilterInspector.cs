using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.IpLockDown.IPFilterInspectors
{
    public class IpAndDnsFilterInspector : IDispatchMessageInspector
    {
        private readonly IBaseIpFilterInspector _baseIpFilterInspector;
        private readonly ISystemSettingRepository _systemSettingRepository;
        private readonly IpFilterCache _ipFilterCache;

        public IpAndDnsFilterInspector(            
            IBaseIpFilterInspector baseIpFilterInspector,
            ISystemSettingRepository systemSettingRepository,
            IpFilterCache ipFilterCache)
        {
            _baseIpFilterInspector = baseIpFilterInspector;
            _systemSettingRepository = systemSettingRepository;
            _ipFilterCache = ipFilterCache;
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return _baseIpFilterInspector.AfterReceiveRequest(
                request, 
                _ipFilterCache.LoadInternalServicesIpCacheIfEmpty(ReadInternalServicesIpCacheData));
            
        }

        public IpFilterCacheData ReadInternalServicesIpCacheData()
        {
            string accessAllowedIpAddresses = _systemSettingRepository.Get(SystemSettingConstants.Server.AccessAllowedIPAddresses, BackendInstance.Current.CompanyId);
            return new IpFilterCacheData(_baseIpFilterInspector.ParseWhiteList(new List<string> { accessAllowedIpAddresses }));
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }
    }
}
