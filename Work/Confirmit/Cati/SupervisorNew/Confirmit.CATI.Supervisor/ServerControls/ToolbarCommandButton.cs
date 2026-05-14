using System;
using System.Web.UI;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.ServerControls.Commands;
using Confirmit.CATI.Supervisor.ServerControls.Confirmit;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    /// <summary>
    /// Item of XpMenu that is linked to the Command class.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public class ToolbarCommandButton : XpMenuItem
    {
        /// <summary>
        /// The Key of the linked Command class.
        /// </summary>
        public string Key { get; set; }

        private Command _linkedCommand;
        private string _onClientClickCached;

        public Command LinkedCommand
        {
            get { return _linkedCommand; }
            set
            {
                if (value.Key != Key)
                {
                    throw new InvalidOperationException("Toolbar button and linked command should have the same Key.");
                }

                _linkedCommand = value;
            }
        }

        public Control BaseControl { get; set; }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // HACK: Ensure that linked command registered its scripts at this point
            // If OnClientClick will be called at Render phase - script won't appear on the page.
            if (LinkedCommand != null)
            {
                _onClientClickCached = OnClientClick;
            }
        }

        public override string OnClientClick
        {
            get
            {
                if (LinkedCommand == null)
                {
                    throw new InvalidOperationException("Linked command is not initialized.");
                }

                return _onClientClickCached ?? LinkedCommand.GetClientEventJavaScript(Page, BaseControl);
            }
        }

        public override string ToolTip
        {
            get
            {
                if (LinkedCommand == null)
                {
                    throw new InvalidOperationException("Linked command is not initialized.");
                }

                return ResourceWrapper.Instance.GetString(LinkedCommand.Caption);
            }
        }

        public override string ImageName
        {
            get
            {
                if (LinkedCommand == null)
                {
                    throw new InvalidOperationException("Linked command is not initialized.");
                }

                return LinkedCommand.Image;
            }
        }
    }
}