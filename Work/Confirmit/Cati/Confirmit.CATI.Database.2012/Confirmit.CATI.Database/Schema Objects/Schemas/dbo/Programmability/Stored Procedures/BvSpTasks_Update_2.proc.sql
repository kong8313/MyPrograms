CREATE PROCEDURE [dbo].[BvSpTasks_Update_2]
 @PersonSID int,
 @SurveySID int,
 @InterviewID int,
 @InterviewState tinyint,
 @TimeCallDelivered DATETIME,
 @CallOutcome int,
 @TzID int,
 @DiallingMode TINYINT
AS

DECLARE @Now DATETIME = [dbo].GetUtcNow()

IF( @SurveySID = 0 OR @InterviewID = 0 OR @InterviewState = 0)
BEGIN
 UPDATE [dbo].[BvTasks]
  SET SurveySID = @SurveySID,
   InterviewID = 0,
   InterviewState = @InterviewState,
   CallOutcome = (CASE WHEN @SurveySID = 0 THEN -1 ELSE @CallOutcome END),
   TzID = 0,
   CallID  = 0,
   TimeStateChanged = @Now,
   TimeCallDelivered = @TimeCallDelivered,
   DiallingMode = @DiallingMode
 WHERE PersonSID = @PersonSID
END
ELSE BEGIN
 UPDATE [dbo].[BvTasks]
  SET SurveySID = @SurveySID,
   InterviewID = @InterviewID,
   TimeStateChanged = @Now,
   InterviewState = @InterviewState,
   TimeCallDelivered = @TimeCallDelivered,
   CallOutcome = @CallOutcome,
   TzID = @TzID,
   DiallingMode = @DiallingMode
 WHERE PersonSID = @PersonSID
END

SELECT @@ROWCOUNT AS [RowCount], CallId from BvTasks
 WHERE PersonSID = @PersonSID