using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Classes;
using System.Web.UI;
using System;

namespace Confirmit.CATI.Supervisor.Controls
{
    public partial class ItsSelect : BaseWUC
    {
        /// <summary>
        /// Get checkbox list (cblITS object)
        /// </summary>
        public CheckBoxList CblIts
        {
            get
            {
                return cblITS;
            }
        }

        /// <summary>
        /// Get updatePanelIts object
        /// </summary>
        public UpdatePanel UpdatePanelIts
        {
            get
            {
                return updatePanelIts;
            }
        }

        /// <summary>
        /// Height of popup (can be just a number or a number with px)
        /// Default is 355
        /// </summary>
        public string Height
        {
            set
            {
                pnlITS.Height = Unit.Parse(value.TrimEnd('p', 'x'));
            }
        }

        /// <summary>
        /// Set id of control which opens popup its list
        /// Default is btnITS
        /// </summary>
        public string PopupExtenderMasterID
        {
            set
            {
                peITS.MasterID = value;
            }
        }

        /// <summary>
        /// Set IsSubmit object for OK button
        /// Default is false
        /// </summary>
        public bool IsSubmit
        {
            set
            {
                SetSubmit(value);
            }
        }

        /// <summary>
        /// Add or remove OnClick event for OK button
        /// </summary>
        public event EventHandler Click
        {
            add
            {
                SetSubmit(true);
                btnSelectITS.Click += value;
            }

            remove
            {
                SetSubmit(false);
                btnSelectITS.Click -= value;
            }
        }

        private void SetSubmit(bool value)
        {
            btnSelectITS.IsSubmit = value;
            updatePanelIts.Triggers.Clear();
            if (value)
            {
                updatePanelIts.Triggers.Add(new PostBackTrigger() { ControlID = "btnSelectITS" });
            }
        }
    }
}