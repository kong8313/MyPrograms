using System;
using System.Data;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Surveys;

namespace Confirmit.CATI.Supervisor.Surveys
{
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class SurveyHistoryLoop : BaseForm
    {
        protected int InterviewId
        {
            get { return Convert.ToInt32(ViewState["InterviewID"]); }
            set { ViewState["InterviewID"] = value; }
        }

        private string ProjectId
        {
            get { return ViewState["ProjectId"].ToString(); }
            set { ViewState["ProjectId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InterviewId = Convert.ToInt32(Request["InterviewID"]);
                ProjectId = SurveyRepository.GetById(Convert.ToInt32(Request["ID"])).Name;
            }

            grid.GetPage += GetData;

            toolbar.LeftLabel = "Interview " + InterviewId;
        }

        private DataTable GetData(out int totalCount)
        {
            var result = new DataTable();

            try
            {
                result = CallManager.GetInterviewCallHistoryInfo(ProjectId, InterviewId);
            }
            catch (Exception ex)
            {
                nodata.Visible = true;
                data.Visible = false;

                System.Diagnostics.Trace.TraceWarning(ex.ToString());
            }

            totalCount = result.Rows.Count;
            return result;
        }

        protected void grid_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var dataRow = ((DataRowView) e.Row.DataItem).Row;
                var dataColumns = dataRow.Table.Columns;

                for (int index = 0; index < dataColumns.Count; index++)
                {
                    TableCell cell = e.Row.Cells[index + 1];

                    if (dataColumns[index].DataType == typeof(bool))
                    {
                        if (dataRow.IsNull(index) == false)
                        {
                            cell.Text = (bool) dataRow[index] ? "1" : "0";
                        }
                    }
                    else
                    {
                        cell.Text = cell.Text.Replace("\n", "<br/>");
                    }
                }
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            grid.RefreshData();
        }
    }
}
