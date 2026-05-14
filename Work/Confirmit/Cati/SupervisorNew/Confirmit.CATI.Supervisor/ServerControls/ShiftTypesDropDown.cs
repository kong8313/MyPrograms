using System;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    /// <summary>
    /// Dropdown with shift types (without exclusions and empty shift types).
    /// </summary>
    public class ShiftTypesDropDown: DropDownList
    {
        private int m_surveyID;
        
        /// <summary>
        /// Fusion survey SID to get shift types for.
        /// </summary>
        public int SurveyID
        {
            get 
            {
                int result;
                if (m_surveyID != 0)
                    result = m_surveyID;
                else if (Page is BaseActionForm)
                    result = (Page as BaseActionForm).SurveyID;
                else
                {
                    throw new InternalErrorException(Strings.SurveyIDNotSpecified);
                }
                return result;
            }
            set { m_surveyID = value; }
        }

        /// <summary>
        /// Selected shift type ID.
        /// </summary>
        public int SelectedShiftTypeID
        {
            get
            {
                return Int32.Parse(SelectedValue);

            }
            set
            {
                int shiftTypeID;

                //Note: None constant is less then zero. 
                //therefore check for None should be done before check for Any valid
                if (value == (int)CallShiftType.None)
                {
                    shiftTypeID = (int)CallShiftType.None;
                }
                else if (value <= 0) // Any Valid
                {
                    shiftTypeID = (int)CallShiftType.AnyValid;
                }
                else //Specific shift type
                {
                    shiftTypeID = value;
                }

                SelectedValue = shiftTypeID.ToString();
            }
        }

        override protected void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                foreach (ShiftType shiftType in SurveyManager.GetShiftTypes(SurveyID))
                {
                    Items.Add(new ListItem(shiftType.Name, shiftType.ObjectId.ToString()));
                }
                Items.Insert(0, new ListItem("[None]", ((int)CallShiftType.None).ToString()));
                Items.Insert(1, new ListItem("[Any Valid]", ((int)CallShiftType.AnyValid).ToString()));   
            }
        }
    }
}
