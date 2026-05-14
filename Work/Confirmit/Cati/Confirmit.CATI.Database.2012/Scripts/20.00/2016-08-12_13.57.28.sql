PRINT N'Dropping [dbo].[BvSamples].[ix_BvSamples1]...';


GO
DROP INDEX [ix_BvSamples1]
    ON [dbo].[BvSamples];


GO
PRINT N'Starting rebuilding table [dbo].[BvSamples]...';


GO
CREATE TABLE [dbo].[tmp_ms_xx_BvSamples] (
    [BatchID]          INT            NOT NULL,
    [SurveySID]        INT            NOT NULL,
    [State]            INT            NOT NULL,
    [StateDescription] NVARCHAR (MAX) NOT NULL,
    [StartedTime]      DATETIME       NOT NULL,
    [FinishedTime]     DATETIME       NULL,
    [CountInterviews]  INT            NOT NULL,
    [SampleType]       INT            CONSTRAINT [DF_BvSamples_SampleType] DEFAULT (0) NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_Pk_BvSamples] PRIMARY KEY CLUSTERED ([BatchID] ASC, [SampleType] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[BvSamples])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_BvSamples] ([BatchID], [SurveySID], [State], [StateDescription], [StartedTime], [FinishedTime], [CountInterviews])
        SELECT   [BatchID],
                 [SurveySID],
                 [State],
                 [StateDescription],
                 [StartedTime],
                 [FinishedTime],
                 [CountInterviews]
        FROM     [dbo].[BvSamples]
        ORDER BY [BatchID] ASC;
    END

DROP TABLE [dbo].[BvSamples];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BvSamples]', N'BvSamples';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_Pk_BvSamples]', N'Pk_BvSamples', N'OBJECT';

GO
PRINT N'Creating [dbo].[BvSamples].[ix_BvSamples1]...';


GO
CREATE NONCLUSTERED INDEX [ix_BvSamples1]
    ON [dbo].[BvSamples]([State] ASC, [SampleType] ASC);


GO
PRINT N'Creating [dbo].[BvSamples].[ix_BvSamples]...';


GO
CREATE NONCLUSTERED INDEX [ix_BvSamples]
    ON [dbo].[BvSamples]([SurveySID] ASC, [StartedTime] ASC);


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
			  'IsHistoryItemForChildInterview' = CAST(0 AS BIT),
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
PRINT N'Altering [dbo].[BvSpSampleUtilisationReport]...';


GO
ALTER PROCEDURE [dbo].[BvSpSampleUtilisationReport]
(
 @SurveySid INT,
 @CompletedItses NVARCHAR(MAX),
 @StartDateTime DATETIME,
 @EndDateTime DATETIME
)
AS
BEGIN

SET @StartDateTime = ISNULL(@StartDateTime, '1753-01-01T00:00:00.000')
SET @EndDateTime = ISNULL(@EndDateTime, '9999-12-31T23:59:59.997');

SELECT  
	s.BatchId		[Batchid], 
	ss.Name			[SurveyName], 
	s.FinishedTime		[BatchAddedAt],
	s.CountInterviews 	[InterviewsAdded],
	counts.*,
	s.CountInterviews - counts.[InterviewsCurrent] as [InterviewsDeleted]
 FROM bvsamples s 
 JOIN bvsurvey ss
	on s.SurveySID = ss.SID
	CROSS APPLY dbo.GetCountsForSample(s.batchid, @CompletedItses) counts

WHERE s.State = 2 AND s.SampleType = 0 AND s.SurveySID = @SurveySid AND s.StartedTime >= @StartDateTime AND s.FinishedTime <= @EndDateTime
ORDER BY s.FinishedTime
END
GO
PRINT N'Update complete.';


GO
