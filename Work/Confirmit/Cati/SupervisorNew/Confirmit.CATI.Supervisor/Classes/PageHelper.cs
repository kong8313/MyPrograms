using System;
using System.Reflection;
using System.Web.UI;
using System.Web;

namespace Confirmit.CATI.Supervisor.Classes
{
    public static class PageHelper
    {
        public static readonly string VersionSuffix = "v=" + Assembly.GetExecutingAssembly().GetName().Version;

        public static void RegisterClientLibrary(string path)
        {
            var page = (Page)HttpContext.Current.CurrentHandler;
            page.ClientScript.RegisterClientScriptInclude(
                page.GetType(), path.ToLowerInvariant(), BaseForm.BaseRelativePath(path) + "?" + VersionSuffix);
        }

        public static void RegisterClientLibraryForAsyncRequest(Control control, string path)
        {
            ScriptManager.RegisterClientScriptInclude(control, 
                                                      control.GetType(), 
                                                      path.ToLowerInvariant(), 
                                                      BaseForm.BaseRelativePath(path) + "?" + VersionSuffix);
        }

        public static string GetCssPathReference(string path)
        {
            return BaseForm.BaseRelativePath(path) + "?" + VersionSuffix;
        }

        /// <summary>
        /// Produces an absolute path based on specified dir.
        /// </summary>
        /// <param name="path">The base path.</param>
        /// <param name="useSsl">if set to <c>true</c> https prefix is always set, otherwise http is used.</param>
        public static string AbsolutePath(string path, bool useSsl)
        {
            var schema = useSsl ? "https" : "http";
            
            return String.Format("{0}://{1}{2}", schema, 
                                                 HttpContext.Current.Request.Url.Host, 
                                                 BaseForm.BaseRelativePath(path));
        }
    }
}