using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Script.Classes;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.Script.Controls
{
    public partial class ShiftTypesNewControl : ScheduleControlBase
    {
        protected override string ClientControllerName
        {
            get { return "shiftTypesController"; }
        }

        /// <summary>
        /// Contains all shift types 
        /// </summary>
        protected ShiftTypeCollection ShiftTypeCollection
        {
            get
            {
                return WorkingSchedule.ShiftTypes;
            }
        }

        /// <summary>
        /// Return true if ShiftTypeCollection contains 'Exclusion' type
        /// </summary>
        protected bool HasExclusion
        {
            get
            {
                return ShiftTypeCollection.Any(shiftType => shiftType.IsExclusionType);
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HasExclusion)
            {
                m_grid.DisableCommand("NewExclusion");
            }

            m_grid.GetPage += (out int totalCount) => ScheduleManager.GetShiftTypes(ShiftTypeCollection, out totalCount);
            m_grid.InitializeRow += m_grid_InitializeRow;            
            
            StateChecker.AddSaveButton(btnSave);                  
        }
        
        protected void Delete(object sender, EventArgs e)
        {
            int id = m_grid.SelectedKeysInt.First();

            ErrorCollection errors;
            ShiftTypeCollection.RemoveById(id, out errors);

            if (errors != null && errors.Count > 0)
            {
                NotifyUser(errors);
            }
            else
            {
                OnChange(sender, e);
            }
        }

        protected void OnChange(object sender, EventArgs eventArgs)
        {
            Page.RegisterStartupScript("Common.fireGlobalEvent('ScriptViewScheduleShiftTypeChanged');");
            ScheduleChangedHandler(this, eventArgs);
        }

        void m_grid_InitializeRow(object sender, RowEventArgs e)
        {
            var row = e.Row;

            var colorName = row.Items.FindItemByKey("ColorName").Value.ToString();

            if (!string.IsNullOrEmpty(colorName))
            {
                if (Enum.IsDefined(typeof(KnownColor), colorName))
                {
                    ((Panel)e.Row.Items.FindItemByKey("Color").FindControl("pnlColor")).BackColor = ColorTranslator.FromHtml(colorName);
                }
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