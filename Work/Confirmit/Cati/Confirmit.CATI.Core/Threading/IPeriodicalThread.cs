using System;

using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Threading
{
    public interface IPeriodicalThread
    {
        string ThreadName { get; }

        TimeSpan StopTimeout { get; }

        TimeSpan SleepTimeout { get; }

        ISystemSettings SystemSettings { get; }

        ISideBySideManager SideBySideManager { get; }

        void Start();
        void Start(object parameter);

        bool Wait(int period);
        bool Wait(TimeSpan period);

        void Stop();

        void OnStop();
    }
}