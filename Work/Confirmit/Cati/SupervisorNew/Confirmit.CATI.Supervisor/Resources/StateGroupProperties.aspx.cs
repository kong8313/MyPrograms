using System;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.ITSs;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class StateGroupProperties : BaseForm
    {
        /// <summary>
        /// ID of state group to be copied.
        /// </summary>
        private int? CopyID
        {
            get { return (int?)ViewState["CopyID"]; }
            set { ViewState["CopyID"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["CopyID"] != null)
            {
                CopyID = Int32.Parse(Request.Params["CopyID"]);
            }

            dialog.OKButton.ResName = CopyID.HasValue ? "Duplicate" : "Create";
        }

        /// <summary>
        /// Creates new state group base on Default state group.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                using (var transaction = new DatabaseTransactionScope("Supervisor.CreateStateGroup", DeadlockPriority.Supervisor))
                {
                    string stateName = tbStateGroupName.Text.Trim();
                    if (!CopyID.HasValue)
                    {
                        StateGroupsManager.AddStateGroup(stateName);
                    }
                    else
                    {
                        StateGroupsManager.CopyStateGroup(stateName, CopyID.Value);
                    }

                    transaction.Commit();
                }

                CloseOverlay(true);
            }
            catch (ArgumentException ex)
            {
               AddUserMessage(ex);
            }
            catch(Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}
