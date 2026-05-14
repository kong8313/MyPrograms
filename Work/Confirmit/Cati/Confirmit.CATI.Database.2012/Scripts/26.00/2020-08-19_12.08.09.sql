PRINT N'Altering [dbo].[BvSpSurveyCleanup_GetSurveysWhichAreReadyForCleanup]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurveyCleanup_GetSurveysWhichAreReadyForCleanup]
        @LastTouchTime DATETIME,
        @LastSendNoticyTime DATETIME
AS
	SELECT SID AS Id, Name, Description, NotificationEmail, ISNULL(sample.Count, 0) SampleSize, LastTouchTime
    FROM BvSurvey s
        LEFT JOIN BvUserNotification n
        ON n.ObjectId = s.SID AND n.Type = 1/*UserNotificationType.SurveyCleanupNotificationWarning*/ AND  s.LastTouchTime < n.SendDate
        LEFT JOIN (SELECT COUNT(*) as Count, SurveySID FROM BvInterview group by SurveySid ) as sample on SID = sample.SurveySID 
        WHERE State = 0 AND LastTouchTime < @LastTouchTime AND n.SendDate < @LastSendNoticyTime
GO
PRINT N'Altering [dbo].[BvSpSurveyCleanup_GetSurveysWhichAreReadyForNotice]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurveyCleanup_GetSurveysWhichAreReadyForNotice]
    @LastTouchTime DATETIME
AS
    SELECT SID AS Id, Name, Description, NotificationEmail, ISNULL(sample.Count, 0) SampleSize, LastTouchTime
    FROM BvSurvey s
        LEFT JOIN BvUserNotification n
        ON n.ObjectId = s.SID AND n.Type = 1/*UserNotificationType.SurveyCleanupNotificationWarning*/ AND  s.LastTouchTime < n.SendDate
        LEFT JOIN (SELECT COUNT(*) as Count, SurveySID FROM BvInterview group by SurveySid ) as sample on SID = sample.SurveySID 
        WHERE State = 0 AND LastTouchTime < @LastTouchTime AND n.Id IS NULL
GO
PRINT N'Update complete.';


GO
