using System;
using System.Linq;
using System.Globalization;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.CallCenters;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;

namespace Confirmit.CATI.Supervisor.CallCenters
{
    public partial class CallCenterProperties : CallCenterAdminForm
    {
        [StoreInViewState]
        protected byte? CallCenterId;

        private readonly IDialersRepository _dialersRepository;
        private ICallCenterProvider _callCenterProvider;

        protected int[] DialerIds
        {
            get { return tbDialerIds.Text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray(); }
        }

        public CallCenterProperties()
        {
            _dialersRepository = ServiceLocator.Resolve<IDialersRepository>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();

            if (IsPostBack)
            {
                return;
            }

            BindActiveTimezonesList();

            if (!string.IsNullOrEmpty(Request["ID"]))
            {
                CallCenterId = Byte.Parse(Request["ID"]);
            }

            _dialog.OKButton.Text = CallCenterId.HasValue ? "Save" : "Create";

            if (CallCenterId.HasValue)
            {
                BindExistingCallCenterData();
            }

            if (CallCenterId.HasValue)
            {
                return;
            }

            _activeTimezones.SelectedValue =
                CallCenterRepository.Default.LocalTimezoneId.ToString(CultureInfo.InvariantCulture);
        }

        protected void SaveButtonClick(object sender, EventArgs e)
        {
            if (ValidateInput() == false)
            {
                return;
            }

            if (CallCenterId.HasValue)
            {
                var callCenter = CallCenterRepository.GetCallCenterWithDialers(CallCenterId.Value);

                var oldDialerIds = callCenter.DialerIds;
                string oldCallCanterName = callCenter.Name;

                FillCallCentrerEntity(callCenter);

                if (!ValidateLoggedPersonsIfDialerChanged(callCenter.ID, callCenter.DialerIds, oldDialerIds, oldCallCanterName))
                {
                    return;
                }

                CallCenterRepository.Update(callCenter, callCenter.DialerIds, oldDialerIds);

                if (callCenter.ID == _callCenterProvider.GetCurrentId())
                {
                    RegisterStartupScript("refreshCallCenterInfo();");
                }
            }
            else
            {
                var callCenter = new BvCallCenterEntityWithDialerIds();
                FillCallCentrerEntity(callCenter);

                CallCenterRepository.Insert(callCenter);
            }

            CloseOverlay(true);
        }

        private void BindActiveTimezonesList()
        {
            _activeTimezones.DataSource = TimezoneManager.ActiveTimezonesList.Where(tz => !tz.IsCustom).OrderBy(tz => tz.Name);
            _activeTimezones.DataValueField = "ID";
            _activeTimezones.DataTextField = "Name";
            _activeTimezones.DataBind();
        }

        private void BindExistingCallCenterData()
        {
            if (!CallCenterId.HasValue)
            {
                throw new InvalidOperationException("Couldn't get call center id");
            }

            var callCenter = CallCenterRepository.GetCallCenterWithDialers(CallCenterId.Value);

            tbCallCenterName.Text = callCenter.Name;
            tbDescription.Text = callCenter.Description;
            _activeTimezones.SelectedValue = callCenter.LocalTimezoneId.ToString(CultureInfo.InvariantCulture);
            cbHidePii.Checked = callCenter.HidePii;
            tbDialerIds.Text = string.Join(" ", callCenter.DialerIds);
        }

        private void FillCallCentrerEntity(BvCallCenterEntityWithDialerIds callCenter)
        {
            callCenter.Name = tbCallCenterName.Text.Trim();
            callCenter.Description = tbDescription.Text.Trim();
            callCenter.LocalTimezoneId = Int32.Parse(_activeTimezones.SelectedValue);
            callCenter.DialerId = DialerIds.FirstOrDefault();
            callCenter.DialerIds = DialerIds;
            callCenter.HidePii = cbHidePii.Checked;
        }

        private bool ValidateInput()
        {
            if (String.IsNullOrEmpty(tbCallCenterName.Text.Trim()))
            {
                AddUserMessage(Strings.Err_EmptyName);
                return false;
            }

            if (!String.IsNullOrEmpty(tbDialerIds.Text.Trim()))
            {
                var allDialers = _dialersRepository.GetAll().Select(x => x.Id).ToArray();
                var notExistDialers = DialerIds.Except(allDialers);

                if (notExistDialers.Any())
                {
                    AddUserMessage(string.Format(Strings.Err_WrongDialerId, string.Join(" ", notExistDialers)));
                    return false;
                }
            }

            bool isNameInUse = CallCenterService.IsNameAlreadyInUse(tbCallCenterName.Text);

            if (isNameInUse)
            {
                if (CallCenterId.HasValue == false ||
                    (CallCenterRepository.Get(CallCenterId.Value).Name != tbCallCenterName.Text))
                {
                    AddUserMessage(Strings.Error_CallCenterProperties_NameIsAlreadyUsed);
                    return false;
                }
            }

            return true;
        }

        private bool ValidateLoggedPersonsIfDialerChanged(
            int callCenterId,
            int[] dialerIds,
            int[] oldDialerIds,
            string callCenterName)
        {
            var dialerIdsChanged = false;
            if (dialerIds.Length == oldDialerIds.Length)
            {
                foreach(int id in oldDialerIds)
                {
                    if (!dialerIds.Contains(id))
                    {
                        dialerIdsChanged = true;
                        break;
                    }
                }

                if (!dialerIdsChanged)
                {
                    return true;
                }
            }

            if (ServiceLocator.Resolve<ICallCenterService>().HasLoggedInPersons(callCenterId, 0))
            {
                ShowClientMessage(
                    string.Format(Strings.Error_CallCenterProperties_CallCenterHasLoggedPersons, callCenterName));

                return false;
            }

            return true;
        }
    }
}
