using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.ServerControls.Commands;


namespace Confirmit.CATI.Supervisor.Controls
{
    [ParseChildren(true)]
    [PersistChildren(false)]
    //[PersistChildren(true)]
    public partial class Dialog : BaseWUC
    {
        private ITemplate content;
        private bool hideButtons;
        private bool hideHeader;

        /// <summary>
        /// Content template.
        /// </summary>
        [TemplateInstance(TemplateInstance.Single)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
            }
        }

        /// <summary>
        /// Dialog title.
        /// </summary>
        public string Title
        {
            get { return lbTitle.Text; }
            set { lbTitle.Text = value; }
        }

        /// <summary>
        /// Ok button.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ServerControls.Button OKButton
        {
            get { return this.btnOK; }
        }

        /// <summary>
        /// Save button.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ServerControls.Button SaveButton
        {
            get { return this.btnSave; }
        }

        /// <summary>
        /// Cancel button.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public HtmlAnchor CancelButton
        {
            get { return this.cancel; }
        }

        /// <summary>
        /// Hides all buttons. 
        /// For modal mode buttons cannot be hidden.
        /// </summary>
        public bool HideButtons
        {
            get
            {
                return hideButtons;
            }
            set
            {
                hideButtons = value;
            }
        }

        /// <summary>
        /// Hides header toolbar.
        /// </summary>
        public bool HideHeader
        {
            get
            {
                return hideHeader;
            }
            set
            {
                hideHeader = value;
            }
        }

        /// <summary>
        /// Dialog window mode.
        /// </summary>
        public DialogWindowMode Mode
        {
            get
            {
                if (ViewState["DialogWindowMode"] != null)
                    return (DialogWindowMode)ViewState["DialogWindowMode"];
                return DialogWindowMode.Frame;
            }
            set
            {
                ViewState["DialogWindowMode"] = value;
            }
        }

        public bool ShowSaveButton { get; set; }

        public bool ShowBottomBorder { get; set; } = true;

        protected void Page_Init(object sender, EventArgs e)
        {
            //Instantiate content template into content placeholder.
            Content.InstantiateIn(this.phContent);

            if (PutActionButtonsInsideGridIfPossible && !HideButtons)
            {
                var phForControls = GetControlByType<PlaceHolder>(phContent, holder => holder.ID == "phDialogButtons");
                if (phForControls != null && phForControls.Visible)
                {
                    if (cancel.Visible)
                    {
                        phForControls.Controls.Add(cancel);
                        Controls.Remove(cancel);
                    }

                    if (btnSave.Visible)
                    {
                        phForControls.Controls.Add(btnSave);
                        Controls.Remove(btnSave);
                    }

                    if (btnOK.Visible)
                    {
                        phForControls.Controls.Add(btnOK);
                        Controls.Remove(btnOK);
                    }

                    if (!divButtonsHolder.Controls.Cast<Control>().Any(x =>
                        (x.GetType() == typeof(HtmlAnchor) || x.GetType() == typeof(ServerControls.Button)) &&
                        x.Visible))
                    {
                        divButtonsHolder.Visible = false;
                    }

                    ShowBottomBorder = false;
                }
            }
        }

        public bool PutActionButtonsInsideGridIfPossible { get; set; } = true;

        public T GetControlByType<T>(Control root, Func<T, bool> predicate = null) where T : Control
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            var stack = new Stack<Control>(new Control[] { root });

            while (stack.Count > 0)
            {
                var control = stack.Pop();
                T match = control as T;

                if (match != null && (predicate == null || predicate(match)))
                {
                    return match;
                }

                foreach (Control childControl in control.Controls)
                {
                    stack.Push(childControl);
                }
            }

            return default(T);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ViewState["DialogWindowMode"] == null && string.IsNullOrEmpty(Request["Mode"]) == false)
            {
                Mode = (DialogWindowMode)(Enum.Parse(typeof(DialogWindowMode), Request["Mode"], true));
            }

            InitLayout();

            dialogPanel.CssClass = Mode == DialogWindowMode.Modal ? "modalDialog" : "frameDialog";
        }

        /// <summary>
        /// Refreshes only right list frame.
        /// Right bottom frame 
        /// </summary>
        public void RefreshListFrameIfDialogNonModal()
        {
            if (Mode != DialogWindowMode.Modal)
            {
                Page.RefreshListFrame();
            }
        }

        public void RefreshListFrame()
        {
            Page.RefreshListFrame();
        }

        public void SetCancelAction(string scriptAction)
        {
            cancel.Attributes["onclick"] = scriptAction;
        }

        /// <summary>
        /// Control's layout initialization.
        /// </summary>
        protected void InitLayout()
        {
            //Customize dialog appearance regarding to dialog window mode.
            switch (Mode)
            {
                case DialogWindowMode.Frame:
                    btnOK.Visible = false;
                    break;
                case DialogWindowMode.Modal:
                    btnSave.Visible = false;
                    break;
                case DialogWindowMode.Floating:
                    btnSave.Visible = false;
                    break;
            }
            //Hide header toolbar if it's needed.
            if (HideHeader)
            {
                trHeader.Visible = false;
            }

            if (HideButtons)
            {
                divButtonsHolder.Visible = false;
                ShowBottomBorder = false;
            }

            if (ShowSaveButton)
            {
                btnSave.Visible = true;
            }
        }
    }
}
