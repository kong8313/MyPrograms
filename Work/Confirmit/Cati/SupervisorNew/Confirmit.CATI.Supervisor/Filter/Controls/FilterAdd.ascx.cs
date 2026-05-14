using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Classes.Filters;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Core.ActivityLogging;

namespace Confirmit.CATI.Supervisor.Filter.Controls
{
    /// <summary>
    ///		Summary description for FilterAdd.
    /// </summary>
    public partial class FilterAdd : BaseWUC
    {
        private readonly ICachedLocalTimezoneManager _timezoneProvider =
            ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        public event EventHandler FilterSaved;

        protected virtual void OnFilterSave()
        {
            if (FilterSaved != null)
            {
                FilterSaved(this, EventArgs.Empty);
            }
        }

        [StoreInViewState]
        public int SurveyID;

        [StoreInViewState] 
        public int FilterID;

        private ISupervisorFilterFactory _filterFactory;

        private ISupervisorFilterFactory FilterFactory
        {
            get { return _filterFactory ?? (_filterFactory = ServiceLocator.Resolve<ISupervisorFilterFactory>()); }
        }

        public static Dictionary<int,string> Signs
        {
            get
            {
                return
                    Enum.GetValues(typeof (FilterOperator)).Cast<FilterOperator>().ToDictionary(
                        x => (int) x,
                        y => y.Description());
            }
        }

        //---------------------------------------------------------------------------
        public static Dictionary<int, int[]> Operations
        {
            get
            {
                return new Dictionary<int,int[]>
                           {
                               {
                                   (int) VariableTypes.Integer,
                                   new[]
                                       {
                                           (int) FilterOperator.Less,
                                           (int) FilterOperator.Bigger,
                                           (int) FilterOperator.Equal,
                                           (int) FilterOperator.NotEqual
                                       }
                                   },
                               {
                                   (int) VariableTypes.String,
                                   new[]
                                       {
                                           (int) FilterOperator.Like,
                                           (int) FilterOperator.Less,
                                           (int) FilterOperator.Bigger,
                                           (int) FilterOperator.Equal,
                                           (int) FilterOperator.NotEqual
                                       }
                                   },
                               {
                                   (int) VariableTypes.Date,
                                   new[]
                                       {
                                           (int) FilterOperator.Less,
                                           (int) FilterOperator.Bigger,
                                           (int) FilterOperator.Equal,
                                           (int) FilterOperator.NotEqual
                                       }
                                   },
                               {
                                   (int) VariableTypes.Decimal,
                                   new[]
                                       {
                                           (int) FilterOperator.Less,
                                           (int) FilterOperator.Bigger,
                                           (int) FilterOperator.Equal,
                                           (int) FilterOperator.NotEqual
                                       }
                                   },
                               {
                                   (int) VariableTypes.PredefinedValue,
                                   new[]
                                       {
                                           (int) FilterOperator.Equal,
                                           (int) FilterOperator.NotEqual
                                       }
                               },
                               {(int) VariableTypes.Subfilter, null}
                           };
            }
        }

        /// <summary>
        /// Gets or sets the current filter in development.
        /// Used to store filter fields between postbacks.
        /// </summary>
        public List<BvFilterFieldsEntity> CurrentFilterFields
        {
            get
            {
                return (List<BvFilterFieldsEntity>)ViewState["Filters"] ?? new List<BvFilterFieldsEntity>();
            }
            set
            {
                ViewState["Filters"] = value;
            }
        }

        //---------------------------------------------------------------------------
        private string GetImageByTableType(TableTypes tteValue)
        {
            switch (tteValue)
            {
                case TableTypes.Call:
                    return ("svgimages/call.svg");
                case TableTypes.ShiftType:
                    return ("svgimages/call.svg");
                case TableTypes.Resource:
                    return ("svgimages/call.svg");
                case TableTypes.Interview:
                    return ("svgimages/receipt.svg");
                case TableTypes.QSLVariables:
                    return ("svgimages/data_usage.svg");
                case TableTypes.Quotas:
                    return ("svgimages/quota_name.svg");
                case TableTypes.Appointment:
                    return ("svgimages/time.svg");
                case TableTypes.Subfilter:
                    return ("svgimages/filter_list.svg");
                case TableTypes.CFVariables:
                    return ("svgimages/question_answer.svg");
                default:
                    return ("svgimages/receipt.svg");
            }
        }

        //---------------------------------------------------------------------------
        protected void Page_Init(object sender, EventArgs e)
        {
            this.Page.ClientScript.RegisterClientScriptBlock(
                this.GetType(),
                "ClientID",
                String.Format("<script>var ClientID = \"{0}\";</script>", this.ClientID));
        }

        //---------------------------------------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlITS.DataSource = SurveyService.GetTransientStates(SurveyID);
                ddlITS.DataValueField = "StateID";
                ddlITS.DataTextField = "Name";
                ddlITS.DataBind();

                ddlState.DataSource = GetStateListDataSource();
                ddlState.DataValueField = "Value";
                ddlState.DataTextField = "Text";
                ddlState.DataBind();
                
                ddlReviewStatus.DataSource = new[] {
                    new ListItem(Strings.NotSentToReview, "0"),
                    new ListItem(Strings.SentToReview, "1"),
                    new ListItem(Strings.SessionReviewStarted, "2"),
                    new ListItem(Strings.SessionReviewCompleted, "3")
                };
                ddlReviewStatus.DataValueField = "Value";
                ddlReviewStatus.DataTextField = "Text";
                ddlReviewStatus.DataBind();

                rblOperator.Items.Clear();
                rblOperator.Items.AddRange(
                    new[]
                        {
                            new ListItem(Strings.And, ((int)(AndOrOperator.And)).ToString()),
                            new ListItem(Strings.Or, ((int)(AndOrOperator.Or)).ToString())
                        });
                rblOperator.Items[0].Selected = true;
                if (FilterID != Int32.MinValue)
                {
                    BvFiltersEntity filter = FilterRepository.GetById(FilterID);
                    EdtName.Text = filter.Name;
                    EdtDescription.Text = filter.Description;
                }

                variablesTree.SurveySid = SurveyID;
                variablesTree.FilterId = FilterID == Int32.MinValue ? (int?) null : FilterID;
            }
            LoadFilter(variablesTree.TreeItems);

            dialogControl.OKButton.Text = FilterID == Int32.MinValue ? "Create filter" : "Save";

            RegisterScripts();
        }

        //---------------------------------------------------------------------------
        private void LoadFilter(IEnumerable<VariableInfo> itemsList)
        {
            var items = itemsList.ToDictionary(var => ((int) var.TableType).ToString() + "_" + var.Column);

            try
            {
                var filterFields = new List<BvFilterFieldsEntity>();
                if (!IsPostBack)
                {
                    if (FilterID != Int32.MinValue)
                    {
                        BvFiltersEntity filter = FilterRepository.GetById(FilterID);
                        string sCriteria = filter.AndOrOperator.ToString();
                        rblOperator.Items.FindByValue(sCriteria).Selected = true;

                        filterFields = FilterService.GetFields(FilterID);
                    }
                }
                else
                {
                    filterFields = CurrentFilterFields;
                }

                for (int i = TblFields.Rows.Count - 1; i >= 1; i--)
                {
                    TblFields.Rows.RemoveAt(i);
                }

                bool unexpectedVariables = false;
                var validator = ServiceLocator.Resolve<IFilterFieldValidator>();

                foreach (BvFilterFieldsEntity filterField in filterFields)
                {
                    try
                    {
                        validator.Validate(filterField);
                    }
                    catch (Exception ex)
                    {
                        ShowClientMessage(string.Format(Strings.InvalidFieldWhileLoadingFilter, ex.Message));
                        continue;
                    }

                    int tableType = filterField.Table;
                    string column = filterField.Column;
                    int varType = filterField.Type;
                    int sign = filterField.Sign;
                    string value = filterField.Value;

                    //convert datetime from utc to site time zone
                    if ((VariableTypes)varType == VariableTypes.Date)
                    {
                        var date = DateTime.Parse(value);
                        value = _timezoneProvider.ConvertToLocalTime(date).ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    var columnField = (VariableTypes)varType != VariableTypes.Subfilter ? column : value;
                    string key = String.Format("{0}_{1}", tableType, columnField);

                    if (!items.ContainsKey(key))
                    {
                        unexpectedVariables = true;
                        continue;
                    }

                    HtmlTableRow row = new HtmlTableRow();
                    row.Attributes.Add("TableType", tableType.ToString());
                    row.Attributes.Add("Column", column);
                    row.Attributes.Add("VarType", varType.ToString());
                    row.Attributes.Add("OnClick", "SelectVariable(Y.Event.getEvent(event))");
                    row.Attributes.Add("IsBackground", items[key].IsBackground.ToString());
                    row.Attributes.Add("VarTitle", items[key].Name);
                    TblFields.Rows.Add(row);

                    var cell = new HtmlTableCell { Width = "130" };

                    cell.InnerHtml = String.Format(
                        @"<nobr><img src='{1}' align='absmiddle'>&nbsp;{0}</nobr>",
                        (items[key]).Name,
                        BaseRelativePath(GetImageByTableType((TableTypes)tableType)));
                    row.Cells.Add(cell);

                    int[] signs;
                    HtmlSelect slt;
                    HtmlInputText txt;
                    HtmlInputButton btn;
                    HtmlInputHidden hdn;
                    HtmlGenericControl selectWrapper;

                    switch ((VariableTypes)varType)
                    {
                        case VariableTypes.Date:
                            cell = new HtmlTableCell();
                            cell.Width = "60";
                            selectWrapper = new HtmlGenericControl("div");
                            selectWrapper.Attributes["class"] = "dropdown-control";
                            cell.Controls.Add(selectWrapper);
                            slt = new HtmlSelect();
                            slt.Style.Add("width", "100%");
                            slt.Attributes.Add("class", "plain_dropdown");
                            signs = Operations[varType];
                            for (int j = 0; j < signs.GetLength(0); j++)
                            {
                                slt.Items.Add(
                                    new ListItem(
                                        Signs[signs[j]],
                                        signs[j].ToString()));
                            }
                            slt.Items.FindByValue(sign.ToString()).Selected = true;

                            selectWrapper.Controls.Add(slt);
                            row.Cells.Add(cell);

                            cell = new HtmlTableCell();
                            cell.Width = "100%";

                            hdn = new HtmlInputHidden();
                            string strg = value;
                            hdn.Value = strg;
                            cell.Controls.Add(hdn);

                            txt = new HtmlInputText();
                            txt.Attributes.Add("readonly", "");
                            txt.Style.Add("width", "100%");
                            txt.Attributes.Add("class", "plain_textbox");
                            if (strg.Trim().Length == 19)
                            {
                                int year = Int32.Parse(strg.Substring(0, 4));
                                int month = Int32.Parse(strg.Substring(5, 2));
                                int day = Int32.Parse(strg.Substring(8, 2));
                                int hour = Int32.Parse(strg.Substring(11, 2));
                                int minute = Int32.Parse(strg.Substring(14, 2));
                                int second = Int32.Parse(strg.Substring(17, 2));
                                DateTime date = new DateTime(year, month, day, hour, minute, second);
                                txt.Value = date.ToString();
                            }
                            else
                                txt.Value = "";

                            cell.Controls.Add(txt);
                            row.Cells.Add(cell);

                            cell = new HtmlTableCell();
                            cell.Width = "35";
                            btn = new HtmlInputButton();
                            btn.Style.Add("width", "100%");
                            btn.Attributes.Add("class", "plain_button");
                            btn.Value = ">>";
                            btn.Attributes.Add("onclick", "ShowCalendar(event)");
                            cell.Controls.Add(btn);
                            row.Cells.Add(cell);
                            break;

                        case VariableTypes.Subfilter:
                            cell = new HtmlTableCell();
                            cell.ColSpan = 1;
                            hdn = new HtmlInputHidden();
                            hdn.Value = ((int)FilterOperator.Subfilter).ToString();
                            cell.Controls.Add(hdn);
                            row.Cells.Add(cell);

                            cell = new HtmlTableCell();
                            hdn = new HtmlInputHidden();
                            hdn.Value = value;
                            cell.Controls.Add(hdn);
                            cell.ColSpan = 2;
                            row.Cells.Add(cell);

                            break;

                        default:
                            cell = new HtmlTableCell();
                            cell.Width = "60";
                            selectWrapper = new HtmlGenericControl("div");
                            selectWrapper.Attributes["class"] = "dropdown-control";
                            cell.Controls.Add(selectWrapper);
                            slt = new HtmlSelect();
                            slt.Style.Add("width", "100%");
                            slt.Attributes.Add("class", "plain_dropdown");
                            if (tableType == (int)TableTypes.ShiftType && column == "Name")
                            {
                                slt.Items.Add(
                                    new ListItem(
                                        Signs[(int)FilterOperator.Equal],
                                        ((int)FilterOperator.Equal).ToString()));
                            }
                            else
                            {
                                signs = Operations[varType];
                                for (int j = 0; j < signs.GetLength(0); j++)
                                {
                                    slt.Items.Add(
                                        new ListItem(
                                            Signs[signs[j]],
                                            signs[j].ToString()));
                                }
                            }

                            slt.Items.FindByValue(sign.ToString()).Selected = true;
                            selectWrapper.Controls.Add(slt);
                            row.Cells.Add(cell);

                            if (column == "TransientState")
                            {
                                cell = GenerateTableCellWithDropDown(SurveyService.GetTransientStates(SurveyID), value, "StateID", "Name");
                                row.Cells.Add(cell);
                            }
                            else if(column == "CallState")
                            {
                                cell = GenerateTableCellWithDropDown(GetStateListDataSource(), value, "Value", "Text" );
                                row.Cells.Add(cell);
                            }
                            else if (column == "ReviewStatus")
                            {
                                cell = GenerateTableCellWithDropDown(new[]
                                                                        {
                                                                            new ListItem(Strings.NotSentToReview, ""),
                                                                            new ListItem(Strings.SentToReview, "0"),
                                                                            new ListItem(Strings.SessionReviewStarted, "1"),
                                                                            new ListItem(Strings.SessionReviewCompleted, "2")
                                                                        }, value, "Value", "Text");
                                row.Cells.Add(cell);
                            }

                            cell = new HtmlTableCell();
                            cell.Width = "100%";
                            cell.ColSpan = 2;
                            txt = new HtmlInputText();
                            txt.Style.Add("width", "100%");
                            txt.Attributes.Add("class", "plain_textbox");
                            txt.Value = value;
                            if (column == "TransientState")
                            {
                                txt.Style.Add("display", "none");
                            }
                            cell.Controls.Add(txt);
                            row.Cells.Add(cell);
                            break;
                    }
                }
                if (unexpectedVariables)
                {
                    Page.AddUserMessage(Strings.VariablesCouldNotBeFoundInCurrentSurvey);
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
                Page.CloseOverlay();
            }
        }

        private static ListItem[] GetStateListDataSource()
        {
            return new[]
                       {
                           new ListItem(Strings.EnabledStateString, "2"),
                           new ListItem(Strings.DisabledByFCDStateString, "1"),
                           new ListItem(Strings.DisabledByUserStateString, "3")
                       };
        }

        private HtmlTableCell GenerateTableCellWithDropDown(object dataSource, string selectedValue, string dataValueField, string dataTextField)
        {
            var cell = new HtmlTableCell
                           {
                               Width = "100%", ColSpan = 2
                           };

            var selectWrapper = new HtmlGenericControl("div");
            selectWrapper.Attributes["class"] = "dropdown-control";
            cell.Controls.Add(selectWrapper);
            var ddl = new HtmlSelect();
            ddl.Attributes.Add("class", "plain_dropdown");
            ddl.Style.Add("width", "100%");

            ddl.DataSource = dataSource;
            ddl.DataValueField = dataValueField;
            ddl.DataTextField = dataTextField;
            ddl.DataBind();

            ddl.Value = selectedValue;

            selectWrapper.Controls.Add(ddl);
            return cell;
        }

        //---------------------------------------------------------------------------
        protected void BtnSave_ServerClick(object sender, EventArgs e)
        {
            bool bNeedCloseWnd = true;

            try
            {
                using (var transactionScope = new DatabaseTransactionScope("SaveFilter", DeadlockPriority.Supervisor))
                {
                    string filterXml = HdnFields.Value.Replace("&lt;", "<").Replace("&gt;", ">");
                    var filterData = FilterFactory.Create(FilterID, EdtName.Text.Trim(), EdtDescription.Text,
                                                          rblOperator.SelectedValue, filterXml);

                    var filter = filterData.Filter;
                    var fields = filterData.Fields;
                    CurrentFilterFields = fields.ToList();
                    bool onlyThisSrv = fields.Any(x => x.Table == (int) TableTypes.CFVariables);
                    filter.SurveySID = onlyThisSrv ? SurveyID : 0;

                    int surveySid = filter.SurveySID;
                    IEnumerable<int> fieldIds = fields.Select(x => x.ID);
                    IEnumerable<string> columns = fields.Select(x => x.Column);

                    var evt = FilterID == Int32.MinValue
                                  ? (IManagementActivityEvent)
                                    new CreateFilterEvent(FilterID, filter.Name, surveySid, false, fieldIds, columns)
                                  : new UpdateFilterEvent(FilterID, filter.Name, surveySid, false, fieldIds, columns);

                    if (FilterID == Int32.MinValue)
                    {
                        FilterID = FilterRepository.Insert(filter);
                    }
                    else
                    {
                        FilterRepository.Update(filter);
                    }

                    FilterService.SetFields(FilterID, CurrentFilterFields);

                    evt.ObjectId = FilterID;
                    evt.Finish();
                    transactionScope.Commit();
                }
            }
            catch (Exception ex)
            {
                bNeedCloseWnd = false;
                Context.AddError(ex);
                LoadFilter(variablesTree.TreeItems);
            }
            finally
            {
                if (bNeedCloseWnd)
                {
                    OnFilterSave();
                }
            }
        }

        /// <summary>
        /// Registers the client scripts.
        /// </summary>
        private void RegisterScripts()
        {
            string script =
                "var TableTypes = {" +
                    " Subfilter :    " + (int)TableTypes.Subfilter +
                    ",Interview :    " + (int)TableTypes.Interview +
                    ",Call :         " + (int)TableTypes.Call +
                    ",Appointment :  " + (int)TableTypes.Appointment +
                    ",QSLVariables : " + (int)TableTypes.QSLVariables +
                    ",Quotas :       " + (int)TableTypes.Quotas +
                    ",Container :    " + (int)TableTypes.Container +
                    ",ShiftType :    " + (int)TableTypes.ShiftType +
                    ",Resource :     " + (int)TableTypes.Resource +
                    ",Web :          " + (int)TableTypes.Web +
                    ",CFVariables :  " + (int)TableTypes.CFVariables +
                "};";

            Page.RegisterScriptBlock(script);

            script =
                "var VariableTypes = {" +
                    " Subfilter : " + (int)VariableTypes.Subfilter +
                    ",Integer :   " + (int)VariableTypes.Integer +
                    ",String :    " + (int)VariableTypes.String +
                    ",Date :      " + (int)VariableTypes.Date +
                    ",Decimal :   " + (int)VariableTypes.Decimal +
                    ",PredefinedValue : " + (int)VariableTypes.PredefinedValue +
                "};";

            Page.RegisterScriptBlock(script);

            script =
                "var FilterOperator = {" +
                    " Less :        " + (int)FilterOperator.Less +
                    ",Bigger :      " + (int)FilterOperator.Bigger +
                    ",Equal :       " + (int)FilterOperator.Equal +
                    ",LessEqual :   " + (int)FilterOperator.LessEqual +
                    ",BiggerEqual : " + (int)FilterOperator.BiggerEqual +
                    ",NotEqual :    " + (int)FilterOperator.NotEqual +
                    ",Like :        " + (int)FilterOperator.Like +
                    ",Subfilter :   " + (int)FilterOperator.Subfilter +
                "};";

            Page.RegisterScriptBlock(script);

            Page.RegisterStartupScript("InitializeConstants();");
        }
    }
}
