using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.ServerControls.Confirmit;

namespace Confirmit.CATI.Supervisor.Classes.Activity
{    
    public abstract class BaseActivityView: BaseForm
    {
        protected IEnumerable<int> _selectedSurveys;

        protected readonly ICachedLocalTimezoneManager TimezoneProvider =
            ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
 
        /// <summary>
        /// Use SessionPageStatePersister as PageStatePersister to store
        /// viewstate data in session.
        /// </summary>
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new LightSessionPageStatePersister(this);
            }
        }
        
        public virtual IEnumerable<int> SelectedSurveys
        {
            get
            {
                if (_selectedSurveys == null)
                {
                    _selectedSurveys = GetSurveysSelectedByUser() ??
                                       SurveyManager.GetOpenSurveys(User.Name, String.Empty).Select(x => x.Id).ToArray();
                }

                return _selectedSurveys;
            }            
        }

        protected abstract IEnumerable<int> GetSurveysSelectedByUser();
        
        protected override void OnLoad(EventArgs e)
        {
            RegisterClientLibrary("ActivityViews/client/activity.js");

            // AJAX timeout in Activity views is smaller than default to handle connectivity errors more gracefully.
            // Similar server-side executionTimeout is set for activity views in web.config.
            ScriptManager.AsyncPostBackTimeout = 30;

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            GZipEncodePage();
            base.OnPreRender(e);
        }

        /// <summary>
        /// Determins if web browser supports GZip encoding.
        /// </summary>
        protected bool IsGZipSupported()
        {
            string AcceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];
            if (!string.IsNullOrEmpty(AcceptEncoding) &&
               (AcceptEncoding.Contains("gzip") || AcceptEncoding.Contains("deflate")))
                return true;
            return false;
        }

        /// <summary>
        /// Encodes web page using gzip stream (if supported by browser).
        /// </summary>
        protected void GZipEncodePage()
        {
            if (IsGZipSupported())
            {
                HttpResponse httpResponse = HttpContext.Current.Response;

                string AcceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];
                if (AcceptEncoding.Contains("gzip"))
                {
                    httpResponse.Filter = new System.IO.Compression.GZipStream(httpResponse.Filter,
                                 System.IO.Compression.CompressionMode.Compress);
                    httpResponse.AppendHeader("Content-Encoding", "gzip");
                }
                else
                {
                    httpResponse.Filter = new System.IO.Compression.DeflateStream(httpResponse.Filter,
                                 System.IO.Compression.CompressionMode.Compress);
                    httpResponse.AppendHeader("Content-Encoding", "deflate");
                }
            }
        }

        /// <summary>
        /// Gets the thresholds list for current activity view list.
        /// This method should be overridden in derived class before usage.
        /// </summary>
        public abstract List<BvThresholdType> GetThresholdsList();

        /// <summary>
        /// Gets the alerts list for current activity view list.
        /// This method should be overridden in derived class before usage.
        /// </summary>
        public abstract List<SurveyAlertInfo> GetAlertsList();

        /// <summary>
        /// Returns JavaScript code which constructs object of MessageSender class
        /// used for sending messages.
        /// </summary>
        /// <returns>JavaScript code.</returns>
        protected string GetClientMessageSenderScript()
        {
            return String.Format(
                "var messageSender = new MessageSender('{0}','{1}')",
                Title,
                BaseRelativePath("Messaging/SendMessageView.aspx")
            );
        }

        protected void InitHelpLink(XpMenuItem btnToolBarHelp, string helpPageUrl)
        {
            btnToolBarHelp.OnClientClick = string.Format("ActivityViews.showHelp('{0}')", BaseRelativePath(helpPageUrl));
        }
    }
}