using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.ServerControls.Confirmit;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public class StateChecker : Control
    {
        public StateChecker()
        {
            SaveButtons = new List<XpMenuItem>();
        }

        private HtmlInputHidden _pageState = new HtmlInputHidden { ID = "PageState", Value = bool.FalseString };

        public bool IsPageChanged
        {
            get { return Convert.ToBoolean(_pageState.Value); }
        }

        private List<XpMenuItem> SaveButtons { get; set; }

        public void AddSaveButton(XpMenuItem value)
        {
            if (!SaveButtons.Contains(value))
            {
                SaveButtons.Add(value);
            }
        }

        public bool AutomaticallySubscribeOnChangeEvents { get; set; }
        public bool ShowBeforeUnloadWarning { get; set; }
        public bool Disabled { get; set; }

        public void MarkAsChanged()
        {
            _pageState.Value = bool.TrueString;
        }

        public void MarkAsUnchanged()
        {
            _pageState.Value = bool.FalseString;
        }

        protected override void OnInit(EventArgs e)
        {
            Controls.Add(_pageState);
            base.OnLoad(e);
            Page.PreRenderComplete += Page_PreRenderComplete;
        }

        private void Page_PreRenderComplete(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "StateCheckerInit", GetClientControllerScript(), true);

            if (Disabled)
                return;

            if (SaveButtons.Any())
            {
                foreach (var saveButton in SaveButtons)
                {
                    if (IsPageChanged)
                    {
                        saveButton.CssClass += "save-icon-blinking";
                    }
                }
            }

            ScriptManager.RegisterOnSubmitStatement(this, GetType(), "StateCheckerOnSubmit", "StateChecker.BeforeSubmit();");
        }

        private string GetClientControllerScript()
        {
            return string.Format("var StateChecker = new _stateChecker({0});", GetClientControllerSettings());
        }

        private string GetClientControllerSettings()
        {
            var settings = new
            {
                pageStateId = _pageState.ClientID,
                saveButtonIds = SaveButtons.Select(x => x.ClientID).ToArray(),
                automaticallySubscribeOnChangeEvents = AutomaticallySubscribeOnChangeEvents,
                showBeforeUnloadWarning = ShowBeforeUnloadWarning,
                disabled = Disabled,
            };

            return new JavaScriptSerializer().Serialize(settings);
        }
    }
}