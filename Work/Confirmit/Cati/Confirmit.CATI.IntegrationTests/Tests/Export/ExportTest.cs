using System;
using System.Collections.Generic;
using Confirmit.TelephonyProblemStates.ProblemState;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Supervisor.Core.Export;
using DocumentFormat.OpenXml.Packaging;
using System.Xml.Linq;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Supervisor.Core.Export.CollectionProvider.SpecificProvider;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Common;
using System.IO;
using Confirmit.CATI.Supervisor.Core.Export.Tools;
using System.Globalization;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Supervisor.Core.Export.CollectionProvider;
using ConfirmitDialerInterface;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.Supervisor.Core.Activity.Fakes;

namespace Confirmit.CATI.IntegrationTests.Tests.Export
{
    /// <summary>
    /// Summary description for Export
    /// </summary>
    [TestClass]
    public class ExportTest
    {
        private enum TemplateFileType
        {
            ExportInterviewerActivity,
            ExportAppointmentActivity,
            ExportSurveyActivity
        }

        private static string _templateFilePath = String.Empty;
        private static readonly XNamespace Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        private string _testDataFolder;

        [TestInitialize]
        public void TestInitialize()
        {
            _testDataFolder = new Configuration().TestDataPath;
            ServiceLocator.StaticInitialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ServiceLocator.StaticCleanup();

            if (!String.IsNullOrEmpty(_templateFilePath) &&
                   File.Exists(_templateFilePath))
            {
                File.Delete(_templateFilePath);
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void InterviewerActivityExport_CorrectInputData_Success()
        {
            List<TaskActivityInfo> list = new List<TaskActivityInfo>();

            list.Add(GetTaskActivityInfo(1,
                                        "p1234567",
                                         "SurveyName",
                                         InterviewState.INTERVIEWING,
                                         0,
                                         DialingMode.Manual));

            ExportDefinitionData defenitionData = new ExportDefinitionData()
            {
                SheetName = "InterviewerActivity",
                Data = new InterviewerActivityExportProvider(list)
            };

            _templateFilePath = CopyTemplateToTempDir(TemplateFileType.ExportInterviewerActivity);

            ExportManager.ExportUsingTemplate(_templateFilePath, new[] { defenitionData });

            using (SpreadsheetDocument template = SpreadsheetDocument.Open(_templateFilePath, true))
            {
                WorkbookPart workbook = template.WorkbookPart;
                SharedStringTablePart sharedStringsPart = workbook.SharedStringTablePart;
                string[] sharedStrings = OpenXmlHelper.GetSharedStrings(sharedStringsPart, Namespace);

                WorksheetPart worksheet = OpenXmlHelper.GetWorksheetByName(workbook, defenitionData.SheetName);

                XDocument sheet = OpenXmlHelper.GetXmlFromPart(worksheet);

                Assert.AreEqual("1", GetCellValue(sheet, sharedStrings, "A3"));
                Assert.AreEqual("p1234567", GetCellValue(sheet, sharedStrings, "B3"));
                Assert.AreEqual("SurveyName", GetCellValue(sheet, sharedStrings, "C3"));
                Assert.AreEqual(StringHelper.GetStringFromEnum(InterviewState.INTERVIEWING), GetCellValue(sheet, sharedStrings, "K3"));
                Assert.AreEqual(new CatiProblemStateFactory(new CatiProblemStateInfo(string.Empty)).GetState(0).Message, GetCellValue(sheet, sharedStrings, "O3"));
                Assert.AreEqual(StringHelper.GetStringFromEnum(DialingMode.Manual), GetCellValue(sheet, sharedStrings, "N3"));
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void AppointmentActivityExport_CorrectInputData_AppointmentActivityInfo_Success()
        {
            DateTime appointmentTime = DateTime.Now;

            List<AppointmentActivityInfo> list = new List<AppointmentActivityInfo>();
            list.Add(GetAppointmentActivityInfo(1, "p1234567", "SurveyName", appointmentTime));

            ExportDefinitionData defenitionData = new ExportDefinitionData()
            {
                SheetName = "AppointmentActivityInfo",
                Data = new AppointmentActivityExportProvider(list, true, 1)
            };

            _templateFilePath = CopyTemplateToTempDir(TemplateFileType.ExportAppointmentActivity);

            var timezoneServiceStub = new StubITimezoneService();
            timezoneServiceStub.ConvertTimeFromUtcInt32DateTime = (sid, time) => appointmentTime;
            ServiceLocator.RegisterInstance<ITimezoneService>(timezoneServiceStub);

            ExportManager.ExportUsingTemplate(_templateFilePath, new[] { defenitionData });

            using (SpreadsheetDocument template = SpreadsheetDocument.Open(_templateFilePath, true))
            {
                WorkbookPart workbook = template.WorkbookPart;
                SharedStringTablePart sharedStringsPart = workbook.SharedStringTablePart;
                string[] sharedStrings = OpenXmlHelper.GetSharedStrings(sharedStringsPart, Namespace);

                WorksheetPart worksheet = OpenXmlHelper.GetWorksheetByName(workbook, defenitionData.SheetName);

                XDocument sheet = OpenXmlHelper.GetXmlFromPart(worksheet);

                Assert.AreEqual("1", GetCellValue(sheet, sharedStrings, "A2"));
                Assert.AreEqual("p1234567", GetCellValue(sheet, sharedStrings, "B2"));
                Assert.AreEqual("SurveyName", GetCellValue(sheet, sharedStrings, "C2"));
                Assert.AreEqual(appointmentTime.ToOADate(), Double.Parse(GetCellValue(sheet, sharedStrings, "E2"), CultureInfo.InvariantCulture));
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void AppointmentActivityExport_CorrectInputData_AppointmentCountInfo_Success()
        {
            List<SurveyAppointmentCountInfo> list = new List<SurveyAppointmentCountInfo>();

            list.Add(new SurveyAppointmentCountInfo()
            {
                ProjectId = "p1234567",
                ProjectName = "SurveyName",
                ShortIntervalCount = 1,
                LongIntervalCount = 2
            });

            string ShortIntervalText = "1 hour";
            string LongIntervalText = "1 day";

            Dictionary<string, string> additionalParams = new Dictionary<string, string>();
            additionalParams.Add("ShortIntervalCount", ShortIntervalText);
            additionalParams.Add("LongIntervalCount", LongIntervalText);

            ExportDefinitionData defenitionData = new ExportDefinitionData()
            {
                SheetName = "SurveyAppointmentCountInfo",
                Data = new CollectionExportProvider(list, additionalParams)
            };

            _templateFilePath = CopyTemplateToTempDir(TemplateFileType.ExportAppointmentActivity);

            ExportManager.ExportUsingTemplate(_templateFilePath, new[] { defenitionData });

            using (SpreadsheetDocument template = SpreadsheetDocument.Open(_templateFilePath, true))
            {
                WorkbookPart workbook = template.WorkbookPart;
                SharedStringTablePart sharedStringsPart = workbook.SharedStringTablePart;
                string[] sharedStrings = OpenXmlHelper.GetSharedStrings(sharedStringsPart, Namespace);

                WorksheetPart worksheet = OpenXmlHelper.GetWorksheetByName(workbook, defenitionData.SheetName);

                XDocument sheet = OpenXmlHelper.GetXmlFromPart(worksheet);

                Assert.AreEqual("p1234567", GetCellValue(sheet, sharedStrings, "A3"));
                Assert.AreEqual("SurveyName", GetCellValue(sheet, sharedStrings, "B3"));
                Assert.AreEqual("1", GetCellValue(sheet, sharedStrings, "C3"));
                Assert.AreEqual("2", GetCellValue(sheet, sharedStrings, "D3"));
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void SurveyActivityExport_CorrectInputData_Success()
        {
            TimeSpan timeSpent = new TimeSpan(10, 10, 10);
            TimeSpan interviewDuration = new TimeSpan(20, 20, 20);
            DateTime nextAppointment = DateTime.Now;

            List<SurveyActivityInfo> list = new List<SurveyActivityInfo>();

            list.Add(new SurveyActivityInfo()
            {
                // Insert wrong xml char here to test export works with it
                Id = "p1234567\u0001",
                Name = "SurveyName",
                LoggedCount = 1,
                AssignedCount = 2,
                SampleSize = 3,
                TotalTime = timeSpent,
                NextAppointment = nextAppointment,
                ScheduledCallsCount = 5,
                SuspendedCallsCount = 6,
                StrikeRate = 7,
                CountCalls = 8,
                InterviewDuration = interviewDuration
            });

            List<StatusInfo> statusInfos = new List<StatusInfo>();
            statusInfos.Add(new StatusInfo(1, "Appointment", 1, AlertStatus.Ok));
            statusInfos.Add(new StatusInfo(2, "Terminated", 2, AlertStatus.Ok));
            statusInfos.Add(new StatusInfo(3, "Completed", 3, AlertStatus.Ok));

            ExportDefinitionData defenitionData = new ExportDefinitionData()
            {
                SheetName = "SurveyActivity",
                Data = new SurveyActivityExportProvider(list)
            };

            var timezoneManagerStub = new StubIActivityManager { GetStatusBreakdownInt32 = id => statusInfos };
            ServiceLocator.RegisterInstance<IActivityManager>(timezoneManagerStub);

            _templateFilePath = CopyTemplateToTempDir(TemplateFileType.ExportSurveyActivity);

            ExportManager.ExportUsingTemplate(_templateFilePath, new ExportDefinitionData[] { defenitionData });

            using (SpreadsheetDocument template = SpreadsheetDocument.Open(_templateFilePath, true))
            {
                WorkbookPart workbook = template.WorkbookPart;
                SharedStringTablePart sharedStringsPart = workbook.SharedStringTablePart;
                string[] sharedStrings = OpenXmlHelper.GetSharedStrings(sharedStringsPart, Namespace);

                WorksheetPart worksheet = OpenXmlHelper.GetWorksheetByName(workbook, defenitionData.SheetName);

                XDocument sheet = OpenXmlHelper.GetXmlFromPart(worksheet);

                Assert.AreEqual("p1234567", GetCellValue(sheet, sharedStrings, "A3"));
                Assert.AreEqual("SurveyName", GetCellValue(sheet, sharedStrings, "B3"));
                Assert.AreEqual("1", GetCellValue(sheet, sharedStrings, "C3"));
                Assert.AreEqual("2", GetCellValue(sheet, sharedStrings, "D3"));
                Assert.AreEqual(timeSpent, TimeSpan.FromDays(double.Parse(GetCellValue(sheet, sharedStrings, "L3"), CultureInfo.InvariantCulture)));
                Assert.AreEqual(nextAppointment.ToString(),
                                DateTime.FromOADate(double.Parse(GetCellValue(sheet, sharedStrings, "M3"),
                                                    CultureInfo.InvariantCulture)).ToString());
                Assert.AreEqual("5", GetCellValue(sheet, sharedStrings, "N3"));
                Assert.AreEqual("6", GetCellValue(sheet, sharedStrings, "O3"));
                Assert.AreEqual("7", GetCellValue(sheet, sharedStrings, "P3"));
                Assert.AreEqual("8", GetCellValue(sheet, sharedStrings, "R3"));
                Assert.AreEqual(interviewDuration, TimeSpan.FromDays(double.Parse(GetCellValue(sheet, sharedStrings, "T3"), CultureInfo.InvariantCulture)));

                Assert.AreEqual("Appointment", GetCellValue(sheet, sharedStrings, "A4"));
                Assert.AreEqual("1", GetCellValue(sheet, sharedStrings, "A5"));

                Assert.AreEqual("Terminated", GetCellValue(sheet, sharedStrings, "B4"));
                Assert.AreEqual("2", GetCellValue(sheet, sharedStrings, "B5"));

                Assert.AreEqual("Completed", GetCellValue(sheet, sharedStrings, "C4"));
                Assert.AreEqual("3", GetCellValue(sheet, sharedStrings, "C5"));
            }
        }

        private string CopyTemplateToTempDir(TemplateFileType exportType)
        {
            string tempFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string templateFile;

            switch (exportType)
            {
                case TemplateFileType.ExportSurveyActivity:
                    templateFile = @"ExcelExport\TemplExportSurveyActivity.xlsx";
                    break;
                case TemplateFileType.ExportInterviewerActivity:
                    templateFile = @"ExcelExport\TemplExportInterviewerActivity.xlsx";
                    break;
                case TemplateFileType.ExportAppointmentActivity:
                    templateFile = @"ExcelExport\TemplExportAppointmentActivity.xlsx";
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Template type {0} in not supported.", exportType));
            }

            templateFile = Path.Combine(_testDataFolder, templateFile);
            File.Copy(templateFile, tempFileName);

            return tempFileName;
        }

        private static string GetCellValue(XDocument sheet, string[] sharedStrings, string cellName)
        {
            string columnName, columnStyle;

            XElement sheetData = sheet.Root.Element(Namespace + "sheetData");

            return OpenXmlHelper.GetColumnValue(
                OpenXmlHelper.GetCell(sheetData, cellName),
                Namespace, sharedStrings,
                out columnName,
                out columnStyle);
        }

        private TaskActivityInfo GetTaskActivityInfo(int interviewId,
                                                    string projectId,
                                                    string projectName,
                                                    InterviewState interviewState,
                                                    int problemState,
                                                    DialingMode diallingMode)
        {
            TaskActivityInfo info = new TaskActivityInfo()
            {
                InterviewID = interviewId,
                ProjectId = projectId,
                ProjectName = projectName,
                InterviewState = interviewState,
                ProblemState = problemState,
                DiallingMode = diallingMode,
                TimeCallDelivered = null,
                LastKeepAliveTime = null
            };

            return info;
        }

        private AppointmentActivityInfo GetAppointmentActivityInfo(int interviewId,
                                                                   string projectId,
                                                                   string projectName,
                                                                   DateTime appointmentTime)
        {
            AppointmentActivityInfo info = new AppointmentActivityInfo()
            {
                InterviewID = interviewId,
                ProjectID = projectId,
                ProjectName = projectName,
                AppointmentTime = appointmentTime
            };

            return info;
        }
    }
}
