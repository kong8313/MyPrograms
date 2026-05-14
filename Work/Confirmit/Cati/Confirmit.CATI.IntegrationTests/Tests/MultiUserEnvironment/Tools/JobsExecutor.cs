using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Confirmit.CATI.IntegrationTests.Tests.MultiUserEnvironment.Tools
{
    public class JobsExecutor
    {
        private Thread[] threads;

        internal Job[] Jobs { get; set; }

        public JobsExecutor(IEnumerable<Job> jobs)
        {
            Jobs = jobs.ToArray();
        }

        public void Run()
        {
            if (Jobs.Length <= 0)
                throw new Exception("List of jobs is empty");

            threads = new Thread[Jobs.Length];

            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i] = new Thread(Jobs[i].StartJobThread);
            }

            foreach (Thread thread in threads)
            {
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            string exceptionString = String.Empty;

            foreach (Job job in Jobs)
            {
                foreach (Exception ex in job.Exceptions)
                {
                    exceptionString += "Exception: " + ex + Environment.NewLine;
                }
            }

            if (exceptionString != String.Empty)
                throw new Exception(exceptionString);
        }
    }
}