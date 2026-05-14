using System;
using System.Drawing;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class ExternalTransferProperties : BaseForm
    {
        private readonly IExternalTransferTelephoneNumberRepository _externalTransferTelephoneNumberRepository =
            ServiceLocator.Resolve<IExternalTransferTelephoneNumberRepository>();
        private readonly IExternalTransferTelephoneNumberService _externalTransferTelephoneNumberService = 
            ServiceLocator.Resolve<IExternalTransferTelephoneNumberService>();

        [StoreInViewState]
        protected int Id;

        private bool IsNew => Id == 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (int.TryParse(Request["ID"], out var id))
                Id = id;

            if (!IsPostBack)
            {
                if (IsNew)
                {
                    doubleGrid.SelectedSurveysIds = new int[]{};
                }
                else
                {
                    var number = _externalTransferTelephoneNumberRepository.TryGetById(Id);
                    if (number != null)
                    {
                        tbTelephoneNumber.Text = number.TelephoneNumber;
                        tbDescription.Text = number.Description;
                        cbIsHidden.Checked = number.Hidden;
                    }
                    doubleGrid.SelectedSurveysIds = _externalTransferTelephoneNumberService.GetAssignedSurveyIds(Id);
                }
            }

            dialog.OKButton.Text = IsNew ? Strings.Add : Strings.Save;
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            var existingNumbers = _externalTransferTelephoneNumberRepository.TryGetByTelephoneNumber(tbTelephoneNumber.Text);

            if (IsNew)
            {
                
                if (existingNumbers == null)
                {
                    _externalTransferTelephoneNumberService.InsertNumber(
                        tbTelephoneNumber.Text,
                        tbDescription.Text, 
                        cbIsHidden.Checked,
                        doubleGrid.SelectedSurveysIds);
                }
                else
                {
                    AddUserMessage(Strings.Err_DuplicateTelephoneNumber);
                    return;
                }
            }
            else
            {
                if (existingNumbers == null || existingNumbers.Id == Id)
                {
                    _externalTransferTelephoneNumberService.UpdateNumber(
                        Id,
                        tbTelephoneNumber.Text,
                        tbDescription.Text,
                        cbIsHidden.Checked,
                        doubleGrid.SelectedSurveysIds);
                }
                else
                {
                    AddUserMessage(Strings.Err_DuplicateTelephoneNumber);
                    return;
                }
            }
            CloseOverlay(true);
        }
    }
}