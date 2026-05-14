using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Reports
{
    /// <summary>
    /// Reports list class.
    /// </summary>
    public partial class ReportsList : BaseForm
    {
        class ReportItem
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }
        }

        private readonly IToggleSettings _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();

        public override string TopTitle
        {
            get
            {
                return Strings.ReportsListDoubleClickToOpen;
            }
        }

        private object GetPage(out int totalCount)
        {
            var reports = new List<ReportItem>
            {
                new ReportItem
                {
                    Name = Strings.SurveyOverview,
                    Type = Strings.MultiSurvey,
                    Description = Strings.SurveyOverviewReportDescription
                },
                new ReportItem
                {
                    Name = Strings.SurveyProductivity,
                    Type = Strings.MultiSurvey,
                    Description = Strings.ProductivityReportDescription
                },
                new ReportItem
                {
                    Name = Strings.SampleStatusSummary,
                    Type = Strings.SingleSurvey,
                    Description = Strings.SampleStatusSummaryReportDescription
                },
                new ReportItem
                {
                    Name = Strings.CallAttemptsReportCaption,
                    Type = Strings.Log,
                    Description = Strings.CallAttemptsReportDescription
                },
                new ReportItem
                {
                    Name = Strings.InterviewerProductivity,
                    Type = Strings.MultiSurvey,
                    Description = Strings.InterviewerProductivityReportDescription
                },
                new ReportItem
                {
                    Name = Strings.AttemptsByDisposition,
                    Type = Strings.SingleSurvey,
                    Description = Strings.AttemptsByDispositionReportDescription
                },
                new ReportItem
                {
                    Name = Strings.NumberOfAttempts,
                    Type = Strings.SingleSurvey,
                    Description = Strings.NumberOfAttemptsReportDescription
                },
                new ReportItem
                {
                    Name = Strings.InterviewerSubmissionDetails,
                    Type = Strings.Log,
                    Description = Strings.InterviewerSubmissionDetailsDescription
                },
                new ReportItem
                {
                    Name = Strings.AggregatedInterviewerSubmission,
                    Type = Strings.MultiSurvey,
                    Description = Strings.AggregatedInterviewerSubmissionDescription
                },
                new ReportItem
                {
                    Name = Strings.InterviewerSessions,
                    Type = Strings.Log,
                    Description = Strings.InterviewerSessionsDescription
                },
                new ReportItem
                {
                    Name = Strings.SampleUtilisation,
                    Type = Strings.SingleSurvey,
                    Description = Strings.SampleUtilisationReportDescription
                },
                new ReportItem
                {
                    Name = Strings.SampleStatusSummaryByQuestion,
                    Type = Strings.SingleSurvey,
                    Description = Strings.SampleStatusSummaryByQuestionReportDescription
                },
                new ReportItem
                {
                    Name = Strings.QuotaProgress,
                    Type = Strings.SingleSurvey,
                    Description = Strings.QuotaProgressReportDescription
                },

            };

            if (_toggleSettings.EnableInbound)
            {
                reports.Add(new ReportItem
                {
                    Name = Strings.InboundCallHistoryReport,
                    Type = Strings.Log,
                    Description = Strings.InboundCallHistoryReportDescription
                });
                reports.Add(new ReportItem
                {
                    Name = Strings.InboundCallSummaryReport,
                    Type = Strings.SingleSurvey,
                    Description = Strings.InboundCallSummaryReportDescription
                });
            }
            totalCount = reports.Count();
            return reports;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            gridReports.GetPage += GetPage;
            gridReports.ClientEvents.DoubleClick = "OpenReport";

            RegisterScripts();
        }

        private void RegisterScripts()
        {
            RegisterReportForOpenInRightFrame("OpenSurveyOverviewReport", "Reports/SurveyOverviewReport.aspx");
            RegisterReportForOpenInRightFrame("OpenProductivityReport", "Reports/ProductivityReport.aspx?OpenSource=CP");
            RegisterReportForOpenInRightFrame("OpenSSSReport", "Reports/SampleStatusSummaryReport.aspx");
            RegisterReportForOpenInRightFrame("OpenCallAttempts", "Reports/CallAttemptsReport.aspx");
            RegisterReportForOpenInRightFrame("OpenInterviewerProductivityReport", "Reports/CatiProductivityReport.aspx");
            RegisterReportForOpenInRightFrame("OpenAttemptsByDispositionReport", "Reports/AttemptsByDispositionReport.aspx");
            RegisterReportForOpenInRightFrame("OpenNumberOfAttemptsReport", "Reports/NumberOfAttemptsReport.aspx");
            RegisterReportForOpenInRightFrame("OpenInterviewerSubmissionDetails", "Reports/AlertsHistoryReport.aspx");
            RegisterReportForOpenInRightFrame("OpenAggregatedInterviewerSubmission", "Reports/AlertsHistoryAggregatedReport.aspx");
            RegisterReportForOpenInRightFrame("OpenInterviewerSessions", "Reports/InterviewerSessionsReport.aspx");
            RegisterReportForOpenInRightFrame("OpenSampleUtilisationReport", "Reports/SampleUtilisationReport.aspx");
            RegisterReportForOpenInRightFrame("OpenSSSByQuestionReport", "Reports/SampleStatusSummaryByQuestionReport.aspx");
            RegisterReportForOpenInRightFrame("OpenQuotaProgressReport", "Reports/QuotaProgressReport.aspx");
            RegisterReportForOpenInRightFrame("OpenInboundCallsHistoryReport", "Reports/InboundCallHistoryReport.aspx");
            RegisterReportForOpenInRightFrame("OpenInboundCallSummaryReport", "Reports/InboundCallSummaryReport.aspx");

            string script =
                @"function OpenReport(gridID, eventArgs) 
	            {
                    if (eventArgs.get_type() != 'cell')// do not process header click
                        return; 

	                var index = eventArgs.get_item().get_row().get_index();
	                switch(index)
                    {
	                    case 0:
	                    {
	                        OpenSurveyOverviewReport();
                            break;
	                    }
	                    case 1:
	                    {
	                        OpenProductivityReport();
                            break;
	                    }
	                    case 2:
	                    {
	                        OpenSSSReport();
                            break;
	                    }
                        case 3:
                        {
                            OpenCallAttempts();
                            break;
                        }
	                    case 4:
	                    {
	                        OpenInterviewerProductivityReport();
                            break;
	                    }
	                    case 5:
	                    {
	                        OpenAttemptsByDispositionReport();
                            break;
	                    }
	                    case 6:
	                    {
	                        OpenNumberOfAttemptsReport();
                            break;
	                    }
	                    case 7:
	                    {
	                        OpenInterviewerSubmissionDetails();
                            break;
	                    }
	                    case 8:
	                    {
	                        OpenAggregatedInterviewerSubmission();
                            break;
	                    }
                        case 9:
	                    {
	                        OpenInterviewerSessions();
                            break;
	                    }
                        case 10:
                        {
                            OpenSampleUtilisationReport();
                            break;
                        }
                        case 11:
                        {
                            OpenSSSByQuestionReport();
                            break;
                        }
                        case 12:
                        {
                            OpenQuotaProgressReport();
                            break;
                        }
                        case 13:
                        {
                            OpenInboundCallsHistoryReport();
                            break;
                        }
                        case 14:
                        {
                            OpenInboundCallSummaryReport();
                            break;
                        }
                    }
	            }";

            ClientScript.RegisterClientScriptBlock(Page.GetType(), "OpenReport", script, true);
        }

        /// <summary>
        /// Registers JavaScript function which opens report in right frame.
        /// </summary>
        /// <param name="jsFunctionName">JavaScript function name.</param>
        /// <param name="reportUrl">Report url.</param>
        private void RegisterReportForOpenInRightFrame(string jsFunctionName, string reportUrl)
        {
            string script = String.Format(
                "function {0}()" +
                "{{" +
                    "top.setListFrameUrl('{1}');" +
                "}}",
                jsFunctionName,
                reportUrl
            );
            ClientScript.RegisterClientScriptBlock(Page.GetType(), jsFunctionName, script, true);
        }
    }
}