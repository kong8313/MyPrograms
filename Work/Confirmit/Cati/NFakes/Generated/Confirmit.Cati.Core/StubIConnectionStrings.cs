using System;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubIConnectionStrings : IConnectionStrings 
    {
        private IConnectionStrings _inner;

        public StubIConnectionStrings()
        {
            _inner = null;
        }

        public IConnectionStrings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetConnectionStringForSpecificCompanyInt32Delegate(int companyId);
        public GetConnectionStringForSpecificCompanyInt32Delegate GetConnectionStringForSpecificCompanyInt32;

        string IConnectionStrings.GetConnectionStringForSpecificCompany(int companyId)
        {


            if (GetConnectionStringForSpecificCompanyInt32 != null)
            {
                return GetConnectionStringForSpecificCompanyInt32(companyId);
            } else if (_inner != null)
            {
                return ((IConnectionStrings)_inner).GetConnectionStringForSpecificCompany(companyId);
            }

            return default(string);
        }

        public delegate string GetMasterConnectionStringForSpecificServerInt32Delegate(int serverId);
        public GetMasterConnectionStringForSpecificServerInt32Delegate GetMasterConnectionStringForSpecificServerInt32;

        string IConnectionStrings.GetMasterConnectionStringForSpecificServer(int serverId)
        {


            if (GetMasterConnectionStringForSpecificServerInt32 != null)
            {
                return GetMasterConnectionStringForSpecificServerInt32(serverId);
            } else if (_inner != null)
            {
                return ((IConnectionStrings)_inner).GetMasterConnectionStringForSpecificServer(serverId);
            }

            return default(string);
        }

        public delegate string GetMasterConnectionStringForSpecificCompanyServerInt32Delegate(int companyId);
        public GetMasterConnectionStringForSpecificCompanyServerInt32Delegate GetMasterConnectionStringForSpecificCompanyServerInt32;

        string IConnectionStrings.GetMasterConnectionStringForSpecificCompanyServer(int companyId)
        {


            if (GetMasterConnectionStringForSpecificCompanyServerInt32 != null)
            {
                return GetMasterConnectionStringForSpecificCompanyServerInt32(companyId);
            } else if (_inner != null)
            {
                return ((IConnectionStrings)_inner).GetMasterConnectionStringForSpecificCompanyServer(companyId);
            }

            return default(string);
        }

        private string _MasterConnectionString;
        public Func<string> MasterConnectionStringGet;
        public Action<string> MasterConnectionStringSetString;

        string IConnectionStrings.MasterConnectionString
        {
            get
            {
                if (MasterConnectionStringGet != null)
                {
                    return MasterConnectionStringGet();
                } else if (_inner != null)
                {
                    return ((IConnectionStrings)_inner).MasterConnectionString;
                }

                if (MasterConnectionStringSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MasterConnectionString;
                }

                return default(string);
            }

        }

        private string _ConfirmlogConnectionString;
        public Func<string> ConfirmlogConnectionStringGet;
        public Action<string> ConfirmlogConnectionStringSetString;

        string IConnectionStrings.ConfirmlogConnectionString
        {
            get
            {
                if (ConfirmlogConnectionStringGet != null)
                {
                    return ConfirmlogConnectionStringGet();
                } else if (_inner != null)
                {
                    return ((IConnectionStrings)_inner).ConfirmlogConnectionString;
                }

                if (ConfirmlogConnectionStringSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ConfirmlogConnectionString;
                }

                return default(string);
            }

        }

        private string _ConfirmConnectionString;
        public Func<string> ConfirmConnectionStringGet;
        public Action<string> ConfirmConnectionStringSetString;

        string IConnectionStrings.ConfirmConnectionString
        {
            get
            {
                if (ConfirmConnectionStringGet != null)
                {
                    return ConfirmConnectionStringGet();
                } else if (_inner != null)
                {
                    return ((IConnectionStrings)_inner).ConfirmConnectionString;
                }

                if (ConfirmConnectionStringSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ConfirmConnectionString;
                }

                return default(string);
            }

        }

        private string _DefaultInstanceConnectionString;
        public Func<string> DefaultInstanceConnectionStringGet;
        public Action<string> DefaultInstanceConnectionStringSetString;

        string IConnectionStrings.DefaultInstanceConnectionString
        {
            get
            {
                if (DefaultInstanceConnectionStringGet != null)
                {
                    return DefaultInstanceConnectionStringGet();
                } else if (_inner != null)
                {
                    return ((IConnectionStrings)_inner).DefaultInstanceConnectionString;
                }

                if (DefaultInstanceConnectionStringSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DefaultInstanceConnectionString;
                }

                return default(string);
            }

        }

    }
}