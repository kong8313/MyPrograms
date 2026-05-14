using Confirmit.CATI.Core.Mail;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Core.EmailReports
{
    public class EmailReportRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.Register<IScheduledReportEmail, CallHistoryScheduledReportEmail>("CallHistoryEmailReport");
            serviceRegistrator.Register<IScheduledReportEmail, SurveyOverviewScheduledReportEmail>("SurveyOverviewEmailReport");
            serviceRegistrator.Register<IScheduledReportEmail, SurveyProductivityScheduledReportEmail>("SurveyProductivityEmailReport");
            serviceRegistrator.Register<IScheduledReportEmail, CustomInterviewerProductivityScheduledReportEmail>("InterviewerProductivityEmailReport");

            serviceRegistrator.Register<IEmailReportsManager, EmailReportsManager>();
            serviceRegistrator.Register<IMailSender, ConfirmitAuthoringMailSender>();

            serviceRegistrator.Register<ILocalTimeProvider, LocalTimeProvider>();

            //?serviceRegistrator.Register<IReportBuilder, CallHistoryReportBuilder>("CallHistoryReportBuilder");
            //?serviceRegistrator.Register<IReportBuilder, SurveyOverviewReportBuilder>("SurveyOverviewReportBuilder");
            //?serviceRegistrator.Register<IReportBuilder, SurveyProductivityReportBuilder>("SurveyProductivityReportBuilder");
            //?serviceRegistrator.Register<IReportBuilder, InterviewerProductivityReportBuilder>("InterviewProductivityReportBuilder");
        }
    }
}
