using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Confirmit.CATI.Supervisor
{
    /// <summary>
    /// Joins several YUI JS file and returns them as a single file.
    /// </summary>
    public class YuiCombo : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            StringWriter s = new StringWriter(new StringBuilder());
            var yuiPath = "client/YUI";
            var url = HttpUtility.UrlDecode(context.Request.QueryString.ToString());

            // the list of filenames
            var files = url.Split('&').ToList();

            files.ForEach(file =>
            {
                // don't bother checking for the file if it is not valid
                if (!(file.EndsWith(".js") || file.EndsWith(".css")))
                    return;

                string path = string.Format("{0}/{1}", yuiPath, file.Substring(file.IndexOf("/build/") + 7));
                string realPath;

                try
                {
                    realPath = HttpContext.Current.Server.MapPath(path);
                }
                catch (HttpException)
                {
                    return;
                }

                if (!File.Exists(realPath)) return;
                s.WriteLine(File.ReadAllText(realPath));
            });

            // the complete javascript
            string response = s.ToString();

            var now = DateTime.Now;
            var duration = TimeSpan.FromDays(30);
            context.Response.Cache.SetMaxAge(duration);
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(now + duration);
            context.Response.Cache.SetValidUntilExpires(true);
            context.Response.Cache.SetLastModified(now);
            context.Response.Cache.VaryByParams["*"] = true;
            context.Response.Cache.SetOmitVaryStar(true);
            context.Response.ContentType = url.Contains(".css") ? "text/css" : "application/x-javascript";

            context.Response.Write(response);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}