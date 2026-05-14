using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Supervisor.Resources;
using CheckBox = System.Web.UI.WebControls.CheckBox;
using DateDataMode = Infragistics.Web.UI.EditorControls.DateDataMode;
using DropDownList = System.Web.UI.WebControls.DropDownList;
using TextBox = Confirmit.CATI.Supervisor.ServerControls.TextBox;

namespace Confirmit.CATI.Supervisor.Controls.Grid
{
    /// <summary>
    /// Helps manage templated header in the General grid
    /// </summary>
    public static class TemplatedHeaderHelper
    {
        #region Members

        private const string m_ValueControlName = "ValueControl";
        private const string m_OperatorControlName = "OperatorControl";
        private const string m_CheckAllControlName = "CheckAllControl";

        #endregion

        #region Properties

        /// <summary>
        /// Gets name for Value control
        /// </summary>
        public static string ValueControlName
        {
            get
            {
                return m_ValueControlName;
            }
        }

        /// <summary>
        /// Gets name for operator dropdown control
        /// </summary>
        public static string OperatorControlName
        {
            get
            {
                return m_OperatorControlName;
            }
        }

        /// <summary>
        /// Gets name for "Check all" checkbox control
        /// </summary>
        public static string CheckAllControlName
        {
            get
            {
                return m_CheckAllControlName;
            }
        }

        #endregion

        public static WebControl GetValidatorControl(ValidationDataType dataType)
        {
            return GetValidatorControl(dataType, int.MaxValue, int.MinValue);
        }

        /// <summary>
        /// Returns validator web control
        /// Used to validate input in the numeric fields
        /// </summary>     
        public static WebControl GetValidatorControl(ValidationDataType dataType, int maxValue, int minValue)
        {
            var rangeValidator = new RangeValidator
                {
                    ControlToValidate = m_ValueControlName,
                    Type = dataType,
                    EnableClientScript = true,
                    ErrorMessage = Strings.ErrorIncorrectValue,
                    SetFocusOnError = true,
                    Display = ValidatorDisplay.None,
                    MaximumValue = maxValue.ToString(),
                    MinimumValue = minValue.ToString()
                };
            return rangeValidator;
        }

        /// <summary>
        /// Returns text box control for the number
        /// </summary>
        /// <param name="gridClientController">Grid client controller name.</param>
        /// <param name="allowDecimal">True to allow user enter ',' for decimal values</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static WebControl GetNumberValueControl(string gridClientController, bool allowDecimal, string defaultValue)
        {
            return GetValueControl(gridClientController, true, allowDecimal, defaultValue);
        }

        /// <summary>
        /// Returns text box control with default value.
        /// </summary>
        /// <param name="gridClienId">Grid client identifier.</param>
        /// <param name="defaultValue">Control default value.</param>
        /// <returns></returns>
        public static WebControl GetValueControl(string gridClienId, string defaultValue)
        {
            return GetValueControl(gridClienId, false, false, defaultValue);
        }

        /// <summary>
        /// Returns text box control with default value.
        /// </summary>
        /// <param name="gridClientController">Grid client controller name.</param>
        /// <param name="onlyDigits">True to allow user enter only digits</param>
        /// <param name="allowDecimal">True to allow user enter ',' for decimal values</param>
        /// <param name="defaultValue">Control default value.</param>
        /// <returns></returns>
        private static WebControl GetValueControl(string gridClientController, bool onlyDigits, bool allowDecimal, string defaultValue)
        {
            TextBox ctrText = new TextBox { CssClass = "search_textbox" };

            ctrText.ID = ValueControlName;
            ctrText.Width = new Unit("100%");
            ctrText.Attributes["onkeypress"] = string.Format("{0}(event,{1},{2}, this);",
                                                             GetKeyDownFunctionName(gridClientController),
                                                             onlyDigits.ToString().ToLower(),
                                                             allowDecimal.ToString().ToLower());
            if (!String.IsNullOrEmpty(defaultValue))
            {
                ctrText.Text = defaultValue;
            }

            return ctrText;
        }

        /// <summary>
        /// Returns drop-down controls with default value set.
        /// </summary>
        public static WebControl GetDropDownControl(string gridClientController, List<ListItem> items, string defaultValue)
        {
            DropDownList ctrDropDownControl = new DropDownList();
            ctrDropDownControl.Attributes["class"] = "search_dropdown";

            ctrDropDownControl.ID = "ValueControl";
            ctrDropDownControl.Width = new Unit("100%");
            ctrDropDownControl.Attributes["OnKeyDown"] = string.Format("{0}(event);", GetKeyDownFunctionName(gridClientController));
            ctrDropDownControl.AutoPostBack = true;

            ctrDropDownControl.Items.Add(String.Empty);
            if (!String.IsNullOrEmpty(defaultValue))
            {
                foreach (ListItem item in items)
                {
                    item.Selected = (item.Value == defaultValue);
                    ctrDropDownControl.Items.Add(item);
                }
            }
            else
            {
                ctrDropDownControl.Items.AddRange(items.ToArray());
            }

            return ctrDropDownControl;

        }

        /// <summary>
        /// Returns drop-down control filled with condition items
        /// </summary>        
        public static Control GetOperatorControl(string gridClientController, SearchOperator defaultValue)
        {
            DropDownList ctrOperator = new DropDownList();
            ctrOperator.Attributes["class"] = "search_dropdown";
            ctrOperator.Attributes["OnKeyDown"] = string.Format("{0}(event);", GetKeyDownFunctionName(gridClientController));

            ctrOperator.ID = OperatorControlName;
            ctrOperator.Items.Add(new ListItem("=", Convert.ToString((int)SearchOperator.Equal, CultureInfo.InvariantCulture)));
            ctrOperator.Items.Add(new ListItem("<", Convert.ToString((int)SearchOperator.Less, CultureInfo.InvariantCulture)));
            ctrOperator.Items.Add(new ListItem(">", Convert.ToString((int)SearchOperator.Greater, CultureInfo.InvariantCulture)));
            ctrOperator.Items.Add(new ListItem("<=", Convert.ToString((int)SearchOperator.LessThanOrEqual, CultureInfo.InvariantCulture)));
            ctrOperator.Items.Add(new ListItem(">=", Convert.ToString((int)SearchOperator.GreaterThanOrEqual, CultureInfo.InvariantCulture)));
            ctrOperator.Items.Add(new ListItem("<>", Convert.ToString((int)SearchOperator.NotEqual, CultureInfo.InvariantCulture)));


            ctrOperator.Items.FindByValue(Convert.ToString((int)defaultValue, CultureInfo.InvariantCulture)).Selected = true;

            var div = BeautifyHeaderControl(ctrOperator);

            return div;

        }

        /// <summary>
        /// Returns calendar control
        /// </summary>   
        public static WebControl GetCalendarControl(string gridClientController, string defaultValue)
        {
            Panel panel = new Panel();
            panel.Wrap = false;

            var ctrCalendar = new DatePicker
            {
                ID = ValueControlName,
                Width = new Unit("100%"),
                NullText = String.Empty,
                MinValue = new DateTime(1900, 1, 1),
            };

            if (string.IsNullOrEmpty(defaultValue) == false)
            {
                ctrCalendar.Value = DateTime.Parse(defaultValue);
            }

            ctrCalendar.Attributes["OnKeyDown"] = string.Format("{0}(event);", GetKeyDownFunctionName(gridClientController));
            panel.Controls.Add(ctrCalendar);

            return panel;
        }

        /// <summary>
        /// Returns time span control.
        /// </summary>
        /// <param name="gridClientController">Grid's client controller name.</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static WebControl GetTimeSpanControl(string gridClientController, string defaultValue)
        {
            var timeSpan = new DateTimeEditor
            {
                ID = ValueControlName,
                Height = new Unit("16px"),
                Width = new Unit("100%"),
                EditModeFormat = "d.HH:mm:ss",
                DataMode = DateDataMode.Text,
                EnableViewState = true,
                UseLastGoodDate = false,
                PromptChar = '0',
                MinimumNumberOfValidFields = 1
            };

            if (string.IsNullOrEmpty(defaultValue) == false)
            {
                timeSpan.Value = DateTime.Parse(defaultValue);
            }

            timeSpan.Attributes["OnKeyDown"] = string.Format("{0}(event);", GetKeyDownFunctionName(gridClientController));

            return timeSpan;
        }

        /// <summary>
        /// Gets Checkbox control for instatiation into header of selection column
        /// </summary>
        public static IEnumerable<WebControl> GetSelectionControl(string gridClientControllerName, bool hasSearchableToolbar)
        {
            var checkBox = new CheckBox { ID = CheckAllControlName };
            var checkBoxControl =new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            checkBoxControl.Attributes["class"] = "checkbox-selector-wrapper";
            var prettier =new System.Web.UI.HtmlControls.HtmlGenericControl("span");
            prettier.Attributes["class"] = "checkbox-prettier";
            checkBoxControl.Controls.Add(checkBox);
            checkBoxControl.Controls.Add(prettier);

            checkBox.Attributes["onclick"] = gridClientControllerName + ".onSelectAllClick(this.checked)";
            checkBox.InputAttributes["class"] = "SelectAll";
            var labelPanel = new Panel { Width = new Unit("100%"), CssClass = "gridHeaderLabel" };
            labelPanel.Style[HtmlTextWriterStyle.Cursor] = "default";
            labelPanel.Style["border-left-width"] = "0px";
            var innerPanel = new Panel();

            labelPanel.Controls.Add(innerPanel);

            if (hasSearchableToolbar)
            {
                yield return labelPanel;

                var panel = new Panel { CssClass = "gridHeaderFilter" };

                panel.Controls.Add(checkBoxControl);

                yield return panel;
            }
            else
            {
                innerPanel.Controls.Add(checkBoxControl);
                innerPanel.Style[HtmlTextWriterStyle.Padding] = "0px 1px";
                yield return labelPanel;
            }
        }

        /// <summary>
        /// Returns name for javascript KeyDown event handling function.
        /// </summary>
        public static string GetKeyDownFunctionName(string gridClientController)
        {
            return gridClientController + ".onSearchControlKeyDown";
        }

        /// <summary>
        /// Returns list of predefined date periods menu items,
        /// </summary>
        /// <returns>Collection of ListItem's.</returns>
        public static List<ListItem> GetPredefinedDateList()
        {
            List<ListItem> result = new List<ListItem>();

            result.Add(new ListItem(Strings.SearchDatePeriodToday, SearchPredefinedDate.Today.ToString()));
            result.Add(new ListItem(Strings.SearchDatePeriodLastTwoDays, SearchPredefinedDate.LastTwoDays.ToString()));
            result.Add(new ListItem(Strings.Yesterday, SearchPredefinedDate.TodayMinus1.ToString()));
            result.Add(new ListItem("2 " + Strings.DaysAgo, SearchPredefinedDate.TodayMinus2.ToString()));
            result.Add(new ListItem("3 " + Strings.DaysAgo, SearchPredefinedDate.TodayMinus3.ToString()));
            result.Add(new ListItem("4 " + Strings.DaysAgo, SearchPredefinedDate.TodayMinus4.ToString()));
            result.Add(new ListItem("5 " + Strings.DaysAgo, SearchPredefinedDate.TodayMinus5.ToString()));
            result.Add(new ListItem("6 " + Strings.DaysAgo, SearchPredefinedDate.TodayMinus6.ToString()));
            result.Add(new ListItem("7 " + Strings.DaysAgo, SearchPredefinedDate.TodayMinus7.ToString()));
            result.Add(new ListItem(Strings.SearchDatePeriodLastWeek, SearchPredefinedDate.ThisWeek.ToString()));
            result.Add(new ListItem(Strings.SearchDatePeriodLastMonth, SearchPredefinedDate.ThisMonth.ToString()));
            result.Add(new ListItem(Strings.SearchDatePeriodLastThreeMonths, SearchPredefinedDate.LastThreeMonths.ToString()));
            result.Add(new ListItem(Strings.SearchDatePeriodLastSixMonths, SearchPredefinedDate.LastSixMonths.ToString()));
            result.Add(new ListItem(Strings.SearchDatePeriodOneYear, SearchPredefinedDate.ThisYear.ToString()));

            return result;
        }

        public static HtmlGenericControl BeautifyHeaderControl(Control control)
        {
            var div = new HtmlGenericControl("DIV");
            var controlName = control.GetType().Name;
            div.Attributes["class"] = $"header-control--{controlName}";
            div.Controls.Add(control);

            if (controlName == "DropDownList")
            {
                div.Controls.Add(new HtmlGenericControl()
                {
                    InnerHtml =
                        "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path d=\"M0 6l12 12L24 6z\"></path></svg>"
                });
            }

            return div;
        }
    }
}
