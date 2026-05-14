using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Diagnostics;
using StartStopTest;

namespace WebService1
{
    public class Global : System.Web.HttpApplication
    {
        IStartStopTest testObj = //new StartStop<System.Threading.Timer>(new SqlThreadingTimer());
                                    new StartStop<TestThread>(new SqlThread());

        protected void Application_Start(object sender, EventArgs e)
        {            
            Debug.WriteLine("AS");
            Globals.RandomStart = true;
            Service1.myInt = Service1.myInt & 0xFFFF;
            testObj.Create(10, null);
            testObj.Start(null);
        }

        protected void Session_Start(object sender, EventArgs e)
        {            
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Debug.WriteLine("BR");
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            Debug.WriteLine("AR");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Debug.WriteLine("E");
        }

        protected void Session_End(object sender, EventArgs e)
        {
            Debug.WriteLine("SE");
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Debug.WriteLine("AE");
            Service1.myInt = 0;
            testObj.Stop(null);
            testObj.Done();
            GC.Collect();
        }
    }
}