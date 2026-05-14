using System.Collections.Generic;

using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Telephony;

namespace Confirmit.CATI.Core.Telephony
{
    public interface IDialerCollection
    {
        IEnumerable<IDialerInstance> GetDialers();
        IEnumerable<IDialerInstance> GetDialers(DialType dialType);
        IEnumerable<IDialerInstance> GetInitializedDialers(DialType dialType);

        int[] GetDialerIds(DialType dialType);

        IDialerInstance GetDialerById(int dialerId);

        IDialerAPI FirstLoadedDialerApi { get; }
        IDialerInstance GetFirstInitializedDialer(DialType dialType);

        bool IsDialerInitialized(int dialerId);

        void InitializeCollection();

        bool InitializedDialerExists();
        bool InitializedDialerExists(DialType dialType);
    }
}