using System;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.CallCenters;

namespace Confirmit.CATI.Supervisor.Controls
{
    public partial class AppointmentProperties : BaseWUC
    {
        private readonly ICallCenterProvider _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();
        private readonly ITimezoneService _timezoneService = ServiceLocator.Resolve<ITimezoneService>();
        
        public Appointment AppointmentData
        {
            get
            {
                var timezoneId = _callCenterProvider.GetCurrent().LocalTimezoneId;
                return new Appointment
                {
                    state = 0,
                    time = cbxTimeNow.Checked ? DateTime.UtcNow : _timezoneService.ConvertTimeToUtc(timezoneId, dteAppointmentTime.DateTimeValue),
                    expirationTime = cbxTimeToExpire.Checked ? (DateTime?)null : _timezoneService.ConvertTimeToUtc(timezoneId, dteTimeToExpire.DateTimeValue), 
                    contactName = tbContactName.Text
                };
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterClientScripts();
        }

        private void RegisterClientScripts()
        {
            cbxTimeNow.Attributes.Add("onclick", dteAppointmentTime.ClientControllerName + ".setEnabled(!this.checked);");
            cbxTimeToExpire.Attributes.Add("onclick", dteTimeToExpire.ClientControllerName + ".setEnabled(!this.checked);");

            dteAppointmentTime.Enabled = !cbxTimeNow.Checked;
            dteTimeToExpire.Enabled = !cbxTimeToExpire.Checked;
        }
    }
}