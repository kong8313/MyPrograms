using System;
using System.Threading;
using Confirmit.CATI.Core.Misc.CP;
using System.Web.UI;

namespace Confirmit.CATI.Supervisor.Classes
{
    /// <summary>
    /// Base class for all Apollo control panel web user controls
    /// </summary>
    public class BaseWUC: UserControl
    {
        private FieldViewStateSerializer _fieldViewStateSerializer;

        protected FieldViewStateSerializer FieldViewStateSerializer
        {
            get
            {
                return _fieldViewStateSerializer ?? (_fieldViewStateSerializer = new FieldViewStateSerializer(this, ViewState));
            }
        }

        /// <summary>
        /// Gets current user
        /// It is initialized via url data
        /// </summary>
        public SupervisorPrincipal User
        {
            get
            {
                return (SupervisorPrincipal)Context.User;
            }
        }

        /// <summary>
        /// Gets a reference to the BaseForm instance that contains the server control.
        /// </summary>
        public new BaseForm Page
        {
            get
            {
                return (BaseForm)base.Page;
            }
        }

        public string GetResString(string sId)
        {
            return (Core.Common.ResourceWrapper.Instance.GetString(sId));
        }

        protected string GetResString(string sId, params object[] prms)
        {
            return (String.Format(Core.Common.ResourceWrapper.Instance.GetString(sId), prms));
        }

        protected override void OnError(EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex is ThreadAbortException)
            {
                Server.ClearError();
                return;
            }
            Context.AddError(ex);
            Server.ClearError();
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);

            FieldViewStateSerializer.Load();
        }

        protected override object SaveViewState()
        {
            FieldViewStateSerializer.Save();

            return base.SaveViewState();
        }

        /// <summary>
        /// Produces a path, relative to base application dir
        /// </summary>
        public string BaseRelativePath(string path)
        {
            return Request.ApplicationPath + "/" + path;
        }

        /// <summary>
        /// Shows message on client using simle javascript alert() function.
        /// </summary>
        /// <param name="text">Message text.</param>
        public void ShowClientMessage(string text)
        {
            Page.ShowClientMessage(text);
        }
    }
}