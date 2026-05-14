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
    partial class Service2 : ServiceBase
    {
        public Service2()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Debug.WriteLine("Service2 is started.");
        }

        protected override void OnStop()
        {
            Debug.WriteLine("Service2 is stopped.");
        }

        protected override void OnContinue()
        {
            throw new Exception("On Continue");
            Debug.WriteLine("Service2 OnContinue.");
            base.OnContinue();
        }

        protected override void  OnPause()
        {
            throw new Exception("On Continue");
            Debug.WriteLine("Service2 OnPause.");
            base.OnPause();
        }

    }
}
