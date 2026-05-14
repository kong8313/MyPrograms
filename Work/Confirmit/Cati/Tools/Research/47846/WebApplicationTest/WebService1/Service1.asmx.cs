using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace WebService1
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {

        private string GetIds()
        {
            string s = String.Format(
                "t'{0}', i'{1}', n'{2}'",
                Thread.CurrentThread.ManagedThreadId,
                myInt.ToString(),
                AppDomain.CurrentDomain.FriendlyName);
            return(s);
        }

        [DllImport("Kernel32")]
        public static extern void OutputDebugStringW
            ([In, MarshalAs(UnmanagedType.LPWStr)] string message);

        private void Log(string msg)
        {
            string s = String.Format(
                "m'{0}' {1}",
                msg,
                GetIds()
                );
            Debugger.Log(1, "Service1", s);
            OutputDebugStringW("Service1:" + s);
            Debug.WriteLine(s);
        }

        public Service1()
        {
            Log("Service1()");
        }

        ~Service1()
        {
            Log("~Service1()");
        }

        protected override void Dispose(bool disposing)
        {
            Log(String.Format("Dispose({0})", disposing));
            base.Dispose(disposing);
        }

        public static long myInt = DateTime.UtcNow.Ticks;

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public string AppDomainName()
        {
            return AppDomain.CurrentDomain.ToString();
        }

        [WebMethod]
        public void StartGC()
        {
            GC.Collect();
        }

        [WebMethod]
        public string AppDomainId()
        {
            return AppDomain.CurrentDomain.Id.ToString();
        }

        [WebMethod]
        public string AppDomainStaticId()
        {
            return myInt.ToString();
        }

        [WebMethod]
        public void ExceptionTest()
        {
            throw new Exception("Test exception");
        }

        [WebMethod]
        public void ExceptionTestInThreadPool()
        {
            ThreadPool.QueueUserWorkItem(WaitCallback);
        }

        private static void WaitCallback(object state)
        {
            throw new Exception("test 2");
        }

        [WebMethod]
        public void ExceptionTestInThread()
        {
            Thread t = new Thread(WaitCallback);
            t.Start(null);            
        }


        [WebMethod]
        public void StartThread()
        {
            Thread t = new Thread(ThreadProc);
            t.Start(null);
        }

        private static void ThreadProc(object state)
        {
            while (true) 
            {
                Thread.Sleep(1000);
            }
        }


        [WebMethod]
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

            return(String.Format(
                "w:{0}|{1}|{2} c:{3}|{4}|{5}",
                worker, maxWorker, minWorker,
                completion, maxCompletion, minCompletion
                )
            );        
        }


        [WebMethod]
        public string SetThreadPoolInfo()
        {
            int worker;
            int completion;
            int maxWorker = 100;
            int maxCompletion = 50;
            int minWorker= 1;
            int minCompletion = 1;
            ThreadPool.SetMaxThreads(maxWorker, maxCompletion);
            ThreadPool.SetMinThreads(minWorker, minCompletion);            
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

        [WebMethod]
        public void SetMaxThreadPool(int maxWorker, int maxCompletion)
        {
            ThreadPool.SetMaxThreads(maxWorker, maxCompletion);
        }

    }
}
