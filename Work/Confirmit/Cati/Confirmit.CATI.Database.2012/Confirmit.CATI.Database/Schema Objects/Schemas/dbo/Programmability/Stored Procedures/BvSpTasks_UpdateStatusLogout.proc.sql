CREATE PROCEDURE [dbo].[BvSpTasks_UpdateStatusLogout]
 @PersonSID int,
 @StatusLogout tinyint
AS

DECLARE @PreviousStatusLogout TINYINT

UPDATE [dbo].[BvTasks]
SET StatusLogout = @StatusLogout,
    @PreviousStatusLogout = StatusLogout
WHERE PersonSID = @PersonSID

SELECT t.InterviewID, 
       t.LoggedInToDialerState, 
       t.IsLoginRCToDialer, 
       ISNULL(s.[Name], '') AS [ProjectID], 
       @PreviousStatusLogout PreviousStatusLogout,
       t.StartTime,
       t.SurveySid,
       t.DiallingMode
 FROM BvTasks t
 LEFT JOIN BvSurvey s
 ON t.SurveySID = s.[Sid]
 WHERE t.PersonSID = @PersonSID

RETURN 0