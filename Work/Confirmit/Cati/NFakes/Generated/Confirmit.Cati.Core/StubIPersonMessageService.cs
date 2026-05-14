using System;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIPersonMessageService : IPersonMessageService 
    {
        private IPersonMessageService _inner;

        public StubIPersonMessageService()
        {
            _inner = null;
        }

        public IPersonMessageService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CleanMessagesTimeSpanDelegate(TimeSpan expirationTime);
        public CleanMessagesTimeSpanDelegate CleanMessagesTimeSpan;

        void IPersonMessageService.CleanMessages(TimeSpan expirationTime)
        {

            if (CleanMessagesTimeSpan != null)
            {
                CleanMessagesTimeSpan(expirationTime);
            } else if (_inner != null)
            {
                ((IPersonMessageService)_inner).CleanMessages(expirationTime);
            }
        }

    }
}