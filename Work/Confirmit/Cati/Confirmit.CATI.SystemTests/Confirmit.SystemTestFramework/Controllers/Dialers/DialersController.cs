namespace Confirmit.SystemTestFramework.Controllers.Dialers
{
    public class DialersController : TestController
    {
        public DialerController this[int id]
        {
            get
            {
                return new DialerController(id);
            }
        }
    }
}