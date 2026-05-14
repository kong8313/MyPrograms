using System;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Supervisor.Controls
{
    public partial class SelectTaskChoicePermissions : BaseWUC
    {       
        #region Properties

        /// <summary>
        /// Gets selected task choice permissions
        /// </summary>
        public TaskChoicePermissions? Permissions
        {
            get
            {
                    int permissions = 0;

                    if (cbAutomaticSelection.Checked) permissions = (int)TaskChoicePermissions.Automatic;
                    if (cbManualSelection.Checked) permissions = permissions | (int)TaskChoicePermissions.Manual;
                    if (cbSurveySelection.Checked) permissions = permissions | (int)TaskChoicePermissions.SurveyAssignment;

                    if (permissions > 0)
                    {
                        return (TaskChoicePermissions)permissions;
                    }
                

                return null;
            }
            set
            {
                InitPermissionControls(value);
            }
        }

        /// <summary>
        /// Gets/sets client handler for checked/unchecked persmission checkboxes events
        /// </summary>
        public string PermissionChangedClientHandler
        {
            get;
            set;            
        }

        #endregion        

        #region Events

        /// <summary>
        /// Occurs when SurveySelection permission is checked or unchecked
        /// </summary>
        public event EventHandler SurveySelectionPermissionChanged
        {
            add
            {
                cbSurveySelection.AutoPostBack = true;
                cbSurveySelection.CheckedChanged += value;            
            }
            remove
            {
                cbSurveySelection.AutoPostBack = false;
                cbSurveySelection.CheckedChanged -= value; 
            }
        }               

        #endregion
                      

        #region Methods

        /// <summary>
        /// Unchecked all checkboxes
        /// </summary>
        public void ClearSelection()
        {
            cbAutomaticSelection.Checked = false;
            cbManualSelection.Checked = false;
            cbSurveySelection.Checked = false;
        }

        /// <summary>
        /// Set checked/unchecked permission checkboxes accoding "permissions" parameter
        /// </summary>
        /// <param name="permissions"></param>
        private void InitPermissionControls(TaskChoicePermissions? permissions)
        {
            ClearSelection();

            if (permissions.HasValue)
            {
                if ((permissions & TaskChoicePermissions.Automatic) == TaskChoicePermissions.Automatic)
                {
                    cbAutomaticSelection.Checked = true;
                }
                if ((permissions & TaskChoicePermissions.Manual) == TaskChoicePermissions.Manual)
                {
                    cbManualSelection.Checked = true;
                }
                if ((permissions & TaskChoicePermissions.SurveyAssignment) == TaskChoicePermissions.SurveyAssignment)
                {
                    cbSurveySelection.Checked = true;
                }
            }
        }        

        #endregion       

        #region Page cycle

        protected void Page_PreRender(object sender, EventArgs e)
        {            
            if (String.IsNullOrEmpty(PermissionChangedClientHandler) == false)
            {
                string script = "window['" + PermissionChangedClientHandler + "']({0}, this.checked)";

                cbManualSelection.InputAttributes["onclick"] = string.Format(script, (int)TaskChoicePermissions.Manual);
                cbAutomaticSelection.InputAttributes["onclick"] = string.Format(script, (int)TaskChoicePermissions.Automatic);
                cbSurveySelection.InputAttributes["onclick"] = string.Format(script, (int)TaskChoicePermissions.SurveyAssignment);
            }
        }                 

        #endregion
    }
}