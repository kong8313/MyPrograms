using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement;
using Confirmit.CATI.Supervisor.Resources;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.CallManagement.Controls
{
    public partial class CustomViewProperties : BaseForm
    {
        protected enum ActionType
        {
            Add,
            Edit
        }

        private readonly ISupervisorSettingsRepository _supervisorSettingsRepository;
        private readonly ICallManagementViewsProvider _callManagementViewProvider;
        private readonly IToggleSettings _toggleSettings;

        [StoreInViewState]
        protected List<QuotaDetails> ColumnsList;

        [StoreInViewState]
        protected ObservableCollection<KeyValuePair<string, string>> SortedColumns;

        [StoreInViewState]
        protected ActionType Type;

        [StoreInViewState]
        protected string SelectedViewName;

        [StoreInViewState]
        protected int SurveyId;

        private BvSurveyEntity _survey;

        protected BvSurveyEntity Survey
        {
            get { return _survey ?? (_survey = SurveyRepository.GetById(SurveyId)); }
        }

        public CustomViewProperties()
        {
            _supervisorSettingsRepository = ServiceLocator.Resolve<ISupervisorSettingsRepository>();
            _callManagementViewProvider = ServiceLocator.Resolve<ICallManagementViewsProvider>();
            _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();
        }

        private CallManagementViews GetCallManagementViews()
        {
            var callManagementViews = Session[SessionVariablesLiterals.ViewsCallManagement] as CallManagementViews;
            if (callManagementViews == null)
            {
                callManagementViews = _supervisorSettingsRepository.ReadCallManagementViews();
                Session[SessionVariablesLiterals.ViewsCallManagement] = callManagementViews;
            }

            return callManagementViews;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Type = (ActionType)Enum.Parse(typeof(ActionType), Request.Params["Type"]);
                SurveyId = Convert.ToInt32(Request.Params["SurveyId"]);

                if (Type == ActionType.Add)
                {
                    PrepareAddForm();
                }
                else
                {
                    PrepareEditForm();
                }

            }

            columnNamesGrid.GetPage = GetPage;
        }

        private void DefineSortedColumns(List<CallManagementColumn> columns)
        {
            SortedColumns = new ObservableCollection<KeyValuePair<string, string>>(columns
                .Where(x => IsVisible(x.ColumnKey.ToString()) && x.ColumnKey != CallManagementColumnKey.InterviewID)
                .Select(x => new KeyValuePair<string, string>(x.ColumnKey.ToString(), _callManagementViewProvider.GetTranslation(x.ColumnKey))));

            columnNamesGrid.SelectedKeys = columns
                .Where(x => x.IsVisible && IsVisible(x.ColumnKey.ToString()) && x.ColumnKey != CallManagementColumnKey.InterviewID)
                .Select(x => _callManagementViewProvider.GetTranslation(x.ColumnKey)).ToArray();
        }

        private void PrepareAddForm()
        {
            DefineSortedColumns(_callManagementViewProvider.ScheduledColumnKeys.Select(x => new CallManagementColumn { ColumnKey = x, IsVisible = true }).ToList());
        }

        private void PrepareEditForm()
        {
            SelectedViewName = Request.Params["SelectedViewName"];
            tbxCusomViewName.Text = SelectedViewName;

            var callManagementViews = GetCallManagementViews();
            var callManagementView = callManagementViews.Views.First(x => x.Name == SelectedViewName);

            cbxIsDefault.Checked = callManagementView.IsDefault;

            DefineSortedColumns(callManagementView.Columns);
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                var callManagementViews = GetCallManagementViews();

                if (!ValidateName(callManagementViews))
                {
                    return;
                }

                var callManagementCustomViewEvent = ChangeCallManagementViews(callManagementViews);

                _supervisorSettingsRepository.WriteCallManagementViews(callManagementViews);

                callManagementCustomViewEvent.Finish();

                CloseOverlay(true, tbxCusomViewName.Text);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private ManagementActivityEvent<CallManagementCustomViewParameters> ChangeCallManagementViews(CallManagementViews callManagementViews)
        {
            if (Type == ActionType.Add)
            {
                if (cbxIsDefault.Checked)
                {
                    RemoveFlagForDefaultView(callManagementViews);
                }

                CallManagementView newView = new CallManagementView
                {
                    Name = tbxCusomViewName.Text,
                    IsDefault = cbxIsDefault.Checked,
                    Columns = GetSelectedColumns()
                };

                callManagementViews.Views.Add(newView);

                return new AddCallManagementCustomViewEvent(SurveyId, Survey.ProjectId, tbxCusomViewName.Text, cbxIsDefault.Checked, GetColumnInfo(newView.Columns));
            }

            // A part to edit an existing view
            var callManagementView = callManagementViews.Views.First(x => x.Name == SelectedViewName);
            callManagementView.Name = tbxCusomViewName.Text;
            callManagementView.Columns = GetSelectedColumns();
            SetDefaultValue(callManagementView, callManagementViews);

            return new EditCallManagementCustomViewEvent(SurveyId, Survey.ProjectId, tbxCusomViewName.Text, cbxIsDefault.Checked, GetColumnInfo(callManagementView.Columns));
        }

        private string GetColumnInfo(List<CallManagementColumn> columns)
        {
            var info = new StringBuilder();
            foreach (var column in columns)
            {
                if (column.IsVisible)
                {
                    info.Append(_callManagementViewProvider.GetTranslation(column.ColumnKey) + ", ");
                }
            }

            return info.ToString().TrimEnd(' ', ',');
        }

        private void SetDefaultValue(CallManagementView callManagementView, CallManagementViews callManagementViews)
        {
            // if this view was default and a user has removed the default flag we have to set scheduled view as default view
            if (callManagementView.IsDefault && !cbxIsDefault.Checked)
            {
                callManagementViews.Views[0].IsDefault = true;
            }
            // if this view wasn't default and a user set default flag for it we have to remove default flag from default view
            else if (!callManagementView.IsDefault && cbxIsDefault.Checked)
            {
                RemoveFlagForDefaultView(callManagementViews);
            }

            callManagementView.IsDefault = cbxIsDefault.Checked;
        }

        private List<CallManagementColumn> GetSelectedColumns()
        {
            var result = new List<CallManagementColumn>
            {
                new CallManagementColumn { ColumnKey = CallManagementColumnKey.InterviewID, IsVisible = true }
            };

            result.AddRange(SortedColumns.Select(sortedColumn => new CallManagementColumn
            {
                ColumnKey = (CallManagementColumnKey)Enum.Parse(typeof(CallManagementColumnKey), sortedColumn.Key),
                IsVisible = columnNamesGrid.CheckedKeys.Any(key => key == sortedColumn.Value)
            }));

            return result;
        }

        private void RemoveFlagForDefaultView(CallManagementViews callManagementViews)
        {
            var callManagementView = callManagementViews.Views.First(x => x.IsDefault);
            callManagementView.IsDefault = false;
        }

        private bool ValidateName(CallManagementViews callManagementViews)
        {
            string customViewName = tbxCusomViewName.Text;
            if (string.IsNullOrEmpty(customViewName))
            {
                AddUserMessage(Strings.EmptyCustomViewName);
                return false;
            }

            if (!customViewName.All(x=> char.IsLetterOrDigit(x) || x == '_')) 
            {
                AddUserMessage(Strings.WrongCustomViewName);
                return false;
            }

            if (Type == ActionType.Add && callManagementViews.Views.Any(x => x.Name == customViewName))
            {
                AddUserMessage(Strings.DuplicationCustomViewName);
                return false;
            }

            return true;
        }

        protected object GetPage(out int totalCount)
        {
            var orderedList = SortedColumns.Select((value, index) => new
            {
                Priority = index,
                Name = value.Value,
                Key = value.Key
            });

            return BaseMethods.GetPage(orderedList, columnNamesGrid.PageArguments, out totalCount);
        }

        private bool IsVisible(string key)
        {
            if (key == "DialingMode" && Survey.DialingMode != DialingMode.Predictive)
            {
                return false;
            }

            if (key == "DialTypeId" && !_toggleSettings.ShowDialType)
            {
                return false;
            }

            return true;
        }

        protected void MoveUp(object sender, EventArgs e)
        {
            var item = SortedColumns.First(x => x.Value == columnNamesGrid.HighlightedKey);
            var position = SortedColumns.IndexOf(item);
            var newPosition = position - 1 >= 0 ? position - 1 : 0;
            SortedColumns.Move(position, newPosition);
        }

        protected void MoveDown(object sender, EventArgs e)
        {
            var item = SortedColumns.First(x => x.Value == columnNamesGrid.HighlightedKey);
            var position = SortedColumns.IndexOf(item);
            var newPosition = position + 1 <= SortedColumns.Count - 1 ? position + 1 : SortedColumns.Count - 1;
            SortedColumns.Move(position, newPosition);
        }
    }
}
