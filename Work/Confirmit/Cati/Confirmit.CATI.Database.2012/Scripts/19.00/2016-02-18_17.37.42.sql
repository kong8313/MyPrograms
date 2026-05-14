PRINT N'Creating [dbo].[BvSpTasks_UpdateNewSurveySid]...';
GO

CREATE PROCEDURE [dbo].[BvSpTasks_UpdateNewSurveySid]
 @PersonSID int,
 @NewSurveySID int
AS

UPDATE [dbo].[BvTasks]
    SET NewSurveySID = @NewSurveySID
WHERE PersonSID = @PersonSID

RETURN 0
GO

PRINT N'Creating [dbo].[BvSpTasks_UpdateSurveySid]...';
GO

CREATE PROCEDURE [dbo].[BvSpTasks_UpdateSurveySid]
 @PersonSID int,
 @SurveySID int
AS

UPDATE [dbo].[BvTasks]
    SET SurveySID = @SurveySID
WHERE PersonSID = @PersonSID

RETURN 0
GO

PRINT N'Update complete.';
GO
