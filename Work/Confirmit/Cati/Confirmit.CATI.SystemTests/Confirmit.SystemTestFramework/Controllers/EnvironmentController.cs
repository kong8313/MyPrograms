using Confirmit.SystemTestFramework.Controllers.Dialers;

namespace Confirmit.SystemTestFramework.Controllers
{
    public class EnvironmentController : TestController
    {
        public DialersController Dialers { get; private set; }

        public EnvironmentController()
        {
            Dialers = new DialersController();
        }
    }
}
