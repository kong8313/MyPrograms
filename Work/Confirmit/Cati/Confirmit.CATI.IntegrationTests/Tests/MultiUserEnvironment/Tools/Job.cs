using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Confirmit.CATI.IntegrationTests.Tests.MultiUserEnvironment.Tools
{
    public delegate void threadFunc(params object[] parameters);

    public class Job
    {
        private Thread[] threads;
        private threadFunc function;
        private object[] parameters;
        private int repeatCount = -1;

        private List<Exception> exceptions = new List<Exception>();

        public List<Exception> Exceptions
        {
            get { return exceptions; }
        }

        private object lockObj = new object();

        ManualResetEvent startJobEvent = new ManualResetEvent(false);
        private volatile bool shouldStop = false;

        public int RepeatCount
        {
            get { return repeatCount; }
            set { repeatCount = value; }
        }

        public int ThreadCount { get; set; }

        /// <summary>
        /// in ms
        /// </summary>
        public int RunTime { get; set; }

        public Job(
            threadFunc function,
            params object[] parameters)
        {
            this.function = function;
            this.parameters = parameters;
            ThreadCount = 1;
        }

        public Job(
            threadFunc function,
            int threadCount,
            int runTime,
            params object[] parameters)
        {
            this.function = function;
            ThreadCount = threadCount;
            RunTime = runTime;
            this.parameters = parameters;
        }

        private void JobFunctionThread()
        {
            startJobEvent.WaitOne();
            while (true)
            {
                lock (lockObj)
                {
                    if (!shouldStop && RepeatCount != -1)
                        shouldStop = (RepeatCount--) <= 0;
                    if (shouldStop)
                        break;
                }
                try
                {
                    function(parameters);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());

                    lock (((ICollection)exceptions).SyncRoot)
                    {
                        exceptions.Add(ex);
                    }
                }
            }
        }

        public void StartJobThread()
        {
            //if RunTime 0 and not set count of iterations
            //we assume that test on 1 iteration
            if (RunTime == 0 && RepeatCount == -1)
                RepeatCount = 1;

            threads = new Thread[ThreadCount];
            //testmode: false - all thread will be processed until specific time is not ended
            //          true - RuningThreadFunc will be executed RepeatCount time
            bool testMode = (RepeatCount == -1);

            for (int i = 0; i < ThreadCount; ++i)
            {
                threads[i] = new Thread(JobFunctionThread);
            }

            foreach (Thread thread in threads)
            {
                thread.Start();
            }

            startJobEvent.Set();

            if (testMode)
            {
                Thread.Sleep(RunTime);

                shouldStop = true;
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }
    }
}