using System;
using Confirmit.CATI.Common.WcfTools;

namespace Confirmit.CATI.Common.WcfTools.Fakes
{
    public class StubIChannelFactoryWrapper<T> : IChannelFactoryWrapper<T>  where T : class 
    {
        private IChannelFactoryWrapper<T> _inner;

        public StubIChannelFactoryWrapper()
        {
            _inner = null;
        }

        public IChannelFactoryWrapper<T> Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate T GetChannelDelegate();
        public GetChannelDelegate GetChannel;

        T IChannelFactoryWrapper<T>.GetChannel()
        {


            if (GetChannel != null)
            {
                return GetChannel();
            } else if (_inner != null)
            {
                return ((IChannelFactoryWrapper<T>)_inner).GetChannel();
            }

            return default(T);
        }

        public delegate Uri GetFactoryUriDelegate();
        public GetFactoryUriDelegate GetFactoryUri;

        Uri IChannelFactoryWrapper<T>.GetFactoryUri()
        {


            if (GetFactoryUri != null)
            {
                return GetFactoryUri();
            } else if (_inner != null)
            {
                return ((IChannelFactoryWrapper<T>)_inner).GetFactoryUri();
            }

            return default(Uri);
        }

        public delegate void ReleaseDelegate();
        public ReleaseDelegate Release;

        void IChannelFactoryWrapper<T>.Release()
        {

            if (Release != null)
            {
                Release();
            } else if (_inner != null)
            {
                ((IChannelFactoryWrapper<T>)_inner).Release();
            }
        }

        public delegate void ExecuteActionOfTStringDelegate(Action<T> action, string methodName);
        public ExecuteActionOfTStringDelegate ExecuteActionOfTString;

        void IChannelFactoryWrapper<T>.Execute(Action<T> action, string methodName)
        {

            if (ExecuteActionOfTString != null)
            {
                ExecuteActionOfTString(action, methodName);
            } else if (_inner != null)
            {
                ((IChannelFactoryWrapper<T>)_inner).Execute(action, methodName);
            }
        }

        TResult IChannelFactoryWrapper<T>.Execute<TResult>(Func<T, TResult> function, string methodName)
        {


            return default(TResult);
        }

    }
}