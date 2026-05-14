using System.ServiceModel.Channels;

namespace Confirmit.CATI.Common.WcfTools.PseudoHttpsTransport
{
    /// <summary>
    /// Transport binding element for insecure transport of credentials through HTTP.
    /// </summary>
    public class PseudoHttpsTransportBindingElement : HttpTransportBindingElement
    {
        public PseudoHttpsTransportBindingElement()
        {
        }

        protected PseudoHttpsTransportBindingElement(HttpTransportBindingElement elementToBeCloned) :
            base(elementToBeCloned)
        {
        }

        /// <summary>
        /// Gets a property from the specified BindingContext.
        /// </summary>
        /// <typeparam name="T">The property to get.</typeparam>
        /// <param name="context">Binding context.</param>
        /// <returns>Property from the specified BindingContext.</returns>
        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T) (object) new SecurityCapabilities();
            }
            return base.GetProperty<T>(context);
        }

        /// <summary>
        /// Returns cloned binding element.
        /// </summary>
        /// <returns>Cloned binding element.</returns>
        public override BindingElement Clone()
        {
            return new PseudoHttpsTransportBindingElement(this);
        }
    }
}