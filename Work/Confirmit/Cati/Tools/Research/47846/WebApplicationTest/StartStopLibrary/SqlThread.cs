using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace StartStopTest
{
    public class TestThread
    {
        public ManualResetEvent Stop = new ManualResetEvent(false);
        public ManualResetEvent Start = new ManualResetEvent(false);

        internal Thread thread = new Thread(t_Elapsed);

        public TestThread()
        {
            thread.IsBackground = false;
            thread.Start(this);
        }

        static void t_Elapsed(object obj)
        {
            try
            {
                bool bRun = true;
                ((TestThread)obj).Start.WaitOne();
                int delay = Globals.GetStartDelay();
                ((TestThread)obj).Stop.WaitOne(delay);
                while (bRun)
                {
                    try
                    {
                        SQLTask.RunSqlTask();
                        Debug.WriteLine(
                            String.Format(
                            "id:{0} p:{1} t:{2}",
                            Thread.CurrentThread.ManagedThreadId,
                            Thread.CurrentThread.IsThreadPoolThread,
                            DateTime.Now.ToString()
                            )
                            );
                        //Thread.Sleep(100);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                    bRun = !((TestThread)obj).Stop.WaitOne(Globals.Period);
                };
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

    }

    public class SqlThread : IStartStopTest<TestThread>
    {
        #region IStartStopTest<TestThread> Members


        public TestThread CreateTestObject()
        {
            TestThread t = new TestThread();
            return (t);
        }


        public void ClearTestObject(TestThread obj)
        {
            Start(obj);
            Stop(obj);
            obj.thread.Join(5000);
            obj.thread.Abort();
        }

        public void Start(TestThread obj)
        {
            obj.Start.Set();
        }

        public void Stop(TestThread obj)
        {
            //obj.thread.Abort();
            obj.Stop.Set();
        }

        #endregion
    }
}
