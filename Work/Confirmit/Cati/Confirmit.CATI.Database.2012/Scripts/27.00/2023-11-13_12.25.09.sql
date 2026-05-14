GO
IF (SELECT OBJECT_ID('tempdb..#tmpSequenceState')) IS NOT NULL DROP TABLE #tmpSequenceState
GO
IF (SELECT OBJECT_ID('tempdb..#Temp')) IS NOT NULL DROP TABLE #Temp

--we need to drop these tables because they were created and not dropped in previous update scrits what can affect on execution current and next scripts 
GO
PRINT N'Altering [dbo].[BvHistory]...';


GO
ALTER TABLE [dbo].[BvHistory]
    ADD [PreviewTime]   INT NULL,
        [ConnectedTime] INT NULL,
        [WrapTime]      INT NULL;


GO
PRINT N'Refreshing [dbo].[RestView_CallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_CallHistory]';


GO
PRINT N'Refreshing [dbo].[GetCountsForSample]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetCountsForSample]';


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
PRINT N'Refreshing [dbo].[BvSpCallHistory_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistory_List]';


GO
PRINT N'Refreshing [dbo].[BvSpCallHistoryData]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistoryData]';


GO
PRINT N'Refreshing [dbo].[BvSpHistory_GetLinkedInterviews]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpHistory_GetLinkedInterviews]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviewerProductivityReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviewerProductivityReport]';


GO
PRINT N'Refreshing [dbo].[BvSpNumberOfAttemptsReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpNumberOfAttemptsReport]';


GO
PRINT N'Refreshing [dbo].[BvSpReportInboundCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpReportInboundCalls]';


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
PRINT N'Refreshing [dbo].[BvSpSurveyOverviewReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyOverviewReport]';


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
			  BvHistory.OpenEndReviewDuration AS OpenEndReviewDuration,
			  BvHistory.PreviewTime AS PreviewTime,
			  BvHistory.ConnectedTime AS ConnectedTime,
			  BvHistory.WrapTime AS WrapTime,
			  ISNULL( BvRole.[Name], '' ) AS Role,
			  ISNULL( BvPerson.[Name], '' ) AS Person,
			  BvHistory.AppointmentID AS AppointmentID,
			  ISNULL(BvAppointment.ContactName, '' ) AS ContactName,
			  BvAppointment.[Time] AS TimeToCall,
			  BvAppointment.ExpTime AS TimeToExpire,
			  ISNULL(BvHistory.TelephoneNumber, @TelephoneNumber) AS TelephoneNumber,
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
				0 as OpenEndReviewDuration,
				0 as PreviewTime,
				0 as ConnectedTime,
				0 as WrapTime,
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
PRINT N'Update complete.';


GO

GO
PRINT N'Altering [dbo].[BvSpGetCallAttemptsReport_ListPage]...';


GO
ALTER PROCEDURE BvSpGetCallAttemptsReport_ListPage 
	@SupervisorName NVARCHAR(255),
	@PageNumber INT, 
	@PageSize INT, 
	@OrderField NVARCHAR (64), 
	@IsOrderASC INT,
	@SearchCondition NVARCHAR (4000) = NULL,
	@IncludeDisposedByDialerAttempts BIT
AS
BEGIN
	IF @SupervisorName IS NULL AND @PageNumber IS NULL AND @PageSize IS NULL
	BEGIN
	/* Looks like we're generating code using FMTONLY. So lets return metadata*/
		SELECT
		0 as [ID],
		GETDATE() as [EventDate],
		0 as [SurveySID],
		'' as [ProjectID],
		'' as [ProjectName],
		'' as [InterviewerName],
		0 as [InterviewID],
		0 as [CallDuration],
		CAST( 0 as SMALLINT) as [ExtendedStatus],
		'' as [ExtendedStatusName],
		'' as [TelephoneNumber],
		0 as [WaitingTime],
		0 as [DisplayTime],
		0 as [PreviewTime],
		0 as [ConnectedTime],
		0 as [WrapTime]
     
		RETURN 0;
	END
 
	DECLARE @StateGroupID INT
	SELECT @StateGroupID = ID FROM [BvStateGroup] WHERE [Order] = (SELECT MIN([Order]) FROM [BvStateGroup])
	
	DECLARE @Query NVARCHAR(MAX) = 'SELECT
		hist.[ID] as [ID],
		hist.[FiredTime] as [EventDate],
		survey.[SID] as [SurveySID],
		survey.[Name] as [ProjectID],
		survey.[Description] as [ProjectName],
		ISNULL(person.[Name], ''Not Applicable'') as [InterviewerName],
		hist.[InterviewId] as [InterviewID],
		hist.[Duration] as [CallDuration],
		hist.[ITS] as [ExtendedStatus],
		states.[Name] as [ExtendedStatusName],
		hist.[TelephoneNumber] as [TelephoneNumber],
		hist.[WaitingTime] as [WaitingTime],
		hist.[DisplayTime] as [DisplayTime],
		hist.[PreviewTime] as [PreviewTime],
		hist.[ConnectedTime] as [ConnectedTime],
		hist.[WrapTime] as [WrapTime]
		FROM
		[BvHistory] hist INNER JOIN [BvSurvey] survey ON hist.SurveyId = survey.[SID]
		INNER JOIN [BvUserSurveyPermission] perm ON (perm.SurveySID = survey.[SID] AND perm.UserName = ''' + @SupervisorName + ''')
		LEFT JOIN [BvPerson] person ON person.[SID] = hist.[PersonSID] 
		INNER JOIN [BvState] states ON states.StateID = hist.[ITS] AND states.StateGroupID = ' + CAST(@StateGroupID AS NVARCHAR(20)) +
		' WHERE hist.[RoleID] = 2 AND hist.InterviewId IS NOT NULL AND survey.State <> 2 AND person.[Name] IS NOT NULL OR
		(person.[Name] IS NULL AND '+ CAST(@IncludeDisposedByDialerAttempts AS NVARCHAR(50)) + ' = 1)'

	DECLARE @TotalCount INT
	exec @TotalCount = BvSpGetListPage @PageNumber, @PageSize, @OrderField, @IsOrderASC, @Query, 'ID', @SearchCondition
	RETURN @TotalCount
END
GO
PRINT N'Update complete.';


GO
