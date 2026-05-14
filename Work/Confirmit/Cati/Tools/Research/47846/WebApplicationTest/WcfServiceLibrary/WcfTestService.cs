using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace WcfServiceLibrary
{
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    //[IPFilterBehavior]
    public class WcfTestService : IWcfTestService, IDisposable
    {
        public static long myInt = DateTime.UtcNow.Ticks;

        private void Log(string msg)
        {
            string s = String.Format(
                "m'{0}' {1}",
                msg,
                GetIds()
                );
            Debugger.Log(1, "Service1", s);
            Debug.WriteLine(s);
        }

        public WcfTestService()
        {
            Log("WcfTestService()");
        }

        ~WcfTestService()
        {
            Log("~WcfTestService()");
        }

        #region IService1 Members

        public string GetIds()
        {
            string s = String.Format(
                "t'{0}', i'{1}', n'{2}'",
                Thread.CurrentThread.ManagedThreadId,
                myInt.ToString(),
                AppDomain.CurrentDomain.FriendlyName);
            return (s);
        }

        public string AppDomainName()
        {
            return AppDomain.CurrentDomain.ToString();
        }

        public void StartGC()
        {
            GC.Collect();
        }

        public string GetThreadPoolInfo()
        {
            int worker;
            int completion;
            int maxWorker;
            int maxCompletion;
            int minWorker;
            int minCompletion;
            ThreadPool.GetAvailableThreads(out worker, out completion);
            ThreadPool.GetMaxThreads(out maxWorker, out maxCompletion);
            ThreadPool.GetMinThreads(out minWorker, out minCompletion);

            return (String.Format(
                "w:{0}|{1}|{2} c:{3}|{4}|{5}",
                worker, maxWorker, minWorker,
                completion, maxCompletion, minCompletion
                )
            );
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Log(String.Format("Dispose()"));
        }

        #endregion

    }
}
