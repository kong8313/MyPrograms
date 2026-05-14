using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace StartStopTest
{
    public class JustThread 
    {
        public ManualResetEvent Stop = new ManualResetEvent(false);
        public ManualResetEvent Start = new ManualResetEvent(false);

        internal Thread thread = new Thread(t_Elapsed);

        public JustThread()
        {            
            thread.IsBackground = false;            
            thread.Start(this);
        }

        static void t_Elapsed(object obj)
        {
            try
            {
                bool bRun = true;
                ((JustThread)obj).Start.WaitOne();
                int delay = Globals.GetStartDelay();
                ((JustThread)obj).Stop.WaitOne(delay);
                while (bRun)
                {
                    try
                    {
                        using (TestDispose d = new TestDispose())
                        {
                            Thread.Sleep(10000);
                        }
                        Debug.WriteLine(
                            String.Format(
                            "id:{0} p:{1} t:{2}",
                            Thread.CurrentThread.ManagedThreadId,
                            Thread.CurrentThread.IsThreadPoolThread,
                            DateTime.Now.ToString()
                            )
                            );
                        
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                    bRun = !((JustThread)obj).Stop.WaitOne(Globals.Period);
                };
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

    }

    public class TestJustThread : IStartStopTest<JustThread>
    {
        #region IStartStopTest<JustThread> Members


        public JustThread CreateTestObject()
        {
            JustThread t = new JustThread();
            return (t);
        }


        public void ClearTestObject(JustThread obj)
        {
            Start(obj);
            Stop(obj);
            obj.thread.Join(5000);
            obj.thread.Abort();
        }

        public void Start(JustThread obj)
        {
            obj.Start.Set();
        }

        public void Stop(JustThread obj)
        {
            //obj.thread.Abort();
            obj.Stop.Set();
        }

        #endregion
    }

    public class TestDispose : IDisposable
    {
        ~TestDispose()
        {
            Debug.WriteLine("~");
        }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
