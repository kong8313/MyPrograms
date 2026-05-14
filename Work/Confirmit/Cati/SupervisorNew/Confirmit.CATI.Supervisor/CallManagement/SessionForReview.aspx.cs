using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Core;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.CallManagement
{
    public partial class SessionForReview : BaseActionForm
    {
        private readonly IReviewerService _reviewerService;

        protected SessionForReview()
        {
            _reviewerService = ServiceLocator.Resolve<IReviewerService>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var survey = SurveyRepository.GetById(SurveyID);
                SessionNameDialog.OKButton.Text = Strings.Dlg_Ok;
                SessionNameTextBox.Text = ReviewerServiceHelper.GetDefaultSessionName(User.Name, survey.ProjectId);
                SessionUrlDialog.OKButton.Text = Strings.Dlg_Close;
                SessionUrlDialog.Visible = false;
            }
        }

        protected void OkButtonClick(object sender, EventArgs e)
        {
            AntiForgery.Validate();
            RequiredSessionNameValidator.Validate();

            if (!RequiredSessionNameValidator.IsValid)
                return;

            try
            {
                LegacySupervisorMetrics.OnCallManagementAction("ReviewerCreateSession");
                var url = _reviewerService.CreateSessionForReview(SessionNameTextBox.Text,
                    SurveyID,
                    User.Name,
                    BatchParameters
                    );

                SessionUrlTextBox.Text = UrlHelper.ModifyUrlProtocol(url);

                SessionUrlTextBox.Focus();
                SessionUrlTextBox.Attributes.Add("OnFocus", "this.select();");
            }
            catch (Exception exception)
            {
                Context.AddError(exception); 
                CloseOverlay(true);
                return;
            }

            SessionNameDialog.Visible = false;
            SessionUrlDialog.Visible = true;
        }

        protected void CloseButtonClick(object sender, EventArgs e)
        {
            CloseOverlay(true);
        }

    }
}
