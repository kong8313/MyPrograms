PRINT N'Altering [dbo].[BvSpGetListSurveyTasks]...';


GO
ALTER PROCEDURE BvSpGetListSurveyTasks
   @surveysBatchID int,
   @interviewersBatchID int,   
   @TimeZoneID INT,
   @CallCenterID INT,
   @UserName NVARCHAR(MAX)

AS
   DECLARE @currTime DATETIME
   EXEC @currTime = GetUtcNow
   DECLARE @AmberOfLastSubmission INT
   DECLARE @RedOfLastSubmission INT
   DECLARE @AmberOfLastKeepAliveTime INT
   DECLARE @RedOfLastKeepAliveTime INT
   DECLARE @AmberOfNoActivity INT
   DECLARE @RedOfNoActivity INT

   SELECT @AmberOfLastSubmission = Amber, @RedOfLastSubmission = Red
   FROM BvThresholds 
   WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 1/*Task alert*/

   SELECT @AmberOfLastKeepAliveTime = Amber, @RedOfLastKeepAliveTime = Red
   FROM BvThresholds 
   WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 16/*Last keep alive alert*/
   
   SELECT @AmberOfNoActivity = Amber, @RedOfNoActivity = Red
   FROM BvThresholds 
   WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 18/*No activity alert*/
   
   SELECT tsk.InterviewID, 
          tsk.PersonSID, 
          p.Name as InterviewerName, 
          tsk.SurveySID, 
          tsk.ProjectID, 
          tsk.SurveyName,
          tsk.TimeCallDelivered, 
          tsk.State, 
          tsk.SecondsSinceLastSubmission, 
          tsk.LastSubmissionAlert, 
          tsk.LastKeepAliveTime,
          tsk.LastKeepAliveTimeAlert,
          tsk.EndOfLastActivityAlert,
          tsk.InterviewState,
          tsk.LoggedInToDialerState,
          tsk.TzID, 
          tsk.DiallingMode, -- if no survey assigned to task - use manual dialing mode
          tsk.CallOutcome, 
          tsk.StatusLogout,
          tsk.ProblemId,
          tz.Bias, 
          pm.supervisorName,
          pm.MonitoringSessionID,
          tsk.StationId,
		  tsk.DialType,
		  tsk.OpenEndReviewInSeconds,
		  tsk.DialerId
   FROM
   (SELECT t.InterviewID, 
          t.PersonSID, 
          t.SurveySID, 
          ISNULL(s.Name, '') as ProjectID, 
          ISNULL(s.Description, '') as SurveyName,
          (CASE WHEN t.StatusLogout != 6 /*BREAK*/ THEN t.TimeCallDelivered 
                ELSE lb.StartTime
           END) as TimeCallDelivered, 
          t.State, 
          (CASE WHEN t.InterviewID = 0 THEN NULL ELSE ISNULL(DATEDIFF(second, TimeStateChanged, @currTime), 0) END) as SecondsSinceLastSubmission, 
          (CASE WHEN InterviewID > 0 
				THEN tsc.val
				ELSE 0
			END) LastSubmissionAlert, 
          t.LastKeepAliveTime,
          (CASE WHEN LastKeepAliveTime IS NULL 
				THEN 2 
				ELSE lkat.val
			END) LastKeepAliveTimeAlert,
          (CASE WHEN TimeCallDelivered IS NULL AND t.StartTime IS NOT NULL
                 THEN  naa.val 
                 ELSE 0 
          END) EndOfLastActivityAlert,
          t.InterviewState,
          t.LoggedInToDialerState,
          t.TzID, 
          t.DiallingMode, 
          t.CallOutcome, 
          t.StatusLogout,
          t.ProblemId,
          t.StationId,
		  dt.Name as DialType,
		  CASE WHEN t.OpenEndReviewStartTime IS NOT NULL THEN DATEDIFF(ss, t.OpenEndReviewStartTime, GETUTCDATE()) ELSE NULL END AS OpenEndReviewInSeconds,
		  t.DialerId
   FROM BvTasks t
   LEFT JOIN BvFnSurvey_GetByTransferBatch( @surveysBatchID ) s ON (t.SurveySID = s.SID)
   LEFT JOIN BvUserSurveyPermission up ON t.SurveySID = up.SurveySID AND up.UserName = @UserName
   INNER JOIN BvDialType dt ON t.DialTypeId = dt.Id
   INNER JOIN dbo.BvFnPerson_GetByTransferBatch(@interviewersBatchID) pta ON pta.Id = t.PersonSID
   OUTER APPLY dbo.GetLastTimeBreak(t.PersonSID) lb
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, LastKeepAliveTime, @currTime), @AmberOfLastKeepAliveTime, @RedOfLastKeepAliveTime) as lkat
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, TimeStateChanged, @currTime), @AmberOfLastSubmission, @RedOfLastSubmission ) as tsc
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, t.StartTime  , @currTime), @AmberOfNoActivity, @RedOfNoActivity ) as naa
   WHERE (s.SID IS NOT NULL AND up.SurveySID IS NOT NULL) OR t.SurveySID = 0) as tsk
   INNER JOIN BvFnPerson_Get(@CallCenterID) p ON (tsk.PersonSID = p.SID)
   INNER JOIN BvTimezone tz ON ((CASE WHEN TzID = 0 THEN @TimeZoneID ELSE TzId END) = tz.ID)
   LEFT JOIN BvPersonMonitoring pm ON (pm.PersonSID = tsk.PersonSID)
GO
PRINT N'Update complete.';


GO
