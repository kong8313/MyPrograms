using System;

namespace Confirmit.CATI.Supervisor.Classes
{
    public class FileToBrowserSender : IFileToBrowserSender
    {
        /// <summary>
        /// Sends bytes as a file to client.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="buffer">Bytes to send.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="sendInline">If set to true the file will be transfered with inline content-desposition header.</param>
        public void Send(BaseForm page, byte[] buffer, string fileName, bool sendInline)
        {
            string pathWithParams = $"FileExport.ashx?filename={Uri.EscapeDataString(fileName)}";

            if (sendInline)
            {
                pathWithParams += "&inline=true";
            }

            page.Session[fileName] = buffer;

            /* Export delay in 100 ms has been introduced to prevent problems with initialization of Infragistics' controls in IE8/9 */
            page.RegisterStartupScript(string.Format(
                "setTimeout(function(){{Y.one('body').append(\"<iframe id='{0}' src='{1}' style='position: absolute; top:0px; height:1px; width:1px; visibility: hidden;'></iframe>\");}}, 100);",
                page.FileElementId,
                BaseForm.BaseRelativePath(pathWithParams)));
        }
    }
}