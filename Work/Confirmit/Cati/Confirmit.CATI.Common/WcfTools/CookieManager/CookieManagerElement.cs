using Confirmit.CATI.Common.WcfTools.MessageInterceptor;

namespace Confirmit.CATI.Common.WcfTools.CookieManager
{
    /// <summary>
    /// Binding extension element for <see cref="CookieManager"/>.
    /// Add reference to this class to configuration file to use cookieManager in binding.
    /// </summary>
    /// <example>
    ///   <system.serviceModel>
    ///      <extensions>
    ///        <bindingElementExtensions>
    ///           <add name="cookieManager" type="Confirmit.CATI.Common.WcfTools.CookieManager.CookieManagerElement, Confirmit.CATI.Common"/>
    ///        </bindingElementExtensions>
    ///      </extensions>
    ///   </system.serviceModel>
    /// </example>
    public class CookieManagerElement : InterceptingElement
    {
        protected override ChannelMessageInterceptor CreateMessageInterceptor()
        {
            return CookieManager.Instance;
        }
    }
}