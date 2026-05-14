using System;
using Confirmit.CATI.Core.DAL.Framework;

namespace Confirmit.CATI.Core.DAL.Framework.Fakes
{
    public class StubITableCache : ITableCache 
    {
        private ITableCache _inner;

        public StubITableCache()
        {
            _inner = null;
        }

        public ITableCache Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void OnTableChangedDelegate();
        public OnTableChangedDelegate OnTableChanged;

        void ITableCache.OnTableChanged()
        {

            if (OnTableChanged != null)
            {
                OnTableChanged();
            } else if (_inner != null)
            {
                ((ITableCache)_inner).OnTableChanged();
            }
        }

        public delegate void OnCacheExpiredDelegate();
        public OnCacheExpiredDelegate OnCacheExpired;

        void ITableCache.OnCacheExpired()
        {

            if (OnCacheExpired != null)
            {
                OnCacheExpired();
            } else if (_inner != null)
            {
                ((ITableCache)_inner).OnCacheExpired();
            }
        }

        private string _CachedTableName;
        public Func<string> CachedTableNameGet;
        public Action<string> CachedTableNameSetString;

        string ITableCache.CachedTableName
        {
            get
            {
                if (CachedTableNameGet != null)
                {
                    return CachedTableNameGet();
                } else if (_inner != null)
                {
                    return ((ITableCache)_inner).CachedTableName;
                }

                if (CachedTableNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CachedTableName;
                }

                return default(string);
            }

        }

    }
}