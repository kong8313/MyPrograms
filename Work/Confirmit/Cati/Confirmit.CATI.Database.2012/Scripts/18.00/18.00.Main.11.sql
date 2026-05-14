GO
PRINT N'Dropping [dbo].[BvSpSample_Abandon]...';


GO
DROP PROCEDURE [dbo].[BvSpSample_Abandon];


GO
PRINT N'Dropping [dbo].[BvSpSample_Finalize]...';


GO
DROP PROCEDURE [dbo].[BvSpSample_Finalize];


GO
PRINT N'Add new system settings...';

GO

INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
   SELECT 'AsyncOperation.AddSamplePortionSize', 'Asynchronous ''Add sample'' portion size', 'Supervisor', 'Portion size for asynchronous Add sample operation.', 1, 0, '10000'

GO
PRINT N'Delete sample records from history...';

GO

DELETE FROM BvHistory WHERE RoleId = 0

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
				NULL as Role,
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
		 FROM BvSamples WHERE BatchID =  @BatchID
	 ) t
     ORDER BY DATEADD( s, -Duration, EndTime)

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Update complete.';


GO
