using System;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Script.Classes;
using Infragistics.Web.UI.GridControls;
using Strings = Confirmit.CATI.Supervisor.Resources.Strings;

namespace Confirmit.CATI.Supervisor.Script.Controls
{
    public partial class SchedulingParamsControl : ScheduleControlBase
    {
        protected override string ClientControllerName
        {
            get { return null; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            m_grid.GetPage +=
                delegate(out int totalCount)
                {
                    return ScheduleManager.GetParameters(ParametersCollection, out totalCount);
                };

            m_grid.InitializeRow += m_grid_InitializeRow;            

            StateChecker.AddSaveButton(btnSave);
        }

        protected void Delete(object sender, EventArgs e)
        {
            int id = m_grid.SelectedKeysInt.First();

            ErrorCollection errors;
            ParametersCollection.RemoveById(id, out errors);

            if (errors != null && errors.Count > 0)
            {
                NotifyUser(errors);
            }
            else
            {
                OnChange(sender, e);
            }
        }

        void m_grid_InitializeRow(object sender, RowEventArgs e)
        {
            var type = (int)e.Row.Items.FindItemByKey("Type").Value;
            e.Row.Items.FindItemByKey("TypeName").Value = StringHelper.GetStringForEnum((SchedulingParameterType)type);
        }

        protected void OnChange(object sender, EventArgs eventArgs)
        {
            Page.RegisterStartupScript("Common.fireGlobalEvent('ScriptViewScheduleParametersChanged');");
            ScheduleChangedHandler(this, eventArgs);
        }

        /// <summary>
        /// Contains all shift types 
        /// </summary>
        protected CustomParameterCollection ParametersCollection
        {
            get
            {
                return WorkingSchedule.CustomParameters;
            }
        }

        /// <summary>
        /// Adds confirmation while Launch button click.
        /// </summary>
        public override void AddConfirmationWhileLaunch()
        {
            m_grid.Commands.First(x => x.Key == "Launch").Confirmation = Strings.LaunchScriptConfirmation;
        }
    }
}