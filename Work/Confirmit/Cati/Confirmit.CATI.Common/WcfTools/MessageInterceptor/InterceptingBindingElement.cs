using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Confirmit.CATI.Common.WcfTools.MessageInterceptor
{
    /// <remarks>
    /// Implementation based on the MSDN sample Custom Message Interceptor:
    /// http://msdn.microsoft.com/en-us/library/ms751495.aspx
    /// </remarks>
    public class InterceptingBindingElement : BindingElement, IPolicyExportExtension
    {
        private readonly ChannelMessageInterceptor interceptor;

        public InterceptingBindingElement(ChannelMessageInterceptor interceptor)
        {
            this.interceptor = interceptor;
        }

        protected InterceptingBindingElement(InterceptingBindingElement other) 
            : base(other)
        {
            this.interceptor = other.Interceptor;
        }

        public ChannelMessageInterceptor Interceptor
        {
            get
            {
                return this.interceptor != null ? this.interceptor.Clone() : new NullMessageInterceptor();
            }
        }

        public override BindingElement Clone()
        {
            return new InterceptingBindingElement(this);
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            return context.CanBuildInnerChannelListener<TChannel>();
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            return new InterceptingChannelFactory<TChannel>(this.Interceptor, context);
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            return new InterceptingChannelListener<TChannel>(this.Interceptor, context);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(ChannelMessageInterceptor))
            {
                return (T)(object)this.Interceptor;
            }

            return context.GetInnerProperty<T>();
        }

        void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
        {
        }

        internal class NullMessageInterceptor : ChannelMessageInterceptor
        {
            public override ChannelMessageInterceptor Clone()
            {
                return new NullMessageInterceptor();
            }
        }
    }
}