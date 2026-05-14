CREATE PROCEDURE [dbo].[BvSpTasks_UpdateInterviewState]
 @PersonSID int,
 @InterviewState int,
 @DiallingMode tinyint
AS

IF @InterviewState = 0 --NO_CALLS
BEGIN
 
 UPDATE [dbo].[BvTasks]
     SET 
      InterviewID = 0, 
      CallID = 0,
      TzID = 0,
      TimeStateChanged = GETUTCDATE(),
      TimeCallDelivered = NULL,
      InterviewState = @InterviewState,
      DiallingMode = @DiallingMode
 WHERE PersonSID = @PersonSID
END
ELSE IF @InterviewState = 6 --INTERVIEW_WRAP_UP 
BEGIN
 UPDATE [dbo].[BvTasks]
     SET InterviewState = @InterviewState,
      State = null,
      TimeStateChanged = GETUTCDATE()
 WHERE PersonSID = @PersonSID
END
ELSE IF @InterviewState = 5 --OPEN END REVIEW 
BEGIN
 UPDATE [dbo].[BvTasks]
     SET InterviewState = @InterviewState,
     OpenEndReviewStartTime = GETUTCDATE()
 WHERE PersonSID = @PersonSID
END

ELSE
BEGIN
 UPDATE [dbo].[BvTasks]
     SET InterviewState = @InterviewState
 WHERE PersonSID = @PersonSID
END

RETURN @@ROWCOUNT