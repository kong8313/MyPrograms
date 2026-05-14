using System.Collections.Generic;
using System.IO;
using System.Web;
using Confirmit.CATI.Supervisor.Classes.Auth;
using Confirmit.SystemTestFramework.LogOn;
using Confirmit.SystemTestFramework.Settings;
using Firmglobal.Framework.Security;

namespace Confirmit.SystemTestFramework.Controllers
{
    public class ConfirmitController : TestController
    {
        public ConfirmitSurveysController Surveys { get; }
        public CatiController Cati { get; }

        private ConfirmitController(string clientKey)
        {
            UserInfo.ClientKey = clientKey;

            Surveys = new ConfirmitSurveysController(UserInfo);
            Cati = new CatiController(UserInfo);
        }

        public static ConfirmitController Login(UserSettings settings)
        {            
            var logOnClient = new LogOnSoapClient("LogOnSoap");
            var clientKey = logOnClient.LogOnUser(settings.Login, settings.Password);

            MakeContext();

            return new ConfirmitController(clientKey);
        }

        public static void MakeContext()
        {
            ConfirmitPrincipal principal = new ConfirmitPrincipal(new List<string>(0), new ConfirmitIdentity("test", "key12321"));
            var httpRequest = new HttpRequest("", "http://test/", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            HttpContext.Current = new HttpContext(httpRequest, httpResponse)
            {
                User = principal
            };

            AuthorizationManager.SetupBackendInstance(1);
        }
    }
}