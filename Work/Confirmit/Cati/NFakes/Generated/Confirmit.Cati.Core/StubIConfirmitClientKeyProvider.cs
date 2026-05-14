using System;
using Confirmit.CATI.Core.Misc.ConfirmitClientKey;

namespace Confirmit.CATI.Core.Misc.ConfirmitClientKey.Fakes
{
    public class StubIConfirmitClientKeyProvider : IConfirmitClientKeyProvider 
    {
        private IConfirmitClientKeyProvider _inner;

        public StubIConfirmitClientKeyProvider()
        {
            _inner = null;
        }

        public IConfirmitClientKeyProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetDelegate();
        public GetDelegate Get;

        string IConfirmitClientKeyProvider.Get()
        {


            if (Get != null)
            {
                return Get();
            } else if (_inner != null)
            {
                return ((IConfirmitClientKeyProvider)_inner).Get();
            }

            return default(string);
        }

    }
}