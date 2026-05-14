using System;
using System.Web;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.ActivityViews
{
    public partial class TerminateTask : BaseForm
    {
        [StoreInViewState]
        protected int PersonId;

        private readonly IActivityManager _activityManager;

        public TerminateTask()
        {
            _activityManager = ServiceLocator.Resolve<IActivityManager>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PersonId = Convert.ToInt32(Request["PersonId"]);
                var person = PersonRepository.GetById(PersonId);
                lblConfirmation.Text = String.Format(Strings.cf_TerminateTask, HttpUtility.HtmlEncode(person.Name));
            }
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                string reason = null;

                if (rblReason.SelectedIndex != 0 || !string.IsNullOrWhiteSpace(tbxComments.Text))
                {
                    reason = string.Format("Predefined reason: {0}", rblReason.SelectedItem.Text);

                    if (!string.IsNullOrWhiteSpace(tbxComments.Text))
                    {
                        reason += string.Format("; Comments: {0}", tbxComments.Text);
                    }
                }

                _activityManager.TerminateTaskByPerson(PersonId, reason);

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}