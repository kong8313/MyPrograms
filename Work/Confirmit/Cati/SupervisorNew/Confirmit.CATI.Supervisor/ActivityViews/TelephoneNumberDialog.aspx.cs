using System;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.ActivityViews
{
    public partial class TelephoneNumberDialog : BaseForm
    {
        private readonly ICookieDataAccess _cookieDataAccess;
        private const string TelephoneNumberCookieName = @"TelephonyNumberStorageForSupervisor";

        public TelephoneNumberDialog()
        {
            _cookieDataAccess = new CookieDataAccess();
        }

        /// <summary>
        /// Session key used for transfer data to InterviewerActivity dialog
        /// </summary>
        public string SessionKey
        {
            get
            {
                return (string)(ViewState["SessionKey"] ?? String.Empty);
            }
            set
            {
                ViewState["SessionKey"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                SessionKey = Request["SessionKey"];

                var value = _cookieDataAccess.GetValue(TelephoneNumberCookieName);

                if (!string.IsNullOrEmpty(value))
                {
                    tbTelephoneNumber.Text = value;
                }
            }

            TelephonyNumberHelper.SetTelephonyNumber(SessionKey, String.Empty);
            TelephonyNumberHelper.SetDialogResult(SessionKey, DialogResult.StartOnlyVideo);
        }

        protected override void ValidateForm()
        {
        }

        /// <summary>
        /// Executing task choice change for users/groups
        /// </summary>
        protected void btnStart_Click(object sender, EventArgs e)
        {
            base.ValidateForm();

            _cookieDataAccess.SetValue(TelephoneNumberCookieName, tbTelephoneNumber.Text, DateTime.Now.AddDays(7d));

            TelephonyNumberHelper.SetDialogResult(SessionKey, DialogResult.StartAudioVideo);
            TelephonyNumberHelper.SetTelephonyNumber(SessionKey, tbTelephoneNumber.Text);

            CloseOverlay(true);
        }

        protected void btnOnlyVideo_Click(object sender, EventArgs e)
        {
            TelephonyNumberHelper.SetDialogResult(SessionKey, DialogResult.StartOnlyVideo);
            TelephonyNumberHelper.SetTelephonyNumber(SessionKey, String.Empty);

            CloseOverlay(true);
        }
    }
}
