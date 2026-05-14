using System;
using System.Web.UI;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Controls
{
    public partial class ToggleSelector : BaseWUC
    {
        public bool Enabled
        {
            get => Convert.ToBoolean(toggleValue.Value);
            set => toggleValue.Value = value.ToString();
        }
      
        public string Text
        {
            set => lblToggleText.Text = value;
        }


        public string OnToggle { get; set; }

        
        public string HelpTextId
        {
            set => HelpTextEdit.HelpTextId = value;
            get => HelpTextEdit.HelpTextId;
        }

        public string TitleTextId
        {
            set => HelpTextEdit.TitleTextId = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            divToggle.Attributes.Add("onclick", $"toggleClick('{ClientID}_divToggle', '{OnToggle}');");

            if (!string.IsNullOrEmpty(HelpTextId))
            {
                divMain.Attributes.Add("class", "settings-table__with-help");
            }
            else
            {
                divHelp.Visible = false;
            }
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            
            string script = @"
                function toggleClick(toggleId, onToggleName) {
                    var toggle = Y.one('#' + toggleId + ' .comd-button-toggle__checkbox')._node;
                    setToggle(!toggle.checked, toggleId, onToggleName);
                }

                function setToggle(state, toggleId, onToggleName) {
                    var toggle = Y.one('#' + toggleId + ' .comd-button-toggle__checkbox')._node;
                    toggle.checked = state;
                    Y.one('#' + toggleId + ' input').set('value', state.toString());

                    eval(onToggleName + '(' + state + ')');
                }";
                  
            Page.RegisterScriptBlock(script, "Toggle", GetType());

            ScriptManager.RegisterStartupScript(
                this, GetType(), "SetToggle" + ClientID, $"setToggle({Enabled.ToString().ToLower()}, '{ClientID}_divToggle', '{OnToggle}');", true);
        }
    }
}