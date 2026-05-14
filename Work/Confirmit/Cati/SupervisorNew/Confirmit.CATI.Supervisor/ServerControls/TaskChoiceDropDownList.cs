using System;
using System.Web.UI.WebControls;

using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    /// <summary>
    /// Dropdown with task choices
    /// </summary>
    public class TaskChoiceDropDownList : DropDownList
    {       
        #region Constructors

        public TaskChoiceDropDownList()
        {
            Items.Add(new ListItem(Strings.TaskChoiceAutomatic, ((int)AgentTaskChoiceMode.Automatic).ToString(), true));
            Items.Add(new ListItem(Strings.TaskChoiceManualSelection, ((int)AgentTaskChoiceMode.Manual).ToString()));
            Items.Add(new ListItem(Strings.TaskChoiceSurveySelection, ((int)AgentTaskChoiceMode.CampaignAssignment).ToString()));
            Items.Add(new ListItem(Strings.Choice, ((int)AgentTaskChoiceMode.Choice).ToString()));
        }

        #endregion        

        #region Properties

        /// <summary>
        /// Gets selected task choice
        /// </summary>
        public AgentTaskChoiceMode SelectedTaskChoice
        {
            get
            {
                return (AgentTaskChoiceMode)Int32.Parse(SelectedValue);
            }
            set
            {
                SelectedIndex = (int)value;
            }
        }

        #endregion
    }
}
