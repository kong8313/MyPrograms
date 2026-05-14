using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;
using Infragistics.Web.UI.LayoutControls;

namespace Confirmit.CATI.Supervisor.Surveys
{
    public partial class CallHistoryProperties : BaseForm
    {
        [StoreInViewState]
        protected BvHistoryEntity CallHistoryEntity;

        private readonly IHistoryRepository _historyRepository = ServiceLocator.Resolve<IHistoryRepository>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var callHistoryId = Convert.ToInt32(Request["CallHistoryId"]);
                CallHistoryEntity = _historyRepository.GetById(callHistoryId);

                ddlITS.DataSource = SurveyService.GetTransientStates(CallHistoryEntity.SurveyId);
                ddlITS.DataValueField = "StateID";
                ddlITS.DataTextField = "Name";
                ddlITS.DataBind();

                tbTelephoneNumber.Text = CallHistoryEntity.TelephoneNumber;
                ddlITS.SelectedValue = CallHistoryEntity.ITS.ToString();

                ClientScript.RegisterOnSubmitStatement(GetType(), "confirm", "return confirm('" + Strings.cnfr_EditCallAttempt + "');");
            }

            dialog.OKButton.Text = Strings.Save;
            dialog.SetCancelAction(GetCloseOverlayScript(true, null, true));
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (!User.IsCatiAdministratorOrPros)
                {
                    AddUserMessage(Strings.PermissionDenied);
                    return;
                }

                CallHistoryEntity.TelephoneNumber = tbTelephoneNumber.Text;
                CallHistoryEntity.ITS = Convert.ToInt16(ddlITS.SelectedItem.Value);

                _historyRepository.Update(CallHistoryEntity);

                CloseOverlay(true, null, true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}