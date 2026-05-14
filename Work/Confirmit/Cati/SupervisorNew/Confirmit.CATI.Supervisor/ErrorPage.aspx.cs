using System;
using System.Web;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor
{
    /// <summary>
    /// Error page for CATI Supervisor. 
    /// </summary>
    public partial class ErrorPage : BaseForm
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// Sets the error message text to the page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <exception cref="InvalidOperationException">Error message has not been specified.</exception>
        protected void Page_Load(object sender, EventArgs e)
        {
            string errorText = Request[ExceptionTraceHelper.ErrorMessageKey];
            if (string.IsNullOrEmpty(errorText))
            {
                throw new InvalidOperationException(Strings.ErrorMessageHasNotBeenSpecified);
            }

            errorMessage.InnerText = HttpUtility.HtmlEncode(HttpUtility.UrlDecode(errorText));
        }
    }
}