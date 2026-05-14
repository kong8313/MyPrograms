using System.ServiceProcess;

namespace Confirmit.CATI.Backend
{
    internal partial class BackendWin32Service : ServiceBase
    {
        private readonly Host _host;

        public BackendWin32Service()
        {
            _host = new Host();

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _host.OnStart();
        }

        protected override void OnStop()
        {
            _host.OnStop();
        }
    }
}
