using System;
using Confirmit.CATI.Supervisor.Core.Common;

namespace Confirmit.CATI.Supervisor.Core.Common.Fakes
{
    public class StubIResourceWrapper : IResourceWrapper 
    {
        private IResourceWrapper _inner;

        public StubIResourceWrapper()
        {
            _inner = null;
        }

        public IResourceWrapper Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetStringStringDelegate(string sResId);
        public GetStringStringDelegate GetStringString;

        string IResourceWrapper.GetString(string sResId)
        {


            if (GetStringString != null)
            {
                return GetStringString(sResId);
            } else if (_inner != null)
            {
                return ((IResourceWrapper)_inner).GetString(sResId);
            }

            return default(string);
        }

    }
}