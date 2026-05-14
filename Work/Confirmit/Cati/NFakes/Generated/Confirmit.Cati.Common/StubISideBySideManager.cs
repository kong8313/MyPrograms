using System;
using Confirmit.CATI.Common.SideBySide;

namespace Confirmit.CATI.Common.SideBySide.Fakes
{
    public class StubISideBySideManager : ISideBySideManager 
    {
        private ISideBySideManager _inner;

        public StubISideBySideManager()
        {
            _inner = null;
        }

        public ISideBySideManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string AddSideBySideNameToBackendWCFServiceUrlStringDelegate(string url);
        public AddSideBySideNameToBackendWCFServiceUrlStringDelegate AddSideBySideNameToBackendWCFServiceUrlString;

        string ISideBySideManager.AddSideBySideNameToBackendWCFServiceUrl(string url)
        {


            if (AddSideBySideNameToBackendWCFServiceUrlString != null)
            {
                return AddSideBySideNameToBackendWCFServiceUrlString(url);
            } else if (_inner != null)
            {
                return ((ISideBySideManager)_inner).AddSideBySideNameToBackendWCFServiceUrl(url);
            }

            return default(string);
        }

        public delegate string AddSideBySideNameToServiceNameStringDelegate(string serviceName);
        public AddSideBySideNameToServiceNameStringDelegate AddSideBySideNameToServiceNameString;

        string ISideBySideManager.AddSideBySideNameToServiceName(string serviceName)
        {


            if (AddSideBySideNameToServiceNameString != null)
            {
                return AddSideBySideNameToServiceNameString(serviceName);
            } else if (_inner != null)
            {
                return ((ISideBySideManager)_inner).AddSideBySideNameToServiceName(serviceName);
            }

            return default(string);
        }

        public delegate string RemoveSideBySideNameFromServiceNameStringDelegate(string serviceName);
        public RemoveSideBySideNameFromServiceNameStringDelegate RemoveSideBySideNameFromServiceNameString;

        string ISideBySideManager.RemoveSideBySideNameFromServiceName(string serviceName)
        {


            if (RemoveSideBySideNameFromServiceNameString != null)
            {
                return RemoveSideBySideNameFromServiceNameString(serviceName);
            } else if (_inner != null)
            {
                return ((ISideBySideManager)_inner).RemoveSideBySideNameFromServiceName(serviceName);
            }

            return default(string);
        }

        public delegate string AddSideBySideNameToIISServiceUrlStringDelegate(string url);
        public AddSideBySideNameToIISServiceUrlStringDelegate AddSideBySideNameToIISServiceUrlString;

        string ISideBySideManager.AddSideBySideNameToIISServiceUrl(string url)
        {


            if (AddSideBySideNameToIISServiceUrlString != null)
            {
                return AddSideBySideNameToIISServiceUrlString(url);
            } else if (_inner != null)
            {
                return ((ISideBySideManager)_inner).AddSideBySideNameToIISServiceUrl(url);
            }

            return default(string);
        }

        private string _SideBySideName;
        public Func<string> SideBySideNameGet;
        public Action<string> SideBySideNameSetString;

        string ISideBySideManager.SideBySideName
        {
            get
            {
                if (SideBySideNameGet != null)
                {
                    return SideBySideNameGet();
                } else if (_inner != null)
                {
                    return ((ISideBySideManager)_inner).SideBySideName;
                }

                if (SideBySideNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SideBySideName;
                }

                return default(string);
            }

            set
            {
                if (SideBySideNameSetString != null)
                {
                    SideBySideNameSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISideBySideManager)_inner).SideBySideName = value;
                    return;
                }

                if (SideBySideNameGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SideBySideName = value;
                }

            }
        }

        private string _ServicePrefix;
        public Func<string> ServicePrefixGet;
        public Action<string> ServicePrefixSetString;

        string ISideBySideManager.ServicePrefix
        {
            get
            {
                if (ServicePrefixGet != null)
                {
                    return ServicePrefixGet();
                } else if (_inner != null)
                {
                    return ((ISideBySideManager)_inner).ServicePrefix;
                }

                if (ServicePrefixSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ServicePrefix;
                }

                return default(string);
            }

        }

    }
}