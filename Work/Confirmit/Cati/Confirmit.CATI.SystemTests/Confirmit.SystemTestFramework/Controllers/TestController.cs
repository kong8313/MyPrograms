using Confirmit.SystemTestFramework.Settings;

namespace Confirmit.SystemTestFramework.Controllers
{
    public abstract class TestController
    {
        protected UserInfo UserInfo;

        protected TestController()
        {
            UserInfo = new UserInfo();
        }
    }
}