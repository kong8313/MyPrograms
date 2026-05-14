CREATE PROCEDURE [dbo].[BvSpTasks_UpdateProblemState]
 @PersonSID int,
 @ProblemId int
AS

UPDATE [dbo].[BvTasks]
    SET ProblemId = @ProblemId
WHERE PersonSID = @PersonSID

RETURN 0