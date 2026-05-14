using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

using System.Threading;

namespace AppDomainTest
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();           
        }

        public static EventWaitHandle Stop = new EventWaitHandle(false, EventResetMode.ManualReset, "czczx2311234");
        public static EventWaitHandle Start = new EventWaitHandle(false, EventResetMode.ManualReset, "czczx2311235");

        /*private int SumA(int[] a, ref int c)
        {
            if (c == 0)
            {                
                int s = 0;
                foreach (int i in a)
                {
                    s = i;
                }
                return (s);
            }
            else 
            {
                c--;
                return (SumA(a, ref c));
            }
        }*/

        private static void ThreadProc(object obj)
        {
            int stackUsage = (int)obj;
            bool bRun = true;
            Start.WaitOne();
            while (bRun)
            {
                Thread.Sleep(0);
                int[] ar = new int[1];
                int r = stackUsage;
                string s = GetThreadPoolInfo();
                Debug.WriteLine(String.Format("{0}:{1}", Thread.CurrentThread.ManagedThreadId, s));
                //SumA(ar, ref r);
                bRun = !Stop.WaitOne(1000);
                //throw new Exception("test");
            };
    
        }

        public static string GetThreadPoolInfo()
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

        private List<AppDomain> domains = new List<AppDomain>();

        private static void DoInAppDomain(string[] args)
                    {
                        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException); 
                        Thread th = new Thread(ThreadProc);
                        //th.IsBackground = true;
                        th.Name = AppDomain.CurrentDomain.FriendlyName;
                        th.Start(1);
                    }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine(e.ExceptionObject);
        }

        private void btnCreateClick(object sender, EventArgs e)
        {
            Stop.Reset();
            Start.Reset();
            int count = (int)nCount.Value;
            pbProgress.Maximum = count;
            pbProgress.Value = 0;
            int stackUsage = 1024*(int)nStackUsage.Value;

            for (int i = 0; i < count; i++)
            {
                AppDomainSetup aps = new AppDomainSetup();
                aps.AppDomainInitializer = DoInAppDomain;
                AppDomain ad = AppDomain.CreateDomain("MyDomain" + i.ToString(), null, aps);
                domains.Add(ad);
                /*Thread th = new Thread(ThreadProc);                
                th.IsBackground = true;
                threads.Add(th);
                th.Start(stackUsage);*/
                pbProgress.Value = i + 1;
                Application.DoEvents();
            }
        }

        private void btnDeleteClick(object sender, EventArgs e)
        {
            Stop.Set();
            Start.Set();
            pbProgress.Maximum = domains.Count;
            pbProgress.Value = 0;
            int i = 0;
            foreach (AppDomain ap in domains)
            {
                i++;
                pbProgress.Value = i;
                Application.DoEvents();
                AppDomain.Unload(ap);
            }
            domains.Clear();
            Start.Reset();
            Stop.Reset();
        }


        private void btnConnect_Click(object sender, EventArgs e)
        {
            Start.Set();
        }

        private void btnClearInfos_Click(object sender, EventArgs e)
        {
            lbInfos.Items.Clear();
        }

        private void btnConnectApplication_Click(object sender, EventArgs e)
        {
            Stop.Set();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GC.Collect();
        }

        private void btnTPoolEx_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(WaitCallback);
        }

        private static void WaitCallback(object state)
        {
            throw new Exception("test 2");
        }

        private void btnSetPool_Click(object sender, EventArgs e)
        {
            lbInfos.Items.Add(SetThreadPoolInfo());
        }

        public string SetThreadPoolInfo()
        {
            int worker;
            int completion;
            int maxWorker = 100;
            int maxCompletion = 50;
            int minWorker = 1;
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


    }
}
