using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Mail;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using MailMessage = Confirmit.CATI.Core.Mail.MailMessage;

namespace Confirmit.CATI.Core.EmailReports
{
    public enum ReportType
    {
        CallHistory = 1,
        SurveyOverview,
        SurveyProductivity,
        InterviewerProductivity
    }

    /// <summary>
    /// This class is the main class to manage scheduled daily email reports.
    /// </summary>
    public class EmailReportsManager : IEmailReportsManager
    {
        private const int MaxFileSize = 10; // Mb

        private readonly IMailSender _mailSender;
        private readonly IPgpEncryptionService _pgpEncryptionService;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly ILocalTimeProvider _localTimeProvider;

        public EmailReportsManager(
            IMailSender mailSender,
            IPgpEncryptionService pgpEncryptionService,
            IEmailNotificationService emailNotificationService,
            ILocalTimeProvider localTimeProvider)
        {
            _mailSender = mailSender;
            _pgpEncryptionService = pgpEncryptionService;
            _emailNotificationService = emailNotificationService;
            _localTimeProvider = localTimeProvider;
        }

        public void ProcessReports()
        {
            foreach (var scheduledReportEmail in ServiceLocator.ResolveAll<IScheduledReportEmail>())
            {
                try
                {
                    if (!scheduledReportEmail.IsSwitchedOnAndConfiguredAndItsTimeToSend() ||
                        scheduledReportEmail.IsLastDateSentRecent())
                        continue;

                    var evt = new ScheduledReportEmailEvent(scheduledReportEmail.ReportType);

                    var reportBuilder = scheduledReportEmail.GetReportBuilder();
                    var report = reportBuilder.BuildReport(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);
                    evt.AddTiming("Build report");

                    var fileName = scheduledReportEmail.ReportDataExportFileName;
                    var exportFilePath = reportBuilder.ExportReportToDisk(report, fileName);

                    evt.AddTiming("Export report to disk");

                    using (var attachmentStream = _pgpEncryptionService.EncryptIfNeeded(exportFilePath, ref fileName,
                        reportBuilder.ShouldBeEncrypted))
                    {
                        evt.AddTiming("Encrypt if needed");

                        var attachmentSizeIsOk = CheckAttachmentSize(attachmentStream);

                        var mailMessage = attachmentSizeIsOk
                            ? ComposeNormalMessage(report, attachmentStream, fileName)
                            : ComposeTooLargeAttachmentMessage(report);

                        _emailNotificationService.ParseEmailString(scheduledReportEmail.ReportRecipients)
                            .ToList()
                            .ForEach(
                                r => mailMessage.To.Add(r));

                        evt.AddTiming("Create mail message");

                        _mailSender.SendMail(mailMessage);
                        evt.AddTiming("Send mail (synch part only)");
                    }

                    scheduledReportEmail.UpdateReportLastSentTime();
                    evt.AddTiming("Update report last sent time");

                    evt.Finish();
                }
                catch (Exception e)
                {
                    TraceHelper.TraceException(e,
                        string.Format("while processing {0} report", scheduledReportEmail.ReportType));
                }
            }
        }

        private MailMessage ComposeNormalMessage(IReport report, MemoryStream stream, string exportFileName)
        {
            return new MailMessage
            {
                Subject = report.Title,
                Body = MailMessage.CombineBody(report.Name, _localTimeProvider.GetCurrentLocalTime(), _localTimeProvider.GetCurrentLocalTimezoneName()),
                Attachment = stream.ToArray(),
                AttachmentName = exportFileName
            };
        }

        private MailMessage ComposeTooLargeAttachmentMessage(IReport report)
        {
            var result = new MailMessage
            {
                Subject = report.Title,
                Body = MailMessage.CombineBodyAttachmentTooLarge(report.Name, _localTimeProvider.GetCurrentLocalTime(), _localTimeProvider.GetCurrentLocalTimezoneName()),
            };

            _emailNotificationService.ParseEmailString(ServiceLocator.Resolve<ISystemSettings>().Email.AdministratorEmailAddress).ToList().ForEach(
                r => result.Bcc.Add(r));

            return result;
        }

        private static bool CheckAttachmentSize(MemoryStream stream)
        {
            if (stream.Length > MaxFileSize * 1024 * 1024)
            {
                Trace.TraceWarning("Report file size is too large, it exceeds {0}Mb and will not be sent.", MaxFileSize);
                return false;
            }

            return true;
        }
    }
}
