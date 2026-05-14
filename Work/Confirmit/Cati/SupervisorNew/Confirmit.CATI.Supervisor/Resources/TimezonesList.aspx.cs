using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.ServerControls;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class TimezonesList : BaseForm
    {
        public override string TopTitle => Strings.Timezones;

        private IEnumerable<BvCallCenterEntity> _callCentersCache;
        private readonly ICachedLocalTimezoneManager _localTimezoneManager = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        private readonly ITimezoneManager _timezoneManager = ServiceLocator.Resolve<ITimezoneManager>();
        private IEnumerable<int> _depricatedTimezoneIds;

        private IEnumerable<BvCallCenterEntity> CallCentersCache => _callCentersCache ?? (_callCentersCache = ServiceLocator.Resolve<ICallCenterRepository>().GetAll());

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                grid.HintText = Strings.TimezonesHint;
            }

            grid.GetPage +=
                delegate (out int totalCount)
                {
                    var timezoneEntities = _timezoneManager.GetTimezones();

                    var systemTimezoneNames = new HashSet<string>(_timezoneManager.GetSystemTimezoneNames());
                    _depricatedTimezoneIds = timezoneEntities.Where(tz => !systemTimezoneNames.Contains(tz.StandardName)).Select(tz => tz.Id).ToList();
                    return BaseMethods.GetPage(timezoneEntities, grid.PageArguments, out totalCount);
                };

            var column = grid.Columns.FromKey("IsActive") as GeneralGridColumn;
            column.Items.Add(new ListItem("Yes"));
            column.Items.Add(new ListItem("Yes + Custom"));

            grid.InitializeRow += Grid_InitializeRow;
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            var timeZone = e.Row.DataItem as TimezoneEntity;

            if (timeZone == null)
                return;

            if (_depricatedTimezoneIds.Contains(timeZone.Id))
            {
                e.Row.CssClass += " strikethrough";
                timeZone.Name += " (deprecated)";
            }

            if (_localTimezoneManager.GetLocalTimezoneId() == timeZone.Id)
            {
                e.Row.CssClass += " LocalTimezone";
            }
            else if (CallCentersCache.Any(cc => cc.LocalTimezoneId == timeZone.Id))
            {
                e.Row.CssClass += " greyFont";
            }

            if (!string.IsNullOrEmpty(timeZone.DaylightName))
            {
                if (timeZone.IsDaylightSavingTimeNow)
                {
                    e.Row.Items.FindItemByKey("DaylightBias").CssClass += " greenFont";
                }
                else
                {
                    e.Row.Items.FindItemByKey("Bias").CssClass += " greenFont";
                }
            }
        }

        /// <summary>
        /// Adds selected timezone to active timezones list.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ActivateTimezone(object sender, EventArgs e)
        {
            try
            {
                using (var transaction = new DatabaseTransactionScope("Supervisor.ActivateTimezone", DeadlockPriority.Supervisor))
                {
                    int timezoneID = grid.SelectedKeysInt[0];
                    TimezoneManager.AddTimezone(timezoneID);

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void SetLocalTimezone(object sender, EventArgs e)
        {
            try
            {
                using (var transaction = new DatabaseTransactionScope("Supervisor.SetLocalTimezone", DeadlockPriority.Supervisor))
                {
                    int timezoneID = grid.SelectedKeysInt[0];
                    TimezoneManager.AddTimezone(timezoneID);

                    _localTimezoneManager.ChangeLocal(timezoneID);

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void DeactivateTimezone(object sender, EventArgs e)
        {
            try
            {
                int timezoneId = grid.SelectedKeysInt[0];
                if (TimezoneManager.ActiveTimezonesList.All(x => x.ID != timezoneId))
                {
                    AddUserMessage(Strings.TimezoneIsAlreadyDeactivated);
                    return;
                }

                using (var transaction = new DatabaseTransactionScope("Supervisor.DeleteTimezone", DeadlockPriority.Supervisor))
                {

                    TimezoneManager.DeleteTimezone(timezoneId);

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void DeactivateUnused(object sender, EventArgs e)
        {
            try
            {
                using (var transaction = new DatabaseTransactionScope("Supervisor.DeleteUnusedTimezones", DeadlockPriority.Supervisor))
                {
                    TimezoneManager.DeleteUnusedTimezones();

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}