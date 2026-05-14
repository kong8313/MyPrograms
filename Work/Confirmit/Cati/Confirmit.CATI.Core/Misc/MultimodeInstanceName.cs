using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Core.Misc
{
    public interface IMultimodeInstanceName
    {
        string CompanyIdToDatabaseName(int companyId);
        int ServiceNameToCompanyId(string serviceName);
    }

    public class MultimodeInstanceName : IMultimodeInstanceName
    {
        private const string DefaultDatabase = "ConfirmitCATIV15";
        private const string DatabasePrefix = "ConfirmitCATIV15_";

        public static bool IsNameOfService(string name)
        {
            return name.StartsWith(ServiceLocator.Resolve<ISideBySideManager>().ServicePrefix);
        }

        public static string CompanyIdToServiceName(int companyId)
        {
            return ServiceLocator.Resolve<ISideBySideManager>().ServicePrefix + companyId;
        }

        public static int ServiceNameToCompanyId(string serviceName)
        {
            var companyId = serviceName.Remove(0, ServiceLocator.Resolve<ISideBySideManager>().ServicePrefix.Length);

            if (string.IsNullOrEmpty(companyId))
            {
                return 0;
            }

            return int.Parse(companyId);
        }

        public static string CompanyIdToDatabaseName(int companyId)
        {
            if (companyId == 0)
            {
                return DefaultDatabase;
            }

            return DatabasePrefix + companyId;
        }

        public static string GetDefaultServiceName()
        {
            string servicePrefix = ServiceLocator.Resolve<ISideBySideManager>().ServicePrefix;
            return servicePrefix.TrimEnd('$');
        }

        string IMultimodeInstanceName.CompanyIdToDatabaseName(int companyId)
        {
            return CompanyIdToDatabaseName(companyId);
        }

        int IMultimodeInstanceName.ServiceNameToCompanyId(string serviceName)
        {
            return ServiceNameToCompanyId(serviceName);
        }
    }
}