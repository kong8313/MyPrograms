using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Supervisor.Classes.Auth;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.Configuration;

namespace Confirmit.CATI.Supervisor.Classes
{
    /// <summary>
    /// Base class for all Apollo web pages
    /// </summary>
    public class BaseForm : Page, IBaseForm
    {
        public BaseForm()
            : this(ServiceLocator.Resolve<IFileToBrowserSender>(),
            ServiceLocator.Resolve<IPgpEncryptionService>())
        {
        }

        public BaseForm(IFileToBrowserSender fileToBrowserSender, IPgpEncryptionService pgpEncryptionService)
        {
            ShowPostbackProcessingAnimation = true;
            _fileToBrowserSender = fileToBrowserSender;
            _pgpEncryptionService = pgpEncryptionService;

            WebControl.DisabledCssClass = "cati-control--disabled";

            var ai = Infragistics.Web.UI.Framework.AppSettings.SharedAjaxIndicator;
            if (ai != null)
            {
                ai.Text = @"<div class='comd-busy-dots comd-busy-dots--extra-large'><div class='comd-busy-dots__dot'></div><div class='comd-busy-dots__dot'></div><div class='comd-busy-dots__dot'></div></div>";
            }
        }

        protected const string YuiComboHandler = "YuiCombo.ashx?";
        private List<Message> m_CustomMessages;
        private bool m_DisableControlsOnPostback = true;
        private readonly List<string> _styleSheets = new List<string>();
        private readonly IFileToBrowserSender _fileToBrowserSender;
        private readonly IPgpEncryptionService _pgpEncryptionService;

        public string FileElementId = "file-to-client-sender";

        private FieldViewStateSerializer _fieldViewStateSerializer;

        protected FieldViewStateSerializer FieldViewStateSerializer
        {
            get
            {
                return _fieldViewStateSerializer ?? (_fieldViewStateSerializer = new FieldViewStateSerializer(this, ViewState));
            }
        }

        #region Properties.

        public virtual new string Title
        {
            get
            {
                return String.Empty;
            }
        }

        public ScriptManager ScriptManager
        {
            get { return ScriptManager.GetCurrent(this); }
        }

        public FileToClientSender FileToClientSender
        {
            get
            {
                return new FileToClientSender(this, _fileToBrowserSender, _pgpEncryptionService);
            }
        }

        public bool IsAsyncPostback
        {
            get { return ScriptManager != null && ScriptManager.IsInAsyncPostBack; }
        }

        public bool ShowPostbackProcessingAnimation { get; set; }

        /// <summary>
        /// Gets information about the user making the page request.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Security.Principal.IPrincipal"/> that represents the user making the page request.
        /// </returns>
        public new SupervisorPrincipal User
        {
            get
            {
                return SupervisorPrincipal.Current;
            }
        }

        /// <summary>
        /// Indicates, should client script disable controls before postback or not.
        /// Usually it's set to true, but sometimes on "heavy" pages with many controls it work too slowly,
        /// so it may be disabled.
        /// </summary>
        public bool DisableControlsOnPostback
        {
            get { return m_DisableControlsOnPostback; }
            set { m_DisableControlsOnPostback = value; }
        }

        public bool IEDocumentModeEdge { get; set; }

        /// <summary>
        /// List of custom application messages to be shown on client.
        /// </summary>
        public List<Message> CustomMessages
        {
            get
            {
                if (m_CustomMessages == null)
                    m_CustomMessages = new List<Message>();
                return m_CustomMessages;
            }
            set
            {
                m_CustomMessages = value;
            }
        }

        /// <summary>
        /// Gets the title of the page. If overridden in derived class and not null or empty - will be shown in the top frame when the page is loaded.
        /// </summary>
        public virtual string TopTitle
        {
            get
            {
                return null;
            }
        }

        #endregion        

        /// <summary>
        /// Recursively searches though specified control's controls collection
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="control_id"></param>
        /// <returns></returns>
        public static Control FindControlRecursive(Control parent, string control_id)
        {
            return DeepFindControl(parent, control_id);
        }

        /// <summary>
        /// Defines the page title of the Control Panel window.
        /// </summary>
        protected virtual string PageTitle
        {
            get
            {
                return Strings.DefPageTitle;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            CheckSecurity();
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ValidateForm();

            if (!IsPostBack)
                LegacySupervisorMetrics.OnPageView(GetType().Name);
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

        protected override void OnPreRenderComplete(EventArgs e)
        {
            base.OnPreRenderComplete(e);
            ShowContextErrors();
        }

        /// <summary>
        /// Shows and logs context errors.
        /// </summary>
        protected void ShowContextErrors()
        {
            if (Context.AllErrors != null)
            {
                foreach (Exception ex in Context.AllErrors)
                {
                    if (ex is ThreadAbortException)
                    {
                        continue;
                    }

                    ExceptionTraceHelper.TraceException(ex);

                    string errorMessage = ExceptionTraceHelper.GetUserErrorMessageFromException(ex);
                    CustomMessages.Add(new Message(MessageTypeEnum.Error, errorMessage, String.Empty));
                }
                Context.ClearError();
            }
            if (CustomMessages.Count > 0)
            {
                StringBuilder message = new StringBuilder();
                foreach (Message msg in CustomMessages)
                {
                    message.Append(msg.Description).Append("\n");
                }
                ShowClientMessage(message.ToString());
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            RegisterClientLibrary("client/common.js");
            RegisterClientLibrary("client/popper/popper.min.js");

            RegisterClientLibrary("client/clientfunctions.js");
            RegisterClientLibrary("client/OverlayLightBox.js");

            RegisterStartupScript(String.Format("stayAlive('{0}');", ConfigHelper.ConfirmitKeepSessionAspxUrl));

            RegisterStyleSheet("styles/styles.css");
            RegisterStyleSheet("styles/fonts.css");
            RegisterStyleSheet("styles/OverlayLightBox.css");

            if (!IsPostBack)
            {
                var title = string.IsNullOrEmpty(TopTitle) ? "&nbsp;" : Simplefunctions.Instance().noQuotStringForScript(TopTitle);
                RegisterStartupScript(string.Format("Common.setTitle('{0}');", title));
            }

            if (Config.DisablePopupMenu)
                RegisterStartupScript("Common._disablePopupMenu();");

            RegisterStartupScript("Common._startupScript();");

            if (ShowPostbackProcessingAnimation)
            {
                Page.ClientScript.RegisterOnSubmitStatement(GetType(), "OnSubmitChangeProcessingStateCall", "Common._setProcessingState(true);");
            }
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "OverlayLightBox", String.Format("var overlay = new OverlayLightBox('{0}');", Request.ApplicationPath), true);
        }

        protected void CleanChanged()
        {
            RegisterStartupScript("if(top.setChanged) {top.setChanged(false);}");
        }

        protected virtual void CheckSecurity()
        {
            if (Session.IsNewSession)
            {
                Session["ForceSession"] = DateTime.Now;
            }

            /*Sign ViewState with current session*/
            ViewStateUserKey = Session.SessionID;

            if (Page.EnableViewState)
            {
                if (string.IsNullOrEmpty(Request.Params["__VIEWSTATE"]) == false &&
                    string.IsNullOrEmpty(Request.Form["__VIEWSTATE"]))
                {
                    throw new UserMessageException(Strings.AuthorizationErrorHasOccured);
                }
            }

            if (IsPostBack == false && !ExceptionTraceHelper.IsErrorPage())
            {
                new SurveyPermissionVerifier(this, User.Name).Verify();
            }
        }

        protected virtual void ValidateForm()
        {
            if (IsPostBack == false) return;

            Validate();

            if (IsValid) return;

            var message = (from BaseValidator v in Validators where v.IsValid == false select v.ErrorMessage).First();
            throw new UserMessageException(message);
        }

        private static string[] YuiModules = new[]
            {
                "yui-base",
                "oop",
                "event-custom-base",
                "features",
                "dom-core",
                "dom-base",
                "selector-native",
                "selector",
                "node-core",
                "node-base",
                "event-base",
                "event-delegate",
                "event-custom-complex",
                "event-synthetic",
                "event-mousewheel",
                "event-mouseenter",
                "event-key",
                "event-focus",
                "event-resize",
                "event-hover",
                "event-outside",
                "event-valuechange",
                "node-event-delegate",
                "pluginhost-base",
                "pluginhost-config",
                "node-pluginhost",
                "dom-style",
                "dom-screen",
                "node-screen",
                "node-style",
                "json-parse",
                "json-stringify",
                "intl",
                "selector-css2",
                "event-base-ie",
                "dom-style-ie",
                "querystring-parse",
                "array-extras",
                "gallery-beforeunload"
            };

        public static string GetYuiConfigScript()
        {
            var script =
                $@"document.documentElement.addEventListener('click', function(e) {{
                    if(top.fireClickEvent) top.fireClickEvent();
                    else if(top.opener && top.opener.fireClickEvent) top.opener.fireClickEvent();
                    if(window.popper && e.target!=window.popper.popper && e.target!=window.popper.reference)
                    {{   window.popper.destroy(); window.popper = null; }}
                }});
                document.documentElement.addEventListener('custom_pageLoad', function(e) {{
                    if(top.fireCustomPageLoadEvent) top.fireCustomPageLoadEvent();
                    else if(top.opener && top.opener.fireCustomPageLoadEvent) top.opener.fireCustomPageLoadEvent();
                }});
                
                YUI_config = {{
                    base: '{BaseRelativePath("client/YUI/")}',
                    combine: true,
                    comboBase: '{BaseRelativePath(YuiComboHandler)}'
                }};";
            return script;
        }

        public static string GetYuiPreloadScriptUrl()
        {
            var mode = Config.DebugMode ? "" : "-min";
            var url = new StringBuilder();
            url.Append(BaseRelativePath(YuiComboHandler));

            for (int i = 0; i < YuiModules.Length; i++)
            {
                var module = YuiModules[i];
                var mod = string.Format("3.4.1/build/{0}/{0}{1}.js", module, mode);
                url.Append(mod);

                if (i < YuiModules.Length - 1)
                    url.Append("&");
            }

            return url.ToString();
        }

        //---------------------------------------------------------------------------
        protected override void OnError(EventArgs e)
        {
            if (Context.AllErrors != null)
            {
                // we got server error and we have error in Context
                foreach (Exception ex in Context.AllErrors)
                {
                    ExceptionTraceHelper.TraceException(ex);
                }
            }
            ExceptionTraceHelper.ShowServerError();
            base.OnError(e);
        }

        //---------------------------------------------------------------------------
        public static string GetResString(string sId)
        {
            return (Confirmit.CATI.Supervisor.Core.Common.ResourceWrapper.Instance.GetString(sId));
        }

        //---------------------------------------------------------------------------
        public static string GetResString(string sId, params object[] prms)
        {
            return (String.Format(Confirmit.CATI.Supervisor.Core.Common.ResourceWrapper.Instance.GetString(sId), prms));
        }

        /// <summary>
        /// Writes html-code which resizes current window.
        /// </summary>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        public void ResizeWindow(int width, int height)
        {
            RegisterStartupScript(String.Format("top.overlay.resize({0},{1});", width, height));
        }

        /// <summary>
        /// Writes html-code, that forces the left frame to refresh after current postback.
        /// </summary>
        public void RefreshInfoFrame()
        {
            RegisterStartupScript("try{Common.refreshInfoFrame();}catch(e){}");
        }

        public void CloseInfoFrame()
        {
            RegisterStartupScript("Y.on('domready', function(){top.closeAndClearInfoFrame()});");
        }

        /// <summary>
        /// Writes html-code, that forces the list top frame to refresh after current postback.
        /// </summary>
        public void RefreshListFrame()
        {
            RegisterStartupScript("try{Common.refreshListFrame();}catch(e){Y.log(e.message);}");
        }

        /// <summary>
        /// Writes html-code, that forces the left frame to refresh after current postback.
        /// </summary>
        public void RefreshLeftFrame()
        {
            RegisterStartupScript("try{top.refreshMainMenu();}catch(e){}");
        }
        /// <summary>
        /// Registers script, that forces current window to close with given return value.
        /// </summary>
        public void CloseWindow(string return_value)
        {
            return_value = return_value ?? String.Empty;
            CloseWindowEx("window.returnValue = '" + return_value + "';");
        }

        /// <summary>
        /// Registers script, that forces current window to close. Additional actions could be specified to be done before close.
        /// </summary>
        /// <param name="doBeforeClose">JavaScript to be executed before window close. No script tags needed.</param>
        public void CloseWindowEx(string doBeforeClose)
        {
            doBeforeClose = doBeforeClose ?? String.Empty;
            RegisterStartupScript(doBeforeClose + "; window.close()");
        }

        public void RefreshUserSettings()
        {
            RegisterStartupScript("top.Y.fire('updateUserSettings');");
        }

        public string GetCloseOverlayScript(bool executeClosingFunction = false, string data = null, bool closeLocalOverlay = false)
        {
            if (!closeLocalOverlay)
                return String.Format(
                    "Y.one(window).on(\"load\", function(){{ top.overlay.closeLast({0}{1});  }});",
                    executeClosingFunction.ToString().ToLower(),
                    data != null ? string.Format(",\"{0}\"", data) : string.Empty);
            else
            {
                return String.Format(
                    "Y.one(window).on(\"load\", function(){{ if(overlay.isOpen) overlay.closeLast({0}{1}); if(parent.overlay.isOpen) parent.overlay.closeLast({0}{1}); if(top.overlay.isOpen) top.overlay.closeLast({0}{1}); }});",
                    executeClosingFunction.ToString().ToLower(),
                    data != null ? string.Format(",\"{0}\"", data) : string.Empty);
            }
        }

        public void CloseOverlay(bool executeClosingFunction = false, string data = null, bool closeLocalOverlay = false)
        {
            RegisterScriptBlock(GetCloseOverlayScript(executeClosingFunction, data, closeLocalOverlay));
        }

        public void CloseOverlayAfterFileIsReadyToDownload(bool executeClosingFunction = false)
        {
            RegisterStartupScript(string.Format("Y.on('contentready', function(){{setTimeout(function(){{top.overlay.closeLast({0});}}, 1000)}}, '#{1}');",
                executeClosingFunction.ToString().ToLower(),
                FileElementId));
        }

        public void SetOverlayTitle(string title)
        {
            var script = String.Format("Y.one(window).on(\"load\", function(){{overlay.setOverlayTitle('{0}');}});", HttpUtility.HtmlEncode(title));

            RegisterScriptBlock(script);
        }

        /// <summary>
        /// Produces a path, relative to base application dir
        /// </summary>
        public static string BaseRelativePath(string path)
        {
            if (path.Contains(":"))
            {
                return path;
            }

            return HttpContext.Current.Request.ApplicationPath + "/" + path;
        }


        private static Control DeepFindControl(Control parent, string id)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl.ID == id)
                    return ctrl;
                Control fnd = DeepFindControl(ctrl, id);
                if (fnd != null)
                    return fnd;
            }
            return null;
        }

        /// <summary>
        /// Recursively searches for controls with the specified ID though specified control's controls collection.
        /// </summary>
        /// <param name="parent">The parent control to search in.</param>
        /// <param name="controlID">The ID of the controls to search for.</param>
        /// <returns>
        /// The list of controls with the ID specified.
        /// </returns>
        public static List<Control> FindControlsRecursive(Control parent, string controlID)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (String.IsNullOrEmpty(controlID))
                throw new ArgumentNullException("controlID");

            List<Control> result = new List<Control>();
            if (parent.ID == controlID)
            {
                result.Add(parent);
            }

            foreach (Control ctrl in parent.Controls)
            {
                result.AddRange(FindControlsRecursive(ctrl, controlID));
            }

            return result;
        }

        public void RegisterStartupScript(string script, string key = null, Type type = null)
        {
            if (string.IsNullOrWhiteSpace(script))
            {
                throw new ArgumentNullException("script");
            }

            if (key == null)
            {
                key = script;
            }

            if (type == null)
            {
                type = GetType();
            }

            if (!script.EndsWith(";"))
            {
                script += ";";
            }

            //ClientScript.RegisterClientScriptBlock(type, key, script, true);
            ScriptManager.RegisterStartupScript(this, type, key, script, true);
        }

        public void RegisterScriptBlock(string script, string key = null, Type type = null)
        {
            if (string.IsNullOrWhiteSpace(script))
            {
                throw new ArgumentNullException("script");
            }

            if (key == null)
            {
                key = script;
            }

            if (type == null)
            {
                type = GetType();
            }

            if (!script.EndsWith(";"))
            {
                script += ";";
            }
            //ClientScript.RegisterClientScriptBlock(type, key, script, true);
            ScriptManager.RegisterClientScriptBlock(this, type, key, script, true);
        }

        public void RegisterClientLibrary(string path)
        {
            PageHelper.RegisterClientLibrary(path);
        }

        public void RegisterStyleSheet(string path)
        {
            if (_styleSheets.Contains(path) == false)
            {
                _styleSheets.Add(path);
            }
        }

        public string GetStyleSheetLinks()
        {
            var links = _styleSheets.Select(
                x => String.Format(
                    "    <link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />",
                    PageHelper.GetCssPathReference(x)));

            return String.Join(Environment.NewLine, links.ToArray());
        }

        /// <summary>
        /// Shows message on client using simple javascript alert() function.
        /// </summary>
        /// <param name="text">Message text.</param>
        /// <param name="showSynchronously">If set to true the futher script proccessing will be stoped 
        /// until the 'alert' box is closed. Used for example for displaying message before an overlay dialog is closed.
        /// </param>
        public void ShowClientMessage(string text, bool showSynchronously = false)
        {
            text = Simplefunctions.Instance().noQuotStringForScript(text);

            string script = showSynchronously ?
                            String.Format(@"alert('{0}');", text) :
                            String.Format(@"setTimeout(function(){{alert('{0}');}},0)", text);

            RegisterStartupScript(script);
        }

        /// <summary>
        /// Adds the UserMessageException exception with the specific message to the
        /// exception collection for the current HTTP request as warning.
        /// </summary>
        /// <param name="message">The message.</param>
        public void AddUserMessage(string message)
        {
            Context.AddError(new UserMessageException(GetResString(message)));
        }

        /// <summary>
        /// Adds the UserMessageException exception to the exception collection for the current HTTP request.
        /// Exception message will be used as user message text.
        /// </summary>
        /// <param name="exception">The exception to add.</param>
        public void AddUserMessage(Exception exception)
        {
            Context.AddError(new UserMessageException(exception.Message, exception));
        }

        /// <summary>
        /// Adds the UserMessageException exception to the exception collection for the current HTTP request.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="exception">The exception to add as inner exception.</param>
        public void AddUserMessage(string message, Exception exception)
        {
            Context.AddError(new UserMessageException(GetResString(message), exception));
        }

        public void Redirect(string url)
        {
            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
