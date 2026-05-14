using System;

namespace Confirmit.CATI.Common.ServiceLocation
{
    [Serializable]
    public class ServiceLocatorException : Exception
    {
        public ServiceLocatorException(string message)
            : base(message)
        {
        }
    }
}
