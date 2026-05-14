using System;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Telephony;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubIDialerInitializer : IDialerInitializer 
    {
        private IDialerInitializer _inner;

        public StubIDialerInitializer()
        {
            _inner = null;
        }

        public IDialerInitializer Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IDialerAPI CreateInstanceDelegate();
        public CreateInstanceDelegate CreateInstance;

        IDialerAPI IDialerInitializer.CreateInstance()
        {


            if (CreateInstance != null)
            {
                return CreateInstance();
            } else if (_inner != null)
            {
                return ((IDialerInitializer)_inner).CreateInstance();
            }

            return default(IDialerAPI);
        }

        public delegate IDialerAPI InitializeDialerInt32IDialerAPIBooleanInt32OutStringOutDialTypeOutDelegate(int dialerId, IDialerAPI dialerApi, bool sendInitializeToWebService, out int tenantId, out string name, out DialType dialType);
        public InitializeDialerInt32IDialerAPIBooleanInt32OutStringOutDialTypeOutDelegate InitializeDialerInt32IDialerAPIBooleanInt32OutStringOutDialTypeOut;

        IDialerAPI IDialerInitializer.InitializeDialer(int dialerId, IDialerAPI dialerApi, bool sendInitializeToWebService, out int tenantId, out string name, out DialType dialType)
        {
            tenantId = default(int);
            name = default(string);
            dialType = default(DialType);


            if (InitializeDialerInt32IDialerAPIBooleanInt32OutStringOutDialTypeOut != null)
            {
                return InitializeDialerInt32IDialerAPIBooleanInt32OutStringOutDialTypeOut(dialerId, dialerApi, sendInitializeToWebService, out tenantId, out name, out dialType);
            } else if (_inner != null)
            {
                return ((IDialerInitializer)_inner).InitializeDialer(dialerId, dialerApi, sendInitializeToWebService, out tenantId, out name, out dialType);
            }

            return default(IDialerAPI);
        }

    }
}