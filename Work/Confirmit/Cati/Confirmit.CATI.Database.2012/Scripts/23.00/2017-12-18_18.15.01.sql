PRINT N'Altering [dbo].[BvHistory]...';


GO
ALTER TABLE [dbo].[BvHistory]
    ADD [LinkedInterviewSessionId] INT NULL;


GO
PRINT N'Creating [dbo].[BvHistory].[IX_LinkedInterviewSessionId_i_SurveyId_InterviewId_Filtered]...';


GO
CREATE NONCLUSTERED INDEX [IX_LinkedInterviewSessionId_i_SurveyId_InterviewId_Filtered]
    ON [dbo].[BvHistory]([LinkedInterviewSessionId] ASC)
    INCLUDE([SurveyId], [InterviewId]) WHERE LinkedInterviewSessionId IS NOT NULL;


GO
PRINT N'Altering [dbo].[BvTasks]...';


GO
ALTER TABLE [dbo].[BvTasks]
    ADD [LinkedInterviewSessionId] INT NULL;


GO
PRINT N'Creating [dbo].[LinkedInterviewSessionSequence]...';


GO
CREATE SEQUENCE [dbo].[LinkedInterviewSessionSequence]
    AS INT
    START WITH 1
    INCREMENT BY 1;


GO
PRINT N'Refreshing [dbo].[GetCountsForSample]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetCountsForSample]';


GO
PRINT N'Refreshing [dbo].[RestView_CallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_CallHistory]';


GO
PRINT N'Altering [dbo].[BvSpCallHistory_List]...';


GO
ALTER PROCEDURE [dbo].[BvSpCallHistory_List]
@InterviewID     INTEGER,
@SurveyID        INTEGER,
@CallCenterID	 INTEGER
AS
SET NOCOUNT OFF
     DECLARE @StateGroupID INT = ( SELECT StateGroupID FROM BvSurvey WHERE SID = @SurveyID )
	 
	 DECLARE @TelephoneNumber NVARCHAR(MAX)
	 DECLARE @RespondentName NVARCHAR(MAX)
	 DECLARE @TimezoneID INT
	 DECLARE @BatchID INT
	 DECLARE @TimeZoneName NVARCHAR(MAX)
	
	 SELECT @TelephoneNumber = ISNULL(BvInterview.TelephoneNumber, '' ),
		    @RespondentName = ISNULL(BvInterview.RespondentName, '' ),
		    @TimezoneID = ISNULL(BvInterview.TimezoneID, 0 ),
		    @BatchID = BvInterview.BatchID,
		    @TimeZoneName = ISNULL(BvTimezone.[Name], '' )
		    FROM BvInterview
		    LEFT JOIN BvTimezone ON BvTimezone.[ID] = BvInterview.TimezoneID
		    WHERE BvInterview.ID = @InterviewID AND BvInterview.SurveySID = @SurveyID

     SELECT * FROM 
     (
		 SELECT
			  BvHistory.ID AS [ID],
			  BvHistory.SurveyId AS SurveyID,
			  BvHistory.FiredTime AS EndTime,
			  BvHistory.InterviewID AS InterviewID,
			  BvState.[StateID] AS ITS_ID,
			  BvState.[Name] AS TransientState,
			  BvHistory.WaitingTime AS WaitingTime,
			  BvHistory.Duration AS Duration,
			  ISNULL( BvRole.[Name], '' ) AS Role,
			  ISNULL( BvPerson.[Name], '' ) AS Person,
			  BvHistory.AppointmentID AS AppointmentID,
			  ISNULL(BvAppointment.ContactName, '' ) AS ContactName,
			  BvAppointment.[Time] AS TimeToCall,
			  BvAppointment.ExpTime AS TimeToExpire,
			  @TelephoneNumber AS TelephoneNumber,
			  @RespondentName AS RespondentName,
			  @TimezoneID AS TimeZoneID,
			  @TimeZoneName AS TimeZone,
			  ISNULL(BvHistory.LinkedInterviewSessionId, 0) AS LinkedInterviewSessionId,
			  ISNULL( BvCallCenter.Name, '' ) as CallCenterName
		 FROM BvHistory
		 INNER JOIN BvState ON BvState.StateGroupID = @StateGroupID AND BvState.[StateID] = BvHistory.ITS
		 LEFT JOIN BvPerson ON BvPerson.SID = BvHistory.PersonSID
		 LEFT JOIN BvRole ON BvRole.RoleID = BvHistory.RoleID
		 LEFT JOIN BvAppointment ON BvAppointment.[ID] = BvHistory.AppointmentID
		 LEFT JOIN BvCallCenter ON BvCallCenter.ID = BvHistory.CallCenterID
		 WHERE BvHistory.InterviewID = @InterviewID
			   AND BvHistory.SurveyId = @SurveyID
		 UNION ALL
		 SELECT 0 as [ID],
				@SurveyID as SurveyID,
				StartedTime as EndTime,
				@InterviewID as InterviewID,
				NULL as ITS_ID,
				'<Fresh sample>' as TransientState,
				0 as WaitingTime,
				0 as Duration,
				'Sample' as Role,
				NULL as Person,
				NULL as AppointmentID,
				'' as ContactName,
				NULL as TimeToCall,
				NULL as TimeToExpire,
				@TelephoneNumber AS TelephoneNumber,
				@RespondentName AS RespondentName,
				@TimezoneID AS TimeZoneID,
				@TimeZoneName AS TimeZone,
				'IsHistoryItemForChildInterview' = CAST(0 AS BIT),
				'' as CallCenterName
		 FROM BvSamples WHERE BatchID =  @BatchID AND SampleType = 0
	 ) t
     ORDER BY DATEADD( s, -Duration, EndTime)

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpHistory_CfData_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpHistory_CfData_Insert]
    @ProjectID NVARCHAR(256),
    @RespondentPhone NVARCHAR(256),
    @FiredTime DATETIME,
    @InterviewID INT,
    @Status_CF NVARCHAR(256),
    @AppointmentID INT,
    @OpenEndReviewDuration INT,
    @GrossDuration INT,
    @TotalDuration INT,
    @InterviewerID INT,
    @RoleID INT,
	@WaitingTime INT,
	@CallCenterId INT,
	@LinkedInterviewSessionId INT = NULL

AS
DECLARE @SurveySID INT
DECLARE @InterviewerID_BF INT
DECLARE @StatusBvFEE INT

    -- get survey sid and validate it
    SELECT @SurveySID = [Sid] FROM [BvSurvey] WHERE [Name] = @ProjectID
    
    IF @SurveySID IS NULL
    BEGIN
        RAISERROR('Survey for project %s does not exist', 16, 1, @ProjectID)
        RETURN -1
    END

    -- get interviewer and validate it
    IF ( @roleID = 2 /* CATI */ )
    BEGIN
        IF NOT EXISTS ( SELECT [Sid] FROM [BvPerson] WHERE [Sid] = @InterviewerID )
        BEGIN
            --We should ingnore wrong interviewer, because interviewer can be alredy deleted
            SET @InterviewerID_BF = 0
        END
        
        SET @InterviewerID_BF = @InterviewerID
    END
    ELSE IF ( @RoleID = 64 /* CAPI */ )
    BEGIN
        RAISERROR('CAPI data isn''t supported now.', 16, 1)
        RETURN -1
    END
    
    -- get BvFEE status by CfStatus and validate it
    SELECT @StatusBvFEE = [StatusCode_BvFEE] FROM [BvConfirmitStatus]
        WHERE [StatusCode_Cnf] = @Status_CF OR ( @Status_CF IS NULL AND [StatusCode_Cnf] IS NULL )
        
    IF @StatusBvFEE IS NULL
    BEGIN
        SET @StatusBvFEE = 30 --ERROR ITS
    END
    
    --if BvFEE status is appointment we should get latests active appointmentId for the interview
    --because CF does not pass appID but it should be stored in [Hst_Path3] field
    SELECT @AppointmentID = MAX([ID]) FROM [BvAppointment]
		WHERE [SurveySID] = @SurveySID AND InterviewSID = @InterviewID AND [State] = 0 /* has not call*/
  
	SET @AppointmentID = ISNULL(@AppointmentID, 0) --if appointment does not exist

    INSERT INTO [BvHistory]
    (
            [SurveyId],
            [TelephoneNumber],
            [FiredTime],
            [InterviewID],
            [ITS],
            [AppointmentID],
            [WaitingTime],
            [ConfirmitDuration],
            [Duration],
            BatchId,
            [PersonSID],
            [RoleID],
			[CallCenterID],
			[OpenEndReviewDuration],
			[LinkedInterviewSessionId]
    )
    SELECT
		@SurveySID      /*Hst_ObjID*/,
		@RespondentPhone /*TelephoneNumber*/,
		@FiredTime       /*FiredTime*/,
		@InterviewID     /*InterviewID*/,
		@StatusBvFEE     /*ITS*/,
		@AppointmentID   /*AppointmentID*/,
		@WaitingTime     /*WaitingTime*/,
		@GrossDuration   /*ConfirmitDuration*/,
		@TotalDuration   /*Duration*/,
		0                 /*BatchId*/,
		@InterviewerID_BF /*PersonSID*/,
		@RoleID           /*RoleID*/,
		@CallCenterID,
		@OpenEndReviewDuration,
		@LinkedInterviewSessionId
    FROM (
			SELECT @SurveySID SurveySID,
			       @InterviewID InterviewID
		 ) CfData
                                       

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpHistory_GetLinkedInterviews]...';


GO
CREATE PROCEDURE [BvSpHistory_GetLinkedInterviews]
	@LinkedInterviewSessionId INT
AS

	SELECT 
	    ROW_NUMBER()  OVER(ORDER BY h.ID) AS InterviewsOrder,
		h.SurveyId	AS SurveyId,
		s.Name		AS ProjectId, 
		h.InterviewId	AS InterviewId,
		@LinkedInterviewSessionId AS LinkedInterviewSessionId
	FROM BvHistory h
	JOIN BvSurvey s
		ON h.SurveyId = s.SID
	WHERE LinkedInterviewSessionId = @LinkedInterviewSessionId
	ORDER BY h.Id
	
RETURN 0
GO
PRINT N'Refreshing [dbo].[BvSpAggregateInterviewerPerformance]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAggregateInterviewerPerformance]';


GO
PRINT N'Refreshing [dbo].[BvSpAlert_RecalculateAll]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAll]';


GO
PRINT N'Refreshing [dbo].[BvSpAttemptsByDispositionReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAttemptsByDispositionReport]';


GO
PRINT N'Refreshing [dbo].[BvSpCallHistoryData]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistoryData]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviewerProductivityReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviewerProductivityReport]';


GO
PRINT N'Refreshing [dbo].[BvSpNumberOfAttemptsReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpNumberOfAttemptsReport]';


GO
PRINT N'Refreshing [dbo].[BvSpReportSampleStatusSummaryForDatesRange]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpReportSampleStatusSummaryForDatesRange]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyOverviewReportForAllPersons]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyOverviewReportForAllPersons]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyProductivityReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyProductivityReport]';


GO
PRINT N'Refreshing [dbo].[BvSpSampleUtilisationReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSampleUtilisationReport]';


GO
PRINT N'Refreshing [dbo].[BvSpDialer_Reset]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpDialer_Reset]';


GO
PRINT N'Refreshing [dbo].[BvSpFinishInterviewerBreak]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpFinishInterviewerBreak]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]';


GO
PRINT N'Refreshing [dbo].[BvSpGetInterviewerPerformanceList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerPerformanceList]';


GO
PRINT N'Refreshing [dbo].[BvSpGetListSurveyTasks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetListSurveyTasks]';


GO
PRINT N'Refreshing [dbo].[BvSpGetLoggedInPersonsCount]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetLoggedInPersonsCount]';


GO
PRINT N'Refreshing [dbo].[BvSpGetSurveyInterviews]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSurveyInterviews]';


GO
PRINT N'Refreshing [dbo].[BvSpGetSystemWideInfo]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSystemWideInfo]';


GO
PRINT N'Refreshing [dbo].[BvSpLogin_SpinUp]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLogin_SpinUp]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_ListByParent]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_ListByParent]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_ListWithTasksByType]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_ListWithTasksByType]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_SetAutomaticSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_SetAutomaticSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpSendMessageToGroups]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSendMessageToGroups]';


GO
PRINT N'Refreshing [dbo].[BvSpSendMessageToInterviewers]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSendMessageToInterviewers]';


GO
PRINT N'Refreshing [dbo].[BvSpSendMessageToSurveys]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSendMessageToSurveys]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_GetCountOfLoggedPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_GetCountOfLoggedPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpTask_UpdateActiveQuestion]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTask_UpdateActiveQuestion]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_InsertAnswerSubmissionAlertIfNeeded]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_InsertAnswerSubmissionAlertIfNeeded]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_InsertUpdate_2]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_InsertUpdate_2]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_LockByPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_LockByPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_SetTelephonyProblemForLoggedIn]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_SetTelephonyProblemForLoggedIn]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UnLockByPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UnLockByPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_Update_2]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_Update_2]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateCallOutcome]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateCallOutcome]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateInterviewState]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateInterviewState]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateKeepAlive]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateKeepAlive]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateLoggedInToDialerState]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateLoggedInToDialerState]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateNewSurveySid]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateNewSurveySid]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateProblemState]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateProblemState]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateStartTime]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateStartTime]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateStatusLogout]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateStatusLogout]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateSurveySid]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateSurveySid]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyOverviewReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyOverviewReport]';


GO
PRINT N'Update complete.';


GO
