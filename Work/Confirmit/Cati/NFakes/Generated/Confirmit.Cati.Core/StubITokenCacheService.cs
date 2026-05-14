using System;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubITokenCacheService : ITokenCacheService 
    {
        private ITokenCacheService _inner;

        public StubITokenCacheService()
        {
            _inner = null;
        }

        public ITokenCacheService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SetStringStringDelegate(string key, string value);
        public SetStringStringDelegate SetStringString;

        void ITokenCacheService.Set(string key, string value)
        {

            if (SetStringString != null)
            {
                SetStringString(key, value);
            } else if (_inner != null)
            {
                ((ITokenCacheService)_inner).Set(key, value);
            }
        }

        public delegate string GetStringDelegate(string key);
        public GetStringDelegate GetString;

        string ITokenCacheService.Get(string key)
        {


            if (GetString != null)
            {
                return GetString(key);
            } else if (_inner != null)
            {
                return ((ITokenCacheService)_inner).Get(key);
            }

            return default(string);
        }

        public delegate void RemoveStringDelegate(string key);
        public RemoveStringDelegate RemoveString;

        void ITokenCacheService.Remove(string key)
        {

            if (RemoveString != null)
            {
                RemoveString(key);
            } else if (_inner != null)
            {
                ((ITokenCacheService)_inner).Remove(key);
            }
        }

    }
}