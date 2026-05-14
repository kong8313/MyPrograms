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

namespace StartStopTest
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();           
        }

        //IStartStopTest testObj = new StartStop<System.Timers.Timer>(new TimersTimer());
        //IStartStopTest testObj = new StartStop<System.Threading.Timer>(new ThreadingTimer());
        //IStartStopTest testObj = new StartStop<System.Threading.Timer>(new SqlThreadingTimer());
        IStartStopTest testObj;// = new StartStop<TestThread>(new SqlThread());
        

        private void btnCreateClick(object sender, EventArgs e)
        {
            if (testObj != null)
            {
                btnDeleteClick(sender, e);
            }
            Globals.RandomStart = cbRandomStart.Checked;
            switch (cbType.SelectedIndex)
            { 
                case 0:
                {
                    testObj = new StartStop<TestThread>(new SqlThread());
                    break;
                }
                case 1:
                {
                    testObj = new StartStop<System.Threading.Timer>(new SqlThreadingTimer());
                    break;
                }
                case 2:
                {
                    testObj = new StartStop<System.Threading.Timer>(new ThreadingTimer());
                    break;
                }
                case 3:
                {
                    testObj = new StartStop<System.Timers.Timer>(new TimersTimer());
                    break;
                }
                case 4:
                {
                    testObj = new StartStop<JustThread>(new TestJustThread());
                    break;
                }

                default:
                {
                    MessageBox.Show("Wrong type!");
                    break;
                }
            }

            Globals.Period = (int)nStackUsage.Value*1000;
            int count = (int)nCount.Value;
            pbProgress.Maximum = count;
            pbProgress.Value = 0;
            testObj.Create(count, (c, i) => 
                {
                    pbProgress.Value = i + 1;
                    Application.DoEvents();
                }
            );
        }

        private void btnDeleteClick(object sender, EventArgs e)
        {
            pbProgress.Maximum = 0;
            pbProgress.Value = 0;
            testObj.Delete((c, i) =>
                {
                    pbProgress.Maximum = c;
                    pbProgress.Value = i + 1;
                    Application.DoEvents();
                }
            );
            testObj = null;
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            pbProgress.Maximum = 0;
            pbProgress.Value = 0;
            testObj.Start((c, i) =>
                {
                    pbProgress.Maximum = c;
                    pbProgress.Value = i + 1;
                    Application.DoEvents();
                }
            );
        }

        private void btnClearInfos_Click(object sender, EventArgs e)
        {
            lbInfos.Items.Clear();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            pbProgress.Maximum = 0;
            pbProgress.Value = 0;
            testObj.Stop((c, i) =>
                {
                    pbProgress.Maximum = c;
                    pbProgress.Value = i + 1;
                    Application.DoEvents();
                }
            );
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GC.Collect();
        }

        private void btnThreadPoolInfo_Click(object sender, EventArgs e)
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
            
            lbInfos.Items.Add(String.Format(
                "w:{0}|{1}|{2} c:{3}|{4}|{5}",
                worker, maxWorker, minWorker,
                completion, maxCompletion, minCompletion
                )
            );
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cbType.SelectedIndex = 0;
        }

    }
}
