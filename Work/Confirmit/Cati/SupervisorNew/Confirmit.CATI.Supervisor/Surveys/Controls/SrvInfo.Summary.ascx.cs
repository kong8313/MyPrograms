using System;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.ServerControls;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Supervisor.Resources;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.Surveys.Controls
{
    public partial class SrvInfo_Summary : SrvInfoChild
    {
        /// <summary>
        /// Represents single record containing status data.
        /// </summary>
        private class StatusSummaryRecord
        {
            /// <summary>
            /// Gets/sets status identifier.
            /// </summary>
            public int Id
            {
                get;
                set;
            }

            /// <summary>
            /// Gets/sets status name.
            /// </summary>
            public string Name
            {
                get;
                set;
            }

            /// <summary>
            /// Gets/sets number of interviews with current status.
            /// </summary>
            public int TotalCount
            {
                get;
                set;
            }

            public int SampleSize
            {
                get;
                set;
            }

            public int EnabledCallCount
            {
                get;
                set;
            }

            public int FcdDisabledCallCount
            {
                get;
                set;
            }

            public int UserDisabledCallCount
            {
                get;
                set;
            }
            
        }

        /// <summary>
        /// User Selected Filter ID
        /// </summary>
        protected int? FilterID
        {
            get
            {
              return ddlFilter.SelectedIndex != 0 ? new int?(Convert.ToInt32(ddlFilter.SelectedValue)) : null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<BvFiltersEntity> filters = FilterRepository.GetFiltersList(true, Survey.SID);
            ddlFilter.DataTextField = "Name";
            ddlFilter.DataValueField = "SID";
            ddlFilter.DataSource = filters;
            ddlFilter.DataBind();
            ddlFilter.Items.Insert(0, new ListItem("-", "0"));
            gridSummary.InitializeRow += Grid_InitializeRow;
            GridTitle.Text = string.Format(Strings.SrvTabs_SummaryGridTitle, Survey.Description, Survey.Name);
            gridSummary.GetPage +=
                delegate(out int totalCount)
                {
                    var items = new List<StatusSummaryRecord>(
                        from c in SurveyService.GetSampleStatusSummary(Survey.SID, FilterID, cbExludeFreshSampleStatus.Checked ? new []{16} : null)
                        where c.count.HasValue && c.count > 0
                        select new StatusSummaryRecord { Id = c.id.Value, 
                                                         Name = c.name, 
                                                         SampleSize = c.sample_size.Value,
                                                         TotalCount = c.count.Value,
                                                         EnabledCallCount = c.enabled_call.Value,
                                                         FcdDisabledCallCount = c.fcd_disabled_call.Value,
                                                         UserDisabledCallCount = c.user_disabled_call.Value
                        }
                    );

                    return BaseMethods.GetPage(
                        items,
                        new PagingArgs(
                            gridSummary.SortedColumnName,
                            gridSummary.SortIndicatorAsc
                        ),
                        out totalCount
                    );
                };
        }

        private void GridAddPercentToValue(RowEventArgs e, string fieldKey, int value)
        {
            var item = e.Row.Items.FindItemByKey(fieldKey);
            item.Column.Type = typeof(string);
            var data = (StatusSummaryRecord)e.Row.DataItem;

            item.Text = String.Format("{0}   ({1:P2})", value, (float)value / data.SampleSize);
        }
        private void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            var data = (StatusSummaryRecord)e.Row.DataItem;
            GridAddPercentToValue(e, "TotalCount", data.TotalCount);
            GridAddPercentToValue(e, "EnabledCallCount", data.EnabledCallCount);
            GridAddPercentToValue(e, "FcdDisabledCallCount", data.FcdDisabledCallCount);
            GridAddPercentToValue(e, "UserDisabledCallCount", data.UserDisabledCallCount);
        }
    }
}