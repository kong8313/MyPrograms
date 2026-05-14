using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Timers;

namespace StartStopTest
{
    public class TimersTimer : IStartStopTest<System.Timers.Timer>
    {
        #region IStartStopTest<Timer> Members


        public System.Timers.Timer CreateTestObject()
        {
            System.Timers.Timer t = new System.Timers.Timer(Globals.Period);
            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            return (t);
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine(
                String.Format(
                "id:{0} p:{1} t:{2}",                
                Thread.CurrentThread.ManagedThreadId,
                Thread.CurrentThread.IsThreadPoolThread,
                e.SignalTime.ToString()
                )
                );
            Thread.Sleep(1000);
        }

        public void ClearTestObject(System.Timers.Timer obj)
        {
            obj.Stop();
            obj.Dispose();
        }

        public void Start(System.Timers.Timer obj)
        {
            obj.Start();
        }

        public void Stop(System.Timers.Timer obj)
        {
            obj.Stop();
        }

        #endregion
    }
}
