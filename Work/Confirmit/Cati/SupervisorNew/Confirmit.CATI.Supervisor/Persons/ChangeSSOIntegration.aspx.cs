using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Confirmit.CATI.Supervisor.Persons
{
    public partial class ChangeSSOIntegration : BaseForm
    {
        [StoreInViewState]
        public List<int> SelectedIds;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SelectedIds =
                    Request.Params["IDS"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(Int32.Parse)
                                         .ToList();

                if (SelectedIds.Count == 1)
                {
                    ddlSSOIntegration.SelectedIndex = PersonRepository.GetById(SelectedIds[0]).EnableSoftphoneIntegration ? 1 : 0;
                }
            }
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                using (var transaction = new DatabaseTransactionScope("Supervisor.ChangePersonLocation", DeadlockPriority.Supervisor))
                {
                    PersonService.ChangeEnableSoftphoneIntegration(SelectedIds, Convert.ToBoolean(ddlSSOIntegration.SelectedIndex));
                    transaction.Commit();
                }

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}