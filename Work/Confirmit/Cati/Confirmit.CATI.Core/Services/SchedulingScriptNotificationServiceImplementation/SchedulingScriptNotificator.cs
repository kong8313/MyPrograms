using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Mail;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;

namespace Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation
{
    public class SchedulingScriptNotificator : ISchedulingScriptNotificator
    {
        private readonly IMailSender _mailSender;
        private readonly ISurveyService _surveyService;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IEmailSettings _emailSettings;
        private readonly IEmailNotificationService _emailNotificationService;

        public SchedulingScriptNotificator(
            IMailSender mailSender,
            ISurveyService surveyService,
            ISurveyRepository surveyRepository,
            IScheduleRepository scheduleRepository,
            IEmailSettings emailSettings,
            IEmailNotificationService emailNotificationService)
        {
            _mailSender = mailSender;
            _surveyService = surveyService;
            _surveyRepository = surveyRepository;
            _scheduleRepository = scheduleRepository;
            _emailSettings = emailSettings;
            _emailNotificationService = emailNotificationService;
        }

        public void NotifyIfNeeded(Exception exception, int batchId, int interviewId, int surveyId, int scheduleId, SchedulingScriptExecutionReason executionReason = SchedulingScriptExecutionReason.Unspecified, string currentITS = "")
        {
            if (batchId != 0)
            {
                return;
            }

            var exceptionDescription = new SchedulingScriptNotificatorExceptionDescription(interviewId, exception);
            Notify(new List<SchedulingScriptNotificatorExceptionDescription> { exceptionDescription }, batchId, surveyId, scheduleId, executionReason, currentITS);
        }

        public void Notify(List<SchedulingScriptNotificatorExceptionDescription> exceptionList, int batchId, int surveyId, int scheduleId, SchedulingScriptExecutionReason executionReason = SchedulingScriptExecutionReason.Unspecified, string currentITS = "")
        {
            try
            {
                if (exceptionList.Count < 1)
                {
                    return;
                }

                string surveyName = _surveyService.GetProjectIdWithName(surveyId).Trim();
                string scheduleName = _scheduleRepository.GetById(scheduleId).Name.Trim();
                int exceptionLimit = _emailSettings.NotificationExceptionLimit;

                var mm = new MailMessage();

                AddToAddresses(mm, surveyId);
                AddBccAddresses(mm);
                mm.Subject = GetMessageSubject(batchId, surveyName, scheduleName);
                mm.Body = GetMessageBody(exceptionList, batchId, surveyName, scheduleName, exceptionLimit, executionReason, currentITS);

                if (mm.To.Count == 0 && mm.Bcc.Count == 0)
                {
                    Trace.TraceInformation(
                        "Email was not sent bacause E-mail addresses were not provided : subject: {0}, body: {1}",
                        mm.Subject ?? "not defined",
                        mm.Body ?? "not defined");
                }
                else
                {
                    _mailSender.SendMail(mm);
                }
            }
            catch (Exception e)
            {
                TraceHelper.TraceException(e, "Notify");
            }
        }

        private void AddBccAddresses(MailMessage mm)
        {
            string emails = _emailSettings.NotificationEmailBCC;

            if (!string.IsNullOrEmpty(emails))
            {
                foreach (string address in _emailNotificationService.ParseEmailString(emails))
                {
                    mm.Bcc.Add(address);
                }
            }
        }

        private void AddToAddresses(MailMessage mm, int surveyId)
        {
            string emails = _surveyRepository.GetById(surveyId).NotificationEmail;

            if (!string.IsNullOrEmpty(emails))
            {
                foreach (string address in _emailNotificationService.ParseEmailString(emails))
                {
                    mm.To.Add(address);
                }
            }
        }

        private string GetMessageBody(List<SchedulingScriptNotificatorExceptionDescription> exceptionList, int batchId, string surveyName, string scheduleName, int exceptionLimit, SchedulingScriptExecutionReason executionReason, string currentITS)
        {
            string body = GetBodyHeader(exceptionList, batchId, surveyName, scheduleName, exceptionLimit, executionReason, currentITS);
            body += GetBodyExceptionList(exceptionList, batchId, exceptionLimit) + Environment.NewLine + Environment.NewLine;
            body +=
@"To see more details about scheduling errors you can open the Errors List in the Supervisor UI:
To do so, open the CATI Supervisor UI, switch to the Scheduling tab 
then right click on the applicable scheduling definition and select 'Errors List' in the context menu.";
            return body;
        }

        private string GetBodyExceptionList(List<SchedulingScriptNotificatorExceptionDescription> exceptionList, int batchId, int exceptionLimit)
        {
            string s = "";
            int exceptionCnt = Math.Min(exceptionList.Count, exceptionLimit);

            for (int i = 0; i < exceptionCnt; i++)
            {
                SchedulingScriptNotificatorExceptionDescription exception = exceptionList[i];
                if (!string.IsNullOrEmpty(exception.RuleNumber))
                {
                    s += string.Format(
@"Date: {0}
Triggered by: {1}
Interview extended status: {2}
Rule: {3}
Action: {4}
",
                    exception.HappenedAt.ToString(CultureInfo.InvariantCulture),
                    exception.ExecutionReason,
                    exception.ExtendedStatus,
                    exception.RuleNumber,
                    exception.Action);
                }
                if (batchId != 0)
                    s += string.Format("Respondent ID: {0}" + Environment.NewLine, exception.InterviewId);

                s += "Message: " + exception.Message + Environment.NewLine + Environment.NewLine;
            }

            return PrepareMailMessage(s);
        }

        private string GetBodyHeader(List<SchedulingScriptNotificatorExceptionDescription> exceptionList, int batchId, string surveyName, string scheduleName, int exceptionLimit, SchedulingScriptExecutionReason executionReason, string currentITS)
        {
            int exceptionTotalCount = exceptionList.Count;
            string registeredDate = exceptionList[0].HappenedAt.ToString(CultureInfo.InvariantCulture);
            string s;

            if (batchId == 0)
            {
                s = string.Format(
@"Date: {0}
Project: {1}
Scheduling script: {2}
Respondent ID: {3}
Triggered by: {4}
Interview extended status: {5}

"
                    , registeredDate
                    , surveyName
                    , scheduleName
                    , exceptionList[0].InterviewId
                    , SchedulingScriptExecutionReasonConverter.ConvertToString(executionReason)
                    , currentITS
                    ); ;
            }
            else
            {
                s = string.Format(
@"Project: {0}
Scheduling script: {1}"
                    , surveyName
                    , scheduleName
                    );
                if (batchId > 0)
                {
                    s += string.Format(Environment.NewLine + @"BatchId:{0}", batchId);
                }
                s += Environment.NewLine + Environment.NewLine;

                s += string.Format(@"Error(s) detected: {0}.", exceptionTotalCount, scheduleName);

                if (exceptionTotalCount > exceptionLimit)
                {
                    s += string.Format(Environment.NewLine + "The first {0} errors are shown below:", exceptionLimit);
                }
            }

            return PrepareMailMessage(s) + Environment.NewLine + Environment.NewLine;
        }

        private string PrepareMailMessage(string message)
        {
            // Workaround for Outlook and Gmail to now remove New Lines
            // Start each line with 2 empty spaces
            return "  " + message.Replace(Environment.NewLine, Environment.NewLine + "  ").Trim();
        }

        internal string GetMessageSubject(int batchId, string surveyName, string scheduleName)
        {
            string subject;

            if (batchId == 0)
            {
                // CATI scheduling error in <name of scheuling script> for <pxxxxx> <name of project>
                subject = string.Format(
                  "CATI scheduling error in {1} for {0}"
                , surveyName
                , scheduleName);
            }
            else
            {
                // CATI sample scheduling error in <name of scheuling> for <pxxxxx> <name of project>
                subject = string.Format(
                  "CATI sample scheduling error in {1} for {0}"
                , surveyName
                , scheduleName);
            }

            return subject;
        }
    }
}