using System;
using Confirmit.CATI.Core.DAL.Handmade.Cache;

namespace Confirmit.CATI.Core.DAL.Handmade.Cache.Fakes
{
    public class StubISystemSettingCache : ISystemSettingCache 
    {
        private ISystemSettingCache _inner;

        public StubISystemSettingCache()
        {
            _inner = null;
        }

        public ISystemSettingCache Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetStringDelegate(string settingSystemName);
        public GetStringDelegate GetString;

        string ISystemSettingCache.Get(string settingSystemName)
        {


            if (GetString != null)
            {
                return GetString(settingSystemName);
            } else if (_inner != null)
            {
                return ((ISystemSettingCache)_inner).Get(settingSystemName);
            }

            return default(string);
        }

        void ISystemSettingCache.Set<T>(string settingSystemName, T value)
        {

        }

        public delegate void ResetDelegate();
        public ResetDelegate Reset;

        void ISystemSettingCache.Reset()
        {

            if (Reset != null)
            {
                Reset();
            } else if (_inner != null)
            {
                ((ISystemSettingCache)_inner).Reset();
            }
        }

    }
}