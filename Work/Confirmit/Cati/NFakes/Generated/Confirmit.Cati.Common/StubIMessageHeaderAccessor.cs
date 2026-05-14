using System;
using Confirmit.CATI.Common.WcfTools;

namespace Confirmit.CATI.Common.WcfTools.Fakes
{
    public class StubIMessageHeaderAccessor : IMessageHeaderAccessor 
    {
        private IMessageHeaderAccessor _inner;

        public StubIMessageHeaderAccessor()
        {
            _inner = null;
        }

        public IMessageHeaderAccessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        T IMessageHeaderAccessor.GetValueFromHeader<T>(string headerName, string ns)
        {


            return default(T);
        }

    }
}