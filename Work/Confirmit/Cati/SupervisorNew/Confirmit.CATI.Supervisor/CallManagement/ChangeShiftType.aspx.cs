using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Filters;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Supervisor.CallManagement
{
    public partial class ChangeShiftType : BaseActionForm
    {
        #region Properties.

        private Int32 SelectedShiftType
        {
            get
            {
                return ddlShiftType.SelectedShiftTypeID;
            }
        }

        #endregion

        #region Event Handlers

        protected override void OnPreRender(EventArgs e)
        {
            if (IsPostBack == false)
            {
                ddlShiftType.SelectedShiftTypeID = IDS.Any() ? Calculator.Calculate(SurveyID, IDS, entity => entity.ShiftID, (int) CallShiftType.None) : (int) CallShiftType.None;
            }

            base.OnPreRender(e);
        }

        protected void SaveButtonClick(object sender, EventArgs e)
        {
            try
            {
                LegacySupervisorMetrics.OnCallManagementAction("ChangeShiftType");
                var operationEntity = CallManager.ChangeShiftTypeOfCalls(SurveyID, SelectedShiftType, BatchParameters);

                Redirect(operationEntity);

            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        #endregion
    }
}
