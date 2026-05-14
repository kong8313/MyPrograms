using System;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Core.ITSs;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    /// <summary>
    /// Dropdown with extended statuses (without 'disallow activation' statuses).
    /// </summary>
    public class ExtendedStatusDropDown : DropDownList
    {
        private int m_surveyID;

        /// <summary>
        /// Fusion survey SID to get extended statuses for.
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
        /// Selected extended status ID.
        /// </summary>
        public int? SelectedExtendedStatusID
        {
            get
            {
                var success = int.TryParse(SelectedValue, out var result);
                if (success)
                {
                    return result;
                }

                return null;
            }
            set
            {
                int? extendedStatusID;

                extendedStatusID = value;
                SelectedValue = extendedStatusID.ToString();
            }
        }

        override protected void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                var stateGroupId = SurveyRepository.GetById(SurveyID).StateGroupID;

                foreach (BvSpState_ListEntity state in StateGroupsManager.GetITSList(stateGroupId))
                {
                    if (state.DA == 0)
                    {
                        Items.Add(new ListItem(state.Name, state.StateID.ToString()));
                    }
                }

                Items.Insert(0, new ListItem("[No change]", null));
            }
        }
    }
}
