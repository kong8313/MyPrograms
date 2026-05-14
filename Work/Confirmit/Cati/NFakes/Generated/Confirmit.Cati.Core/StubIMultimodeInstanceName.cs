using System;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubIMultimodeInstanceName : IMultimodeInstanceName 
    {
        private IMultimodeInstanceName _inner;

        public StubIMultimodeInstanceName()
        {
            _inner = null;
        }

        public IMultimodeInstanceName Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string CompanyIdToDatabaseNameInt32Delegate(int companyId);
        public CompanyIdToDatabaseNameInt32Delegate CompanyIdToDatabaseNameInt32;

        string IMultimodeInstanceName.CompanyIdToDatabaseName(int companyId)
        {


            if (CompanyIdToDatabaseNameInt32 != null)
            {
                return CompanyIdToDatabaseNameInt32(companyId);
            } else if (_inner != null)
            {
                return ((IMultimodeInstanceName)_inner).CompanyIdToDatabaseName(companyId);
            }

            return default(string);
        }

        public delegate int ServiceNameToCompanyIdStringDelegate(string serviceName);
        public ServiceNameToCompanyIdStringDelegate ServiceNameToCompanyIdString;

        int IMultimodeInstanceName.ServiceNameToCompanyId(string serviceName)
        {


            if (ServiceNameToCompanyIdString != null)
            {
                return ServiceNameToCompanyIdString(serviceName);
            } else if (_inner != null)
            {
                return ((IMultimodeInstanceName)_inner).ServiceNameToCompanyId(serviceName);
            }

            return default(int);
        }

    }
}