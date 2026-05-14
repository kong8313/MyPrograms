using System.ServiceModel.Channels;
using System.Net.Security;
using System.Diagnostics;

namespace Confirmit.CATI.Common.WcfTools.PseudoHttpsTransport
{
    /// <summary>
    /// Security capabilities for insecure transport of credentials through HTTP.
    /// </summary>
    public class SecurityCapabilities : ISecurityCapabilities
    {
        /// <summary>
        /// Gets supported request protection level.
        /// </summary>
        public ProtectionLevel SupportedRequestProtectionLevel
        {
            [DebuggerStepThrough]
            get { return ProtectionLevel.EncryptAndSign; }
        }

        /// <summary>
        /// Gets supported response protection level.
        /// </summary>
        public ProtectionLevel SupportedResponseProtectionLevel
        {
            [DebuggerStepThrough]
            get { return ProtectionLevel.EncryptAndSign; }
        }

        /// <summary>
        /// Gets if client authentication is supported.
        /// </summary>
        public bool SupportsClientAuthentication
        {
            [DebuggerStepThrough]
            get { return false; }
        }

        /// <summary>
        /// Gets if windows client identity is supported.
        /// </summary>
        public bool SupportsClientWindowsIdentity
        {
            [DebuggerStepThrough]
            get { return false; }
        }

        /// <summary>
        /// Gets if server authentication is supported.
        /// </summary>
        public bool SupportsServerAuthentication
        {
            [DebuggerStepThrough]
            get { return true; }
        }
    }
}