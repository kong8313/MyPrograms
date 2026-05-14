using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Classes;
using System.Data.SqlClient;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Script.Classes;

namespace Confirmit.CATI.Supervisor.Script
{
    /// <summary>
    /// Summary description for ScriptProperties.
    /// </summary>
    public partial class ScriptProperties : BaseForm
    {
        private BvScheduleEntity m_schedule;

        public override string Title
        {
            get { return Strings.ScriptProperties; }
        }

        /// <summary>
        /// Gets the script.
        /// </summary>
        /// <value>The script.</value>
        protected BvScheduleEntity Schedule
        {
            get
            {
                if (m_schedule == null)
                {
                    if (ID.HasValue)
                        m_schedule = ScheduleRepository.GetById(ID.Value);
                }
                return m_schedule;
            }
        }

        protected new int? ID
        {
            get
            {
                return ViewState["ID"] != null ? Convert.ToInt32(ViewState["ID"]) : (int?)null;
            }
            set
            {
                ViewState["ID"] = value;
            }
        }
        
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request["ID"]))
                {
                    ID = Int32.Parse(Request["ID"]);
                }
            }

            if (Schedule == null)
            {
                dialogControl.Title = Strings.NewScript;
            }
            else
            {
                dialogControl.Title = Strings.Properties + " of \'" + Schedule.Name + "\'";
                pGeneral.ScriptId = Schedule.ScheduleID;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Schedule != null)
            {
                pGeneral.ScriptId = Schedule.ScheduleID;
                pGeneral.ScriptName = Schedule.Name;
                pGeneral.SelectedStateGroupId = Schedule.DesignStateGroupID;
            }
        }

        /// <summary>
        /// Saves the script on button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveButtonClick(object sender, EventArgs e)
        {
            try
            {
                pGeneral.Validate();
                string scriptName = pGeneral.ScriptName.Trim();
                int? designStateGroupId = pGeneral.SelectedStateGroupId;
                var scriptForCheck = ScheduleRepository.GetByName(scriptName);

                if (Schedule == null)
                {
                    if (scriptForCheck != null)
                    {
                        AddUserMessage("Err_ScrNameExists");
                    }
                    else
                    {
                        using (var transactionScope = new DatabaseTransactionScope("CreateScript", DeadlockPriority.Supervisor))
                        {
                            var evt = new CreateScriptEvent(0, scriptName);

                            m_schedule = ScheduleManager.AddSchedule(scriptName, designStateGroupId);

                            evt.ObjectId = m_schedule.ScheduleID;

                            evt.Finish();
                            transactionScope.Commit();
                        }

                        CloseOverlay(true);
                    }
                }
                else
                {
                    if (scriptForCheck != null && scriptForCheck.ScheduleID != m_schedule.ScheduleID)
                    {
                        AddUserMessage("Err_ScrNameExists");
                    }
                    else
                    {
                        m_schedule.Name = scriptName;
                        m_schedule.DesignStateGroupID = designStateGroupId;

                        using (var transactionScope = new DatabaseTransactionScope("UpdateScript", DeadlockPriority.Supervisor))
                        {
                            var evt = new UpdateScriptEvent(m_schedule.ScheduleID, m_schedule.Name);

                            ScheduleRepository.Update(m_schedule);

                            evt.Finish();
                            transactionScope.Commit();
                        }

                        CloseOverlay(true);
                    }
                }
            }
            catch (SqlException ex)
            {
                // someone may insert srcipt with the same name at the moment between 
                // checking name and inserting/updating script. So we should check it.
                if (BaseMethods.IsUniqueConstraint(ex))
                {
                    AddUserMessage("Err_ScrNameExists", ex);
                }
                else
                {
                    Context.AddError(ex);
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}
