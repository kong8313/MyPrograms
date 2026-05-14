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

namespace TreadTest
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();           
        }

        public static ManualResetEvent Stop = new ManualResetEvent(false);
        public static ManualResetEvent Start = new ManualResetEvent(false);

        private int SumA(int[] a, ref int c)
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
        }

        unsafe private int SumB(int a, ref int c)
        {            
            long size;
            int* p1 = &a;
            {
                fixed (int* p2 = &c)
                {
                    size = p2 - p1;
                }
            
            }
            if (size < c) return(SumB(1, ref c));
            return (a);
        }


        private void ThreadProc(object obj)
        {
            int stackUsage = (int)obj;
            bool bRun = true;
            Start.WaitOne();
            bool stackOnce = true;
            while (bRun)
            {
                Thread.Sleep(0);
                int[] ar = new int[1];
                int r = stackUsage;
                if (stackOnce)
                {
                    SumA(ar, ref r);
                    stackOnce = false;
                }
                //SumB(2, ref r);
                bRun = !Stop.WaitOne(1000);
            };
    
        }

        private List<Thread> threads = new List<Thread>();


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
                Thread th = new Thread(ThreadProc);                
                th.IsBackground = true;
                threads.Add(th);
                th.Start(stackUsage);
                pbProgress.Value = i + 1;
                Application.DoEvents();
            }
        }

        private void btnDeleteClick(object sender, EventArgs e)
        {
            Stop.Set();
            Start.Set();
            pbProgress.Maximum = threads.Count;
            pbProgress.Value = 0;
            int i = 0;
            foreach (Thread th in threads)
            {
                i++;
                pbProgress.Value = i;
                Application.DoEvents();
                th.Join(2000);
            }
            threads.Clear();
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

    }
}
