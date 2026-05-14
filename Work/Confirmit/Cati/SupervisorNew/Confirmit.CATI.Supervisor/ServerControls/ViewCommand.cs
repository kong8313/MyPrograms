using System;
using System.Collections;
using System.Web;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.ServerControls.Commands;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    /// <summary>
    /// Command intended for opening view dialog in right bottom frame and floating window as well
    /// </summary>
    public class ViewCommand : Command
    {
        private string m_url;
        private string m_IDName = "ID";
        private bool m_floatingMode = false;
        private int m_width = 640;
        private int m_height = 480;
        private string m_ResCaptionName = "";

        public ViewCommand()
        {
            SelectMode = CommandGridSelectMode.SingleRow;
            IncludeParentQueryStringParams = true;
        }

        /// <summary>
        /// URL of the dialog to open
        /// </summary>
        public string URL
        {
            get
            {
                return m_url;
            }
            set
            {
                m_url = value;
            }
        }

        public bool IncludeParentQueryStringParams { get; set; }

        /// <summary>
        /// Defines, would dialog be opened in a floating window or not
        /// </summary>
        public bool FloatingMode
        {
            get
            {
                return m_floatingMode;
            }
            set
            {
                m_floatingMode = value;
            }
        }

        /// <summary>
        /// Resource identifier for window's caption (if in floating mode)
        /// </summary>
        public string WindowResCaptionName
        {
            get { return m_ResCaptionName; }
            set { m_ResCaptionName = value; }
        }

        /// <summary>
        /// Width of the dialog (if in floating mode)
        /// </summary>
        public int Width
        {
            get
            {
                return m_width;
            }
            set
            {
                m_width = value;
            }
        }

        /// <summary>
        /// Height of the dialog (if in floating mode)
        /// </summary>
        public int Height
        {
            get
            {
                return m_height;
            }
            set
            {
                m_height = value;
            }
        }

        public override string OnClientClick
        {
            get
            {
                var urlGenerator = new ScriptUrlGenerator(BaseForm.BaseRelativePath(URL));

                if (IncludeParentQueryStringParams)
                {
                    var escapedParemeters = new EscapeHelper().EscapeParameters(HttpContext.Current.Request.QueryString);
                    
                    foreach (var parameter in escapedParemeters)
                    {
                        urlGenerator.AddStaticParameter(parameter.Item1, parameter.Item2);
                    }
                }

                if (SelectMode == CommandGridSelectMode.SingleRow && Owner is GeneralGrid)
                {
                    urlGenerator.AddScriptParameter(IDName, string.Format(@"encodeURIComponent(row.get_cellByColumnKey('{0}').get_value())", IDColumnName));
                }

                if (FloatingMode)
                {
                    urlGenerator.AddStaticParameter("mode", DialogWindowMode.Floating.ToString());
                }

                string captionName = string.IsNullOrEmpty(WindowResCaptionName) ? "" : GetResString(WindowResCaptionName);

                string openDialogCode;

                string url = urlGenerator.GetResult();

                if (FloatingMode || (Owner is GeneralGrid) == false)
                {
                    openDialogCode = string.Format(
                        "GetWM().openWindow({0},'{1}','width={2}px,height={3}px,location=no,menubar=no,status=no,resizable=yes,scrollbars=yes');",
                        url,
                        captionName,
                        Width,
                        Height);
                }
                else
                {
                    openDialogCode = string.Format("window.openAndSetInfoFrame({0});", url);
                }

                return openDialogCode;
            }
        }

        /// <summary>
        /// Name of the parameter in command line of the dialog to pass identificator of current line (if called from GeneralGrid)
        /// </summary>
        public string IDName
        {
			get{return m_IDName;}
			set{m_IDName = value;}
        }

        private ArrayList m_prms = new ArrayList();
        /// <summary>
        /// Array of CommandPrm to pass additional parameters to dialog
        /// </summary>
        public ArrayList Prms
        {
            get
            {
                return m_prms;
            }
            set
            {
                m_prms = value;
            }
        }
    }
}
