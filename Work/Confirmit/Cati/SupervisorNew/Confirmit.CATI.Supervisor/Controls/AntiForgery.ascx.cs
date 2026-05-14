using System;
using System.Security;

namespace Confirmit.CATI.Supervisor.Controls
{
    public partial class AntiForgery : System.Web.UI.UserControl
    {
        /// <summary>
        /// The name of session where the anti forgery token will be stored
        /// </summary>
        public string SessionName { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                GenerateNewToken();
        }

        public void Validate()
        {
            var stored = (Guid?)Session[SessionName];
            var sent = new Guid(AntiForgeryTokenField.Value);

            if (stored == null || sent != stored)
                throw new SecurityException("A required anti-forgery token was not supplied or was invalid");

            Session.Remove(SessionName);
        }

        /// <summary>
        /// CAll this method after validation for pages which assume multiple actions without reloading
        /// </summary>
        public void GenerateNewToken()
        {
            var antiforgeryToken = Guid.NewGuid();
            Session[SessionName] = antiforgeryToken;
            AntiForgeryTokenField.Value = antiforgeryToken.ToString();
        }
    }
}