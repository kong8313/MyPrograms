GO
PRINT N'Altering [dbo].[BvSpReportSSS]...';


GO
ALTER  PROCEDURE BvSpReportSSS
@SurveySID INT, @SelectInterviewsQuery NVARCHAR (MAX)
AS
 IF @SurveySID IS NULL AND @SelectInterviewsQuery IS NULL
 BEGIN
 /* Looks like we're generating code using FMTONLY. So lets return metadata*/
 SELECT
     0  AS id,
     '' AS name,
     0  AS count,
	 0  AS fcd_disabled_call,
	 0  AS enabled_call,
	 0  AS user_disabled_call,
     0  AS sample_size
     RETURN 0;
 END
 
DECLARE @Query NVARCHAR(MAX) ='
         SELECT
             allInterviews.TransientState            ''id'',
             allInterviews.StateName                 ''name'',
             count( allInterviews.TransientState )   ''count'',
			 sum( CASE WHEN call.CallState = 1 THEN 1 ELSE 0 END ) ''fcd_disabled_call'',
			 sum( CASE WHEN call.CallState = 2 THEN 1 ELSE 0 END ) ''enabled_call'',
			 sum( CASE WHEN call.CallState = 3 THEN 1 ELSE 0 END ) ''user_disabled_call'',
             (SELECT count( * ) FROM BvInterview
              WHERE SurveySID = @SurveySID)          ''sample_size''
         FROM ( '+ @SelectInterviewsQuery + ' ) as allInterviews
		 LEFT JOIN BvSvySchedule call ON call.SurveySID = @SurveySID AND call.InterviewId = allInterviews.ID
         GROUP BY allInterviews.TransientState, allInterviews.StateName
         ORDER BY allInterviews.TransientState'

     EXEC sp_executesql  @Query, N'@SurveySID INT',
     @SurveySID = @SurveySID
GO
PRINT N'Update complete.';


GO
