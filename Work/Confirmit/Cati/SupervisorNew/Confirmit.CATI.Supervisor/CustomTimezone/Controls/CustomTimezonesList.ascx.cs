using System;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Strings = Confirmit.CATI.Supervisor.Resources.Strings;

namespace Confirmit.CATI.Supervisor.CustomTimezone.Controls
{
    /// <summary>
    ///		Summary description for CustomTimezonesList.
    /// </summary>
    public partial class CustomTimezonesList : BaseWUC
    {
        public int ParentTimezoneId { get; set; }

        private readonly ITimezoneManager _timezoneManager = ServiceLocator.Resolve<ITimezoneManager>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["Id"] != null)
            {
                ParentTimezoneId = Convert.ToInt32(Request["Id"]);
            }

            customTimezonesGrid.GetPage = null;
            customTimezonesGrid.GetPage += GetPage;

            var timezoneInfo = _timezoneManager.GetMasterTimezoneInfo(ParentTimezoneId);
            customTimezonesGrid.GridName = string.Format(Strings.CustomTimezonesForTimezone, timezoneInfo.DisplayName);
        }

        protected object GetPage(out int totalCount)
        {
            var list = _timezoneManager.GetCustomTimezones(ParentTimezoneId);

            var args = new PagingArgs(customTimezonesGrid.SortedColumnName, customTimezonesGrid.SortIndicatorAsc)
            {
                SearchParameters = customTimezonesGrid.SearchParameterCollection
            };

            return BaseMethods.GetPage(list, args, out totalCount);

        }

        protected void OnCustomTimezonesListChanged(object sender, EventArgs e)
        {
            customTimezonesGrid.RefreshData();
        }
        
        protected void DeleteCustomTimezone(object sender, EventArgs e)
        {
            var id = customTimezonesGrid.SelectedKeys.First();

            try
            {
                using (var transactionScope = new DatabaseTransactionScope("DeleteCustomTimezone", DeadlockPriority.Supervisor))
                {
                    TimezoneManager.DeleteTimezone(int.Parse(id));

                    transactionScope.Commit();
                }
            }
            catch (Exception ex)
            {
                Page.AddUserMessage(string.Format(Strings.CustomTimezoneCannotBeRemoved, id), ex);
            }

            customTimezonesGrid.RefreshData();
            Page.RefreshListFrame();
        }
    }
}