using System;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubIDbLibProvider : IDbLibProvider 
    {
        private IDbLibProvider _inner;

        public StubIDbLibProvider()
        {
            _inner = null;
        }

        public IDbLibProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string ConfirmAdminConnectionStringStringDelegate(string projectId);
        public ConfirmAdminConnectionStringStringDelegate ConfirmAdminConnectionStringString;

        string IDbLibProvider.ConfirmAdminConnectionString(string projectId)
        {


            if (ConfirmAdminConnectionStringString != null)
            {
                return ConfirmAdminConnectionStringString(projectId);
            } else if (_inner != null)
            {
                return ((IDbLibProvider)_inner).ConfirmAdminConnectionString(projectId);
            }

            return default(string);
        }

        public delegate string GetConnectionStringForSpecificCompanyInt32Delegate(int companyId);
        public GetConnectionStringForSpecificCompanyInt32Delegate GetConnectionStringForSpecificCompanyInt32;

        string IDbLibProvider.GetConnectionStringForSpecificCompany(int companyId)
        {


            if (GetConnectionStringForSpecificCompanyInt32 != null)
            {
                return GetConnectionStringForSpecificCompanyInt32(companyId);
            } else if (_inner != null)
            {
                return ((IDbLibProvider)_inner).GetConnectionStringForSpecificCompany(companyId);
            }

            return default(string);
        }

        public delegate int GetRandomCatiSqlServerIdDelegate();
        public GetRandomCatiSqlServerIdDelegate GetRandomCatiSqlServerId;

        int IDbLibProvider.GetRandomCatiSqlServerId()
        {


            if (GetRandomCatiSqlServerId != null)
            {
                return GetRandomCatiSqlServerId();
            } else if (_inner != null)
            {
                return ((IDbLibProvider)_inner).GetRandomCatiSqlServerId();
            }

            return default(int);
        }

        public delegate string GetMasterConnectionStringForServerInt32Delegate(int sqlServerId);
        public GetMasterConnectionStringForServerInt32Delegate GetMasterConnectionStringForServerInt32;

        string IDbLibProvider.GetMasterConnectionStringForServer(int sqlServerId)
        {


            if (GetMasterConnectionStringForServerInt32 != null)
            {
                return GetMasterConnectionStringForServerInt32(sqlServerId);
            } else if (_inner != null)
            {
                return ((IDbLibProvider)_inner).GetMasterConnectionStringForServer(sqlServerId);
            }

            return default(string);
        }

        private string _CatiDefaultConnectionString;
        public Func<string> CatiDefaultConnectionStringGet;
        public Action<string> CatiDefaultConnectionStringSetString;

        string IDbLibProvider.CatiDefaultConnectionString
        {
            get
            {
                if (CatiDefaultConnectionStringGet != null)
                {
                    return CatiDefaultConnectionStringGet();
                } else if (_inner != null)
                {
                    return ((IDbLibProvider)_inner).CatiDefaultConnectionString;
                }

                if (CatiDefaultConnectionStringSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CatiDefaultConnectionString;
                }

                return default(string);
            }

        }

        private string _ConfirmConnectionString;
        public Func<string> ConfirmConnectionStringGet;
        public Action<string> ConfirmConnectionStringSetString;

        string IDbLibProvider.ConfirmConnectionString
        {
            get
            {
                if (ConfirmConnectionStringGet != null)
                {
                    return ConfirmConnectionStringGet();
                } else if (_inner != null)
                {
                    return ((IDbLibProvider)_inner).ConfirmConnectionString;
                }

                if (ConfirmConnectionStringSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ConfirmConnectionString;
                }

                return default(string);
            }

        }

        private string _ConfirmlogConnectionString;
        public Func<string> ConfirmlogConnectionStringGet;
        public Action<string> ConfirmlogConnectionStringSetString;

        string IDbLibProvider.ConfirmlogConnectionString
        {
            get
            {
                if (ConfirmlogConnectionStringGet != null)
                {
                    return ConfirmlogConnectionStringGet();
                } else if (_inner != null)
                {
                    return ((IDbLibProvider)_inner).ConfirmlogConnectionString;
                }

                if (ConfirmlogConnectionStringSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ConfirmlogConnectionString;
                }

                return default(string);
            }

        }

    }
}