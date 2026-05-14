using System;
using System.Diagnostics;
using System.Threading;
using System.Web;

using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Classes
{
    public static class ExceptionTraceHelper
    {
        /// <summary>
        /// Key to use for error message to show in error page in request parameters.
        /// </summary>
        public const string ErrorMessageKey = "Message";

        /// <summary>
        /// Error page file name.
        /// </summary>
        private const string ErrorPageAspx = "ErrorPage.aspx";

        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        public static void TraceException(Exception ex)
        {
            if (ex is UserMessageException || IsRequestTimeout(ex) || IsCommunicationError(ex))
            {
                Trace.TraceWarning(ex.ToString());
            }
            else
            {
                if (Is0X80070057Error(ex) == false)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Determines whether the exception is a thrown by System.Web.Hosting.IIS7WorkerRequest.RaiseCommunicationError method.
        /// </summary>
        /// <example>
        /// System.Web.HttpException (0x80004005): An error occurred while communicating with the remote host. The error code is 0x800703E3. ---> 
        /// System.Runtime.InteropServices.COMException (0x800703E3): The I/O operation has been aborted because of either a thread exit or an application request. (Exception from HRESULT: 0x800703E3)
        ///     at System.Web.Hosting.IIS7WorkerRequest.RaiseCommunicationError(Int32 result, Boolean throwOnDisconnect)
        ///     at System.Web.Hosting.IIS7WorkerRequest.ReadEntityCoreSync(Byte[] buffer, Int32 offset, Int32 size)
        ///     at System.Web.HttpRequest.GetEntireRawContent()
        ///     at System.Web.HttpRequest.FillInFormCollection()
        ///     at System.Web.HttpRequest.get_Form()
        ///     at System.Web.HttpRequest.get_HasForm()
        ///     at System.Web.UI.Page.GetCollectionBasedOnMethod(Boolean dontReturnNull)
        ///     at System.Web.UI.Page.DeterminePostBackMode()
        ///     at System.Web.UI.Page.ProcessRequestMain(Boolean includeStagesBeforeAsyncPoint, Boolean includeStagesAfterAsyncPoint)
        /// </example>
        private static bool IsCommunicationError(Exception ex)
        {
            return ex is HttpException &&
                   ex.ToString().Contains("System.Web.Hosting.IIS7WorkerRequest.RaiseCommunicationError");
        }

        /// <summary>
        /// Determines whether the exception is the 0x80070057 error that
        /// probably occurs when connection is closed during page transmission to client.
        /// </summary>
        /// <param name="ex">The exception to check.</param>
        private static bool Is0X80070057Error(Exception ex)
        {
            return ex is HttpException &&
                   ex.Message.Equals(
                       "An error occurred while communicating with the remote host. The error code is 0x80070057.",
                       StringComparison.InvariantCultureIgnoreCase);
        }

        /// <example>
        /// System.Web.HttpException (0x80004005): Request timed out.
        /// </example>
        /// <remarks>
        /// http://stackoverflow.com/questions/4929115/any-progress-on-diagnosing-request-timed-out-httpexceptions
        /// </remarks>
        private static bool IsRequestTimeout(Exception ex)
        {
            return ex is HttpException &&
                   ex.Message.Equals(
                       "Request timed out.",
                       StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Gets the error message from exception. Default error message is 'Internal Server Error'.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>Error message to show to user</returns>
        public static string GetUserErrorMessageFromException(Exception exception)
        {
            string errorMessage = Strings.InternalServerError;

            if (exception is UserMessageException || Config.DebugMode)
            {
                errorMessage = exception.Message;
            }

            return errorMessage;
        }

        /// <summary>
        /// Shows and logs server error.
        /// </summary>
        public static void ShowServerError()
        {
            HttpServerUtility server = HttpContext.Current.Server;
            Exception ex = server.GetLastError() ?? HttpContext.Current.Error;
            if (ex == null)
                return;

            if (ex is ThreadAbortException)
            {
                server.ClearError();
                return;
            }
            if (ex is HttpUnhandledException && ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            TraceException(ex);
            server.ClearError();
            HttpContext.Current.ClearError();
            HttpContext.Current.Response.StatusCode = 500;
            ShowErrorPage(GetUserErrorMessageFromException(ex));
        }

        public static bool IsErrorPage()
        {
            return HttpContext.Current.Request.Path.Contains(ErrorPageAspx);
        }

        public static void TraceWarning(string text, params object[] args)
        {
            Trace.TraceWarning(string.Format(text, args));
        }

        /// <summary>
        /// Shows error message in error page.
        /// </summary>
        /// <param name="errorMessage">Error message that should be shown.</param>
        public static void ShowErrorPage(string errorMessage)
        {
            Trace.TraceError(errorMessage);

            // Do not redirect to error page from error page.
            if (IsErrorPage())
            {
                Trace.TraceError("Error during processing request to error page: \r\n{0}", errorMessage);
                return;
            }

            string url = BaseForm.BaseRelativePath(String.Format(ErrorPageAspx + "?{0}={1}", ErrorMessageKey, HttpUtility.UrlEncode(errorMessage)));

            try
            {
                // Server.Transfer is not working at Page_Init when session state is not initialized.
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Server.Transfer(url);
                }
                else
                {
                    HttpContext.Current.Response.Redirect(url);
                }
            }
            catch (Exception ex)
            {
                if (!(ex is ThreadAbortException))
                {
                    HttpContext.Current.Response.Write(String.Format("<script>document.URL='{0}';</script>", url));
                }
            }
        }        
    }
}
