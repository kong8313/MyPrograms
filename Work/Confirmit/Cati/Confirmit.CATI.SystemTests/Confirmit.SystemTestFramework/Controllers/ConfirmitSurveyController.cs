using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Confirmit.SystemTestFramework.Samples;
using Confirmit.SystemTestFramework.Settings;
using Confirmit.SystemTestFramework.SurveyData;
using Confirmit.SystemTestFramework.SurveyDeployer;
using OfficeOpenXml;
using DatabaseType = Confirmit.SystemTestFramework.SurveyDeployer.DatabaseType;

namespace Confirmit.SystemTestFramework.Controllers
{
    public class ConfirmitSurveyController : TestController
    {
        private readonly string _pid;

        public ConfirmitRespondentsDataController RespondentsData { get; private set; }

        public ConfirmitSurveyController(UserInfo userInfo, string pid)
        {
            UserInfo = userInfo;
            _pid = pid;

            RespondentsData = new ConfirmitRespondentsDataController(userInfo, pid);
        }

        public void Launch()
        {
            var deployer = new SurveyDeployer.SurveyDeployer();

            var taskId = deployer.LaunchSurvey(UserInfo.ClientKey, _pid,
                DatabaseType.Production,
                GenerateDbOptions.CreateNewDatabase,
                GenerateWiOptions.WiNet);

            var taskStatus = deployer.GetTaskStatus(UserInfo.ClientKey, taskId);

            for (int i = 1; i <= 600 && taskStatus != TaskStatus.Complete && taskStatus != TaskStatus.Error; i++)
            {
                Thread.Sleep(500);

                taskStatus = deployer.GetTaskStatus(UserInfo.ClientKey, taskId);
            }

            int completedOn = deployer.GetTaskPercentageCompleted(UserInfo.ClientKey, taskId);

            if (completedOn != 100)
            {
                throw new TimeoutException(
                    "The time allotted for a survey launch has expired or error occurred in launch process");
            }
            if (taskStatus != TaskStatus.Complete)
            {
                throw new Exception("Task status: " + taskStatus);
            }
        }

        public void AddRespondents(string file, CatiScheduling schedulingMode = CatiScheduling.Simple)
        {
            var surveyData = new SurveyData.SurveyData();

            var parser = new SampleParser();

            var dataSet = parser.Parser(file);

            surveyData.UpdateRespondentsWithCatiScheduling(UserInfo.ClientKey, _pid, dataSet, true, false, null, false, -1, schedulingMode);
        }

        public List<string> AddResponseData(string clientKey, string projectId)
        {
            var interviewIds = new List<string>();
            var columnNames = new List<string>();

            var surveyData = new DataSet("DS");
            var dt = GetDataTableFromExcel("_settings.PathToInterviewData", interviewIds, columnNames);

            surveyData.Tables.Add(dt);

            var transferDef = new TransferDef
            {
                ProjectId = projectId,
                AddRespondents = true,
                DbType = SurveyData.DatabaseType.Production,
                Key = "responseid"
            };
            var transferLevel = new TransferLevel
            {
                LoopId = "responseid",
                Fields = columnNames.Where(x => x.StartsWith("q")).ToArray()
            };

            transferDef.Levels = new[] { transferLevel };

            var surveyDataService = new SurveyData.SurveyData();//"_settings.SurveyDataWebService"

            var errors = surveyDataService.UpdateData(clientKey, transferDef, surveyData, false, false, 0);

            if (errors.Length != 0)
            {
                var exceptionMessage = new StringBuilder("Problems with imported data:" + Environment.NewLine);

                for (int i = 0; i < errors.Length; i++)
                {
                    exceptionMessage.AppendFormat("{0}. {1}" + Environment.NewLine, i + 1, errors[i]);
                }

                throw new InvalidDataException(exceptionMessage.ToString());
            }

            return interviewIds;
        }

        private DataTable GetDataTableFromExcel(string path, List<string> interviewIds, List<string> columnNames)
        {
            var file = File.ReadAllBytes(path);

            var ms = new MemoryStream(file);
            using (var pck = new ExcelPackage(ms))
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                var tbl = new DataTable("responseid");
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(firstRowCell.Text);
                    columnNames.Add(firstRowCell.Text);
                }
                var startRow = 2;
                for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    var row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;

                        if (cell.Start.Column == 1)
                            interviewIds.Add(cell.Text);
                    }
                }

                return tbl;
            }
        }
    }
}