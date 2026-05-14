using System;
using System.Text.RegularExpressions;
using System.Web;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Supervisor.Core.Security;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.Security.Crypto.Web;

namespace Confirmit.CATI.Supervisor
{
    public class InitialParameters
    {
        public StartPages StartPage = StartPages.CATIsurvey;
        public string ProjectId;
    }


    public partial class Main : BaseForm
    {
        private readonly ISurveyPermissionProvider _surveyPermissionProvider;

        public Main()
        {
            _surveyPermissionProvider = ServiceLocator.Resolve<ISurveyPermissionProvider>();
        }

        public override string Title
        {
            get
            {
                return "CATI Supervisor";
            }
        }

        private InitialParameters GetInitialParameters()
        {
            var sid = Request.QueryString["sid"];

            var parameters = new InitialParameters();

            if (!string.IsNullOrEmpty(Request.Params["startPage"]))
            {
                int startPage;
                int.TryParse(Request.Params["startPage"], out startPage);

                parameters.StartPage = (StartPages) startPage;
            }

            if (!string.IsNullOrEmpty(Request.Params["projectid"]))
            {
                parameters.ProjectId = Request.Params["projeciId"];
            }

            if (!string.IsNullOrEmpty(sid))
            {
                // If supervisor is opened from Confirmit, Confirmit might pass params in the encrypted sid
                var decryptedSid = EncryptionUsingMachineKey.Decrypt(DataProtection.All, Request.QueryString["sid"]);
                parameters.ProjectId = Regex.Match(decryptedSid, @"(?<=projectid=).*?(?=&|$)", RegexOptions.IgnoreCase).Value;
            }

            return parameters;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                InitialSurveyHelper.HasSurveyBeenShown = false;
                
                var parameters = GetInitialParameters();

                if (parameters.StartPage == StartPages.ProductivityReport)
                {
                    Response.Redirect(BaseRelativePath("Reports/ProductivityReport.aspx"));
                }
                else
                {
                    _surveyPermissionProvider.InitUserSurveyPermissions(User.Name, Int32.Parse(User.Company));
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            PageHelper.RegisterClientLibrary("Client/WindowManager.js");
        }
    }
}
