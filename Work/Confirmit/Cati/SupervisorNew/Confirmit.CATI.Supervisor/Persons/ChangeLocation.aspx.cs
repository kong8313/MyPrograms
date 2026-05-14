using System;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Classes;
using System.Linq;

namespace Confirmit.CATI.Supervisor.Persons
{
    public partial class ChangeLocation : BaseForm
    {
        [StoreInViewState]
        public List<int> SelectedIds;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SelectedIds =
                    Request.Params["IDS"].Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(Int32.Parse)
                                         .ToList();

                if (SelectedIds.Count == 1)
                {
                    tbLocation.Text = PersonRepository.GetById(SelectedIds[0]).Location;
                }
            }
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                using (var transaction = new DatabaseTransactionScope("Supervisor.ChangePersonLocation", DeadlockPriority.Supervisor))
                {
                    PersonService.ChangeLocation(SelectedIds, tbLocation.Text);
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