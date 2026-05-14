GO
PRINT N'Altering Procedure [dbo].[BvSpCallHistory_List]...';


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
			  ISNULL( BvCallCenter.Name, '' ) as CallCenterName,
			  BvHistory.CallAttemptNumber as CallAttemptNumber
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
				'' as CallCenterName,
				NULL as CallAttemptNumber
		 FROM BvSamples WHERE BatchID =  @BatchID AND SampleType = 0
	 ) t
     ORDER BY DATEADD( s, -Duration, EndTime)

RETURN (0)
GO
PRINT N'Update complete.';


GO
