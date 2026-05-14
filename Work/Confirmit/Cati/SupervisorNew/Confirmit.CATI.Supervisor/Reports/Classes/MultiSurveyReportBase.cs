using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using DropDownList = System.Web.UI.WebControls.DropDownList;

namespace Confirmit.CATI.Supervisor.Reports.Classes
{
    public abstract class MultiSurveyReportBase : SurveyReportBase
    {
        private readonly string _selectString = Strings.SelectVariableOption;
        protected abstract HtmlGenericControl VariableFilter { get; }
        protected abstract void UpdateSurveyDataFilter();
        protected abstract Button ShiftsSelectionButton { get; }
        private readonly IConfirmitQuestionsProvider _confirmitQuestionsProvider;

        protected MultiSurveyReportBase()
            : this(ServiceLocator.Resolve<ConfirmitQuestionsProvider>())
        {

        }

        protected MultiSurveyReportBase(IConfirmitQuestionsProvider confirmitQuestionsProvider)
        {
            _confirmitQuestionsProvider = confirmitQuestionsProvider;
        }

        protected override void InitSelectedSurveys(bool isInitial)
        {
            bool allSelected;

            SelectedSurveys = GetSelectedSurveys(isInitial, out allSelected).ToList();
            SelectedSurveysNames = allSelected ? Strings.All : GetSelectedSurveysNames();
            UpdateSurveyDataFilter();
        }

        protected void UpdateFilters(DropDownList variableFilter1, DropDownList variableFilter2)
        {
            if (SelectedSurveys.Count == 1)
            {
                FillDropDownListWithReplicatedColums(variableFilter1);
                FillDropDownListWithReplicatedColums(variableFilter2);
                ShowVariableFilter(true);
            }
            else
            {
                ShowVariableFilter(false);
            }
        }

        protected Dictionary<string, int> GetFiltersData(DropDownList filterDropDownList1, TextBox valueTextBox1, DropDownList filterDropDownList2, TextBox valueTextBox2)
        {
            var filtersData = new Dictionary<string, int>();
            CollectSurveyFilterData(filterDropDownList1, valueTextBox1, ref filtersData);
            CollectSurveyFilterData(filterDropDownList2, valueTextBox2, ref filtersData);

            return filtersData;
        }

        protected void CollectSurveyFilterData(DropDownList filterDropDownList, TextBox valueTextBox, ref Dictionary<string, int> data)
        {
            int filterData;
            if (!Int32.TryParse(valueTextBox.Text, out filterData)) return;

            if (filterDropDownList.SelectedItem.Text != _selectString)
                data.Add(filterDropDownList.Text, filterData);
        }

        protected string GetDbSurveyDataFilterParam(Dictionary<string, int> filtersData)
        {
            return SelectedSurveys.Count == 1 && filtersData.Any() ? FilterService.GetWhereClauseForReplTable(filtersData) : null;
        }

        protected string GetSurveyDataFilterParam(Dictionary<string, int> filtersData)
        {
            return filtersData.Any() ? FilterService.GetWhereClauseForReplTableNoPrefix(filtersData) : "N/A";
        }

        protected void FillDropDownListWithReplicatedColums(DropDownList filterValueDropDownList, string selectedValue = "", string valueToSkip = "")
        {
            filterValueDropDownList.Items.Clear();
            filterValueDropDownList.Items.Insert(0, new ListItem(_selectString, "0"));
            filterValueDropDownList.Items.AddRange(_confirmitQuestionsProvider.GetIntegerBasedReplicatedColumns(SelectedSurveys.Single())
                .Where(x => x.Name != valueToSkip)
                .Select(x => new ListItem(x.Name, x.Name)).ToArray());

            if (!string.IsNullOrEmpty(selectedValue))
            {
                filterValueDropDownList.SelectedIndex =
                    filterValueDropDownList.Items.IndexOf(new ListItem(selectedValue));
            }
        }

        protected string GetShiftTimes()
        {
            ShiftForReport shift = null;

            if (SourceList == SourceList.CatiProductivityReport && ReportsSessionVariables.ShiftForInterviewerProductivityReport != null)
                shift = ReportsSessionVariables.ShiftForInterviewerProductivityReport;

            if (SourceList == SourceList.SurveyOverviewReport && ReportsSessionVariables.ShiftForSurveyOverviewReport != null)
                shift = ReportsSessionVariables.ShiftForSurveyOverviewReport;

            if (SourceList == SourceList.ProductivityReport && ReportsSessionVariables.ShiftForSurveyProductivityReport != null)
                shift = ReportsSessionVariables.ShiftForSurveyProductivityReport;
            
            if (shift != null)
                return string.Format("{0}: {1}-{2}",
                    GetResString("ShiftTimes"),
                    _timezoneProvider.ConvertToLocalTime(shift.StartShiftTime).ToString("T",CultureInfo.InvariantCulture),
                    _timezoneProvider.ConvertToLocalTime(shift.EndShiftTime).ToString("T",CultureInfo.InvariantCulture)
                    );

            return string.Empty;
        }

        protected string InitShiftsSelectionOnClick(string updatePanelClientId)
        {
            return String.Format("Common.selectShiftForReport('{0}', {1});", updatePanelClientId, (int) SourceList );
        }

        protected void SetShiftButtonSelectionMode(Button btnShift, Button other)
        {
            var shiftTimes = GetShiftTimes();

            if (!string.IsNullOrEmpty(shiftTimes))
            {
                btnShift.BorderColor = Color.Orange;
                btnShift.ToolTip = shiftTimes;
            }
            else
            {
                btnShift.BorderColor = other.BorderColor;
                btnShift.ToolTip = string.Empty;
            }
        }

        private void ShowVariableFilter(bool showFilterRow)
        {
            VariableFilter.Visible = showFilterRow;
            CleanTextBoxesInsideControl(VariableFilter);
        }

        private void CleanTextBoxesInsideControl(Control control)
        {
            foreach (Control item in control.Controls)
            {
                var box = item as TextBox;
                if (box != null)
                {
                    box.Text = string.Empty;
                }
                else if (item.HasControls())
                {
                    CleanTextBoxesInsideControl(item);
                }
            }
        }

        private IEnumerable<int> GetSelectedSurveys(bool isInitial, out bool allSelected)
        {
            allSelected = false;

            if (isInitial && SurveyId.HasValue)
            {
                return new[] { (SurveyId.Value) };
            }

            var result = GetSurveysSelectedByUser();

            if (result == null)
            {
                allSelected = true;

                result = SurveyManager.GetSurveys(User.Name, String.Empty).Select(x => x.Id);
            }

            return result;
        }

        private string GetSelectedSurveysNames()
        {
            if (SelectedSurveys.Count == 1)
            {
                return SurveyService.GetFormattedSurveyName(SelectedSurveys.First());
            }

            var names = SurveyManager.GetSurveys(User.Name, string.Empty).Where(x => SelectedSurveys.Contains(x.Id)).
                                                                          Select(x => x.Name).Distinct().Take(MaxNamesCount);

            return ReportTools.MakeArrayStringEx(names, MaxLineLength, 2);
        }
    }
}