using System;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.SystemSettings;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.CleaningService
{
    public class SurveyCleaningService : ISurveyCleaningService
    {
        private readonly ISystemSettings _settings;
        private readonly IEmailNotificationService _emailService;
        private readonly ISurveyCleaningDataAccess _surveyCleaningDataAccess;
        private readonly ICleaningServiceEmailGenerator _cleaningServiceEmailGenerator;

        public SurveyCleaningService(
            ISystemSettings settings,
            IEmailNotificationService emailService,
            ISurveyCleaningDataAccess surveyCleaningDataAccess,
            ICleaningServiceEmailGenerator cleaningServiceEmailGenerator)
        {
            _settings = settings;
            _emailService = emailService;
            _surveyCleaningDataAccess = surveyCleaningDataAccess;
            _cleaningServiceEmailGenerator = cleaningServiceEmailGenerator;
        }
        
        public void CleanAllUnusedSurveys()
        {
            SendNotificationForSurveyWhichAreReadyForNotice();

            CleanSurveyAndSendNotification();
        }

        private void CleanSurveyAndSendNotification()
        {
            var lastTouchTime = DateTime.UtcNow - _settings.RoutineMaintenance.Actions.SurveyCleanup.NotificationTimeout - _settings.RoutineMaintenance.Actions.SurveyCleanup.CleanupTimeout;
            var lastSentNoticeTime = DateTime.UtcNow - _settings.RoutineMaintenance.Actions.SurveyCleanup.CleanupTimeout;
            var surveys = _surveyCleaningDataAccess.GetSurveysWhichAreReadyForCleanup(lastTouchTime, lastSentNoticeTime);

            if (surveys.Count == 0)
            {
                return;
            }

            _emailService.SendEmail(
                true,
                CleaningServiceEmailGenerator.CleanupSubject,
                null,
                _cleaningServiceEmailGenerator.GetCleanupBody(surveys));

            var groupedSurveys = surveys.GroupBy(x => x.NotificationEmail);

            foreach (var groupedSurvey in groupedSurveys)
            {
                var notificationEmail = groupedSurvey.Key;
                var surveysToSend = groupedSurvey.ToList();

                try
                {
                    var bodyHtml = _cleaningServiceEmailGenerator.GetCleanupBody(surveysToSend);

                    _emailService.SendEmail(
                        notificationEmail,
                        CleaningServiceEmailGenerator.CleanupSubject,
                        null,
                        bodyHtml);

                    foreach (var survey in surveysToSend)
                    {
                        try
                        {
                            _surveyCleaningDataAccess.CleanSurvey((int)survey.Id);

                            SurveyService.UpdateLastTouchTime((int)survey.Id);

                            BvUserNotificationAdapter.Insert(
                                new BvUserNotificationEntity
                                {
                                    ObjectId = (int)survey.Id,
                                    Type = (int)UserNotificationType.SurveyCleanupNotification,
                                    SendDate = DateTime.UtcNow,
                                    Subject = CleaningServiceEmailGenerator.CleanupSubject,
                                    Body = bodyHtml
                                });
                        }
                        catch (Exception ex)
                        {
                            TraceHelper.TraceException(ex, $"An error occured during cleaning data of {survey} survey");
                        }
                    }
                }
                catch (Exception ex)
                {
                    TraceHelper.TraceException(ex, $"An error occured during sending emails about cleaning data of {string.Join(", ", surveysToSend)} surveys");
                }
            }
        }

        private void SendNotificationForSurveyWhichAreReadyForNotice()
        {
            var lastTouchTime = DateTime.UtcNow - _settings.RoutineMaintenance.Actions.SurveyCleanup.NotificationTimeout;
            var surveys = _surveyCleaningDataAccess.GetSurveysWhichAreReadyForNotice(lastTouchTime);

            if (surveys.Count == 0)
            {
                return;
            }

            _emailService.SendEmail(
                true,
                CleaningServiceEmailGenerator.WarningSubject,
                null,
                _cleaningServiceEmailGenerator.GetWarningBody(surveys));

            var groupedSurveys = surveys.GroupBy(x => x.NotificationEmail);

            foreach (var groupedSurvey in groupedSurveys)
            {
                try
                {
                    var notificationEmail = groupedSurvey.Key;
                    var surveysToSend = groupedSurvey.ToList();

                    var bodyHtml = _cleaningServiceEmailGenerator.GetWarningBody(surveysToSend);

                    _emailService.SendEmail(
                        notificationEmail,
                        CleaningServiceEmailGenerator.WarningSubject,
                        null,
                        bodyHtml);

                    foreach (var survey in surveysToSend)
                    {
                        BvUserNotificationAdapter.Insert(
                            new BvUserNotificationEntity
                            {
                                ObjectId = (int) survey.Id,
                                Type = (int) UserNotificationType.SurveyCleanupNotificationWarning,
                                SendDate = DateTime.UtcNow,
                                Subject = CleaningServiceEmailGenerator.WarningSubject,
                                Body = bodyHtml
                            });
                    }
                }
                catch (Exception ex)
                {
                    TraceHelper.TraceException(ex, $"An error occured during sending clean-up warning notification email to address '{groupedSurvey.Key}'");
                }
            }
        }
    }
}
