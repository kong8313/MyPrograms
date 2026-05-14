using System;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.SessionState;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor
{
    /// <summary>
    /// Summary description for FileExport
    /// </summary>
    public class FileExport : IHttpHandler, IRequiresSessionState
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        /// <summary>
        /// Process current request
        /// Write file from session into response
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            string filename = context.Request.Params["filename"];

            bool isInline = false;

            if (context.Request.Params["inline"] != null)
            {
                bool.TryParse(context.Request.Params["inline"], out isInline);
            }
            
            if (!String.IsNullOrEmpty(filename))
            {
                var buffer = (byte[])context.Session[filename];

                context.Response.Clear();

                var encodedFileName = HttpUtility.UrlPathEncode(filename);
                var contentDispositionType = isInline ? "inline" : "attachment";

                context.Response.AddHeader("content-disposition", String.Format("{0};filename={1}", contentDispositionType, encodedFileName));                

                context.Response.ContentType = GetContentTypeByExtension(Path.GetExtension(filename));
                context.Response.AddHeader("content-length", buffer.Length.ToString(CultureInfo.InvariantCulture));

                context.Response.OutputStream.Write(buffer, 0, buffer.Length);

                context.Response.Flush();
                context.Response.End();

                context.Session.Remove(filename);
            }
            else
            {
                context.Response.Write(Strings.InvalidFileName);
            }
        }

        private string GetContentTypeByExtension(string fileExtension)
        {
            var extension = fileExtension.ToLower();
            switch (extension)
            {
                case ".log":
                    return "text/plain";
                case ".txt":
                    return "text/plain";
                case ".zip":
                    return "application/zip, application/octet-stream, application/x-zip-compressed";
                case ".pdf":
                    return "application/pdf";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                default:
                    return "application/octet-stream";
            }
        }
    
        #endregion
    }
}