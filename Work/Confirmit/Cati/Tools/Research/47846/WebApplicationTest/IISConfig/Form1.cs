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

using Web=Microsoft.Web.Administration;
using IISConfig.ServiceReference1;
using System.Net;

namespace IISConfig
{
    public partial class Form1 : Form
    {
        const string WebSiteName = "MyTest Web Site";
        const string WebSitePath = @"C:\Lebed\Research\MyTestWebSite\ws";
        const string WebSitePoo = "MyTest Web Site";

        const string WebAppPref = "/ws";

        public Form1()
        {
            InitializeComponent();           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Web.ServerManager iisManager = new Web.ServerManager();
            Web.Site site = iisManager.Sites[WebSiteName];

            int count = (int)nCount.Value;
            pbProgress.Maximum = count;
            pbProgress.Value = 0;
            for (int i = 0; i < count; i++ )
            {
                Web.Application wa = site.Applications[WebAppPref + i.ToString()];
                if (wa != null)
                {
                    site.Applications.Remove(wa);
                }
                wa = site.Applications.Add(WebAppPref + i.ToString(), WebSitePath);
                wa.ApplicationPoolName = WebSitePoo;
                pbProgress.Value = i + 1;
                Application.DoEvents();
            }
            iisManager.CommitChanges();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Web.ServerManager iisManager = new Web.ServerManager();
            Web.Site site = iisManager.Sites[WebSiteName];

            int count = (int)nCount.Value;
            pbProgress.Maximum = count;
            pbProgress.Value = 0;
            for (int i = 0; i < count; i++)
            {
                Web.Application wa = site.Applications[WebAppPref + i.ToString()];
                if (wa != null)
                {
                    site.Applications.Remove(wa);
                }
                pbProgress.Value = i + 1;
                Application.DoEvents();
            }
            
            iisManager.CommitChanges();
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            Web.ServerManager iisManager = new Web.ServerManager();
            Web.Site site = iisManager.Sites[WebSiteName];

            int count = 0;            
            var apps = (from a in iisManager.Sites[WebSiteName].Applications
                       where a.Path.StartsWith(WebAppPref)
                       select new { App = a, Count = ++count }).ToList();
            pbProgress.Maximum = apps.Count();
            pbProgress.Value = 0;
            int i = 0;
            foreach (var ap in apps)
            {
                Web.Application wa = ap.App;
                if (wa != null)
                {
                    site.Applications.Remove(wa);
                }                                
                i++;
                pbProgress.Value = i;
                Application.DoEvents();
                
            }

            iisManager.CommitChanges();
        }

        [DllImport("Kernel32")]
        public static extern void OutputDebugStringW 
            ([In, MarshalAs(UnmanagedType.LPWStr)] string message);


        private void btnDebugOutTest_Click(object sender, EventArgs e)
        {
            //Debugger.Log(1, Application.ProductName, "Test!");
            OutputDebugStringW("test1");
        }

        private void ConnectToApplication(int num, string name)
        {
            Service1SoapClient sc = new Service1SoapClient("", String.Format("http://localhost:1234{0}/Service1.asmx", name));
            string s = String.Format(
                "n'{0}' i'{1}' n'{2}'",
                num,
                sc.AppDomainStaticId(),
                sc.AppDomainName());
            lbInfos.Items.Add(s);
        }

        private void ConnectToWebApplication(int num, string name)
        {
            using (WebClient wc = new WebClient())
            { 
                string r = wc.DownloadString(String.Format("http://localhost:1234{0}/2.aspx", name));
                string s = String.Format(
                    "n'{0}' n'{1}'",
                    num,
                    r);
                lbInfos.Items.Add(s);
            };
            
        }

        private delegate void ConnectTo(int num, string name);

        private void DoConnect(ConnectTo connectTo)
        {
            Web.ServerManager iisManager = new Web.ServerManager();
            Web.Site site = iisManager.Sites[WebSiteName];

            int count = 0;
            var apps = (from a in iisManager.Sites[WebSiteName].Applications
                        where a.Path.StartsWith(WebAppPref)
                        select new { App = a, Count = ++count }).ToList();
            pbProgress.Maximum = apps.Count();
            pbProgress.Value = 0;
            int i = 0;
            foreach (var ap in apps)
            {
                Web.Application wa = ap.App;
                if (wa != null)
                {
                    connectTo(i, wa.Path);
                }
                i++;
                pbProgress.Value = i;
                Application.DoEvents();

            }        
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            DoConnect(ConnectToApplication);
        }

        private void btnClearInfos_Click(object sender, EventArgs e)
        {
            lbInfos.Items.Clear();
        }

        private void btnConnectApplication_Click(object sender, EventArgs e)
        {
            DoConnect(ConnectToWebApplication);
        }
    }
}
