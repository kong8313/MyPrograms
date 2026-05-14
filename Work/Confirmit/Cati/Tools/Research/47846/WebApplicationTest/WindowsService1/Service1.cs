using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Debug.WriteLine("Service1 is started.");
        }

        protected override void OnStop()
        {
            Debug.WriteLine("Service1 is stopped.");
        }

        protected override void OnContinue()
        {
            throw new Exception("On Continue");
            Debug.WriteLine("Service1 OnContinue.");
            base.OnContinue();
        }

        protected override void OnPause()
        {
            throw new Exception("On Continue");
            Debug.WriteLine("Service1 OnPause.");
            base.OnPause();
        }

    }
}
