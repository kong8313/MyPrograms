using System;
using System.ServiceModel.Configuration;
using System.Diagnostics;
using System.ServiceModel.Channels;

namespace Confirmit.CATI.Common.WcfTools.PseudoHttpsTransport
{
    /// <summary>
    /// Transport element for insecure transport of credentials through HTTP.
    /// </summary>
    public class PseudoHttpsTransportElement : HttpTransportElement
    {
        /// <summary>
        /// Gets type of binding element.
        /// </summary>
        public override Type BindingElementType
        {
            [DebuggerStepThrough]
            get { return typeof(PseudoHttpsTransportBindingElement); }
        }

        /// <summary>
        /// Creates default transport binding element.
        /// </summary>
        /// <returns>Default transport binding element.</returns>
        protected override TransportBindingElement CreateDefaultBindingElement()
        {
            return new PseudoHttpsTransportBindingElement();
        }
    }
}