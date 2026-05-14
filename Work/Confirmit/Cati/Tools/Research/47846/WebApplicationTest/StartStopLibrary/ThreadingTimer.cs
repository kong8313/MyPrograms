using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace StartStopTest
{
    public class ThreadingTimer : IStartStopTest<System.Threading.Timer>
    {
        #region IStartStopTest<Timer> Members


        public System.Threading.Timer CreateTestObject()
        {
            System.Threading.Timer t = new System.Threading.Timer(new TimerCallback(t_Elapsed), null, Timeout.Infinite, Timeout.Infinite);
            return (t);
        }

        void t_Elapsed(object obj)
        {
            try
            {
                Debug.WriteLine(
                    String.Format(
                    "id:{0} p:{1} t:{2}",
                    Thread.CurrentThread.ManagedThreadId,
                    Thread.CurrentThread.IsThreadPoolThread,
                    DateTime.Now.ToString()
                    )
                    );
                Thread.Sleep(10000);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void ClearTestObject(System.Threading.Timer obj)
        {
            obj.Change(Timeout.Infinite, Timeout.Infinite);
            obj.Dispose();
        }

        public void Start(System.Threading.Timer obj)
        {
            int delay = Globals.GetStartDelay();
            obj.Change(delay, Globals.Period);
        }

        public void Stop(System.Threading.Timer obj)
        {
            obj.Change(Timeout.Infinite, Timeout.Infinite);
        }

        #endregion
    }
}
