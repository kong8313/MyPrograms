using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.PersonImport;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Persons.Import;
using Confirmit.CATI.Supervisor.ServerControls;
using Strings = Confirmit.CATI.Supervisor.Resources.Strings;

namespace Confirmit.CATI.Supervisor.Persons
{
    public partial class Import : BaseForm
    {
        private readonly ISupervisorServiceClient _supervisorService;

        public Import()
        {
            _supervisorService = ServiceLocator.Resolve<ISupervisorServiceClient>();
        }

        private ImportOptions ImportOptions
        {
            get
            {
                return new ImportOptions
                {
                    ImportFirstRow = ImportFirstRow.Checked,
                    OverwriteExistentRelations = OverrideDataAndMembership.Checked,
                    OverwriteExistentData = OverrideDataAndMembership.Checked
                };
            }
        }

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new LightSessionPageStatePersister(this);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                divPersonImportHelp.InnerHtml = Strings.PersonImportHelp;
            }

            switch (Mode.Value)
            {
                case "0":
                    dialogControl.OKButton.Click += LoadFile;
                    ReportCheckBox.Text = Strings.ShowReport;
                    uploadLabel.Text = Strings.XLSFileToUpload;
                    break;
                case "1":
                    dialogControl.OKButton.Click += ParseData;
                    // otherwise animation won't be stopped if user downloads report
                    ShowPostbackProcessingAnimation = false;
                    break;
                default:
                    SaveReport();
                    break;
            }
        }

        protected void SaveReport()
        {
            Response.Clear();
            Response.ClearHeaders();
            Response.ContentType = "text/plain";
            Response.AddHeader(
                "content-disposition",
                "attachment;filename=report.txt");
            Response.Write(DateTime.Now.ToShortDateString() + Environment.NewLine);
            Response.Write(ViewState["log"]);
            Response.End();
        }

        protected void LoadFile(object sender, EventArgs e)
        {
            if (FileBox.PostedFile == null || FileBox.PostedFile.InputStream == null || FileBox.PostedFile.InputStream.Length < 1)
            {
                AddUserMessage("FileNotFound");
                return;
            }
            DataTable dt;
            try
            {
                dt = ExcelDataProvider.GetXlsData(FileBox.PostedFile.InputStream);
            }
            catch (ErrorReadingXlsFileException ex)
            {
                AddUserMessage("ErrorReadingXlsFile", ex);
                return;
            }
            catch (EmptyXlsSheetException ex)
            {
                AddUserMessage("EmptyXlsSheet", ex);
                return;
            }
            ViewState["Data"] = dt;

            BindGrid();
            ImportFirstRow.Text = Strings.ImportFirstRow;
            ImportFirstRow.Visible = true;
            OverrideDataAndMembership.Text = Strings.OverwriteExistentData;
            OverrideDataAndMembership.Visible = true;
            Upload.Visible = false;
            Mode.Value = "1";
        }

        protected void ParseData(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(ColRoles.Value))
            {
                BindGrid();
                AddUserMessage("ColumnRolesNotSpecified");
                return;
            }

            ImportResult totalResult = ImportInterviewers();
            if (totalResult == null)
            {
                BindGrid();
                return;
            }

            dialogControl.OKButton.Text = Strings.SaveToFile;
            dialogControl.CancelButton.Title = Strings.Dlg_Close;

            totalResult.Log = String.Format("<b>Import options</b><br/>{0} = {1}<br/>{2} = {3}<br/><hr/>",
                                            ImportFirstRow.Text, ImportFirstRow.Checked,
                                            OverrideDataAndMembership.Text, OverrideDataAndMembership.Checked) +

                              String.Format("<b>Import result</b><br/>{0} = {1}<br/>{2} = {3}<br/>{4} = {5}<br/>{6} = {7}<br/>{8} = {9}<br/><hr/>",
                                            "Rows processed", totalResult.RowsProcessed,
                                            "Groups created", totalResult.GroupsCreated,
                                            "Users created", totalResult.PersonsCreated,
                                            "Automatic survey set", totalResult.AutomaticSurveySet,
                                            "Automatic survey reset", totalResult.AutomaticSurveyReset) +

                              totalResult.Log;

            ViewState["log"] = totalResult.Log
                .Replace("<b>", String.Empty)
                .Replace("</b>", String.Empty)
                .Replace("<br/>", Environment.NewLine)
                .Replace("<hr/>", Environment.NewLine)
                .Replace("<hr>", Environment.NewLine)
                .Replace("<span style=\"color:red;\">", String.Empty)
                .Replace("</span>", String.Empty);

            Mode.Value = "2";
            gridContainer.Visible = false;
            ImportFirstRow.Visible = false;
            OverrideDataAndMembership.Visible = false;
            dialogControl.CancelButton.InnerText = "Close";
            dialogControl.CancelButton.Attributes["onclick"] = "top.overlay.closeLast(true);";
            RefreshLeftFrame();

            if (ReportCheckBox.Checked)
            {
                divReport.Visible = true;
                Report.Text = totalResult.Log;
            }
            else
            {
                CloseOverlay(true);
            }
        }

        private void BindGrid()
        {
            gridContainer.Visible = true;
            RegisterStartupScript(m_grid.ClientControllerName + ".InitializedEvent.on(grid_Initialize);");
            RegisterStartupScript(m_grid.ClientControllerName + ".BeforeContextMenu.on(grid_ContextMenu);");

            var data = (DataTable)ViewState["Data"];

            foreach (DataColumn column in data.Columns)
            {
                m_grid.Columns.Add(
                    new GeneralGridColumn
                    {
                        HeaderText = column.Caption,
                        DataFieldName = column.ColumnName,
                        Key = column.ColumnName,
                        SearchColumnType = SearchColumnType.None
                    });
            }

            m_grid.RefreshColumns();
            var menuItemCollection = m_grid.DataMenuItems.FindDataMenuItemByKey("ColumnRole").Items;
            menuItemCollection.Clear();
            foreach (ColumnRole role in Enum.GetValues(typeof(ColumnRole)).Cast<ColumnRole>())
            {
                menuItemCollection.Add(new DataMenuItem
                    {
                        TextId = role.ToString(),
                        NavigateUrl =
                            string.Format("javascript:setRole('{0}','{1}')", (int)role, GetResString(role.ToString()))
                    });
            }

            menuItemCollection.Add(new DataMenuItem
            {
                Text = Strings.RemoveColumnAssignment,
                NavigateUrl = "javascript:setRole('','&nbsp;')"
            });

            m_grid.GetPage += delegate(out int count)
            {
                count = data.Rows.Count;
                return data;
            };

            m_grid.DataBind();
        }

        private ImportResult ImportInterviewers()
        {
            ImportResult importResult = null;

            Dictionary<string, ColumnRole> columnRoleMap = GetRolesFromStringProvider.GetColumnNameToRoleMap(ColRoles.Value);

            try
            {
                var evt = new ImportInterviewersEvent(new InterviewerImportDetails[0]);
                var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();
                importResult = _supervisorService.ImportPersons(callCenterId, (DataTable)ViewState["Data"], columnRoleMap, ImportOptions);

                evt.Details.Interviewers = importResult.Interviewers.ToArray();
                evt.Finish();

            }
            catch (RolesRequiredException ex)
            {
                AddUserMessage("RequiredRolesNotSpecified", ex);
            }

            return importResult;
        }
    }
}