using System;
using System.Threading;

namespace Confirmit.CATI.Core.Services.WaitingService
{
    public interface IWaitingService
    {
        bool Wait(ManualResetEvent stopEvent,int period);

        bool Wait(ManualResetEvent stopEvent, TimeSpan period);
    }
}