using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.ApplicationServices;
using System.Web.Management;
using System.Web.Hosting;
using Microsoft.Web.Administration;



namespace WebApplication1
{
    public class Global : System.Web.HttpApplication
    {
        int t;

        protected void Application_Start(object sender, EventArgs e)
        {
            t = 10;
            ServerManager serverManager = new ServerManager();
            Configuration config = serverManager.GetApplicationHostConfiguration();
            //this.Request.
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}