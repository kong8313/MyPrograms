using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace Confirmit.CATI.Common.WcfTools.MessageInterceptor
{
    /// <summary>
    /// Configuration class for <see cref="InterceptingBindingElement"/>. To make your <see cref="InterceptingBindingElement"/>
    /// configurable, derive from InterceptingElementInterceptingElement and override CreateMessageInterceptor()
    /// </summary>
    /// <remarks>
    /// Implementation based on the MSDN sample Custom Message Interceptor:
    /// http://msdn.microsoft.com/en-us/library/ms751495.aspx
    /// </remarks>
    public abstract class InterceptingElement : BindingElementExtensionElement
    {
        public override Type BindingElementType
        {
            get
            {
                return typeof(InterceptingBindingElement);
            }
        }

        protected abstract ChannelMessageInterceptor CreateMessageInterceptor();

        protected override BindingElement CreateBindingElement()
        {
            return new InterceptingBindingElement(CreateMessageInterceptor());
        }
    }
}
