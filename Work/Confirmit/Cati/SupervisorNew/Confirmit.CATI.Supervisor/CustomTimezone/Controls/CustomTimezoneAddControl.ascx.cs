using System;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.CustomTimezone.Controls
{
    /// <summary>
    ///		Summary description for CustomTimezoneAdd.
    /// </summary>
    public partial class CustomTimezoneAdd : BaseWUC
    {
        [StoreInViewState]
        public int CustomTimezoneId;

        [StoreInViewState]
        public int ParentTimezoneId;

        protected bool IsNew => CustomTimezoneId == int.MinValue;

        private readonly ITimezoneManager _timezoneManager = ServiceLocator.Resolve<ITimezoneManager>();

        public event EventHandler CustomTimezoneSaved;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateTimezoneName();

                dialogControl.OKButton.Text = IsNew ? "Add" : "Save";
            }
        }

        private void UpdateTimezoneName()
        {
            if (!IsNew)
            {
                var timezone = _timezoneManager.GetActiveTimezone(CustomTimezoneId);
                EdtName.Text = timezone == null ? "" : timezone.Name.Split(')')[1].Trim();
            }
        }

        protected void BtnSave_ServerClick(object sender, EventArgs e)
        {
            var parentTimezone = _timezoneManager.GetMasterTimezoneInfo(ParentTimezoneId);
            var name = GetCustomTimezoneName(parentTimezone.DisplayName, EdtName.Text.Trim());

            if (!IsCustomTimezoneNameValid(name))
            {
                ShowClientMessage(Strings.InvalidCustomTimezoneName);
                return;
            }

            if (IsNew)
            {
                Save(name);
            }
            else
            {
                Update(name);
            }

            CustomTimezoneSaved?.Invoke(this, EventArgs.Empty);
        }

        private static string GetCustomTimezoneName(string parentTimezoneName, string customName)
        {
            var gmtBias = parentTimezoneName.Split('(', ')')[1];
            return $"({gmtBias}) {customName}";
        }

        private void Update(string name)
        {
            try
            {
                using (var transactionScope = new DatabaseTransactionScope("UpdateCustomTimezone", DeadlockPriority.Supervisor))
                {
                    _timezoneManager.UpdateCustomTimezone(CustomTimezoneId, name, ParentTimezoneId);

                    transactionScope.Commit();
                }
            }
            catch (Exception ex)
            {
                Page.AddUserMessage(string.Format(Strings.CustomTimezoneCannotBeUpdated, name), ex);
            }
        }

        private void Save(string name)
        {
            try
            {
                using (var transactionScope = new DatabaseTransactionScope("InsertCustomTimezone", DeadlockPriority.Supervisor))
                {
                    var isParentActive = _timezoneManager.GetActiveTimezone(ParentTimezoneId) != null;
                    if (!isParentActive)
                    {
                        TimezoneManager.AddTimezone(ParentTimezoneId);
                    }

                    _timezoneManager.AddCustomTimezone(name, ParentTimezoneId);

                    transactionScope.Commit();
                }
            }
            catch (Exception ex)
            {
                Page.AddUserMessage(string.Format(Strings.CustomTimezoneCannotBeAdded, name), ex);
            }
        }

        private static bool IsCustomTimezoneNameValid(string name)
        {
            var activeTimezones = TimezoneManager.ActiveTimezonesList;
            var masterTimezones = TimezoneManager.GetMasterTimezonesList();

            return activeTimezones.All(x => x.Name != name) && masterTimezones.All(x => x.Name != name);
        }
    }
}
