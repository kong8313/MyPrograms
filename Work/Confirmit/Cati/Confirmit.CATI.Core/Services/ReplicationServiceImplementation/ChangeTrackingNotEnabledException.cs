using System;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation
{
    public class ChangeTrackingNotEnabledException : Exception
    {
        public ChangeTrackingNotEnabledException(string message) : base(message)
        {
        }
    }
}