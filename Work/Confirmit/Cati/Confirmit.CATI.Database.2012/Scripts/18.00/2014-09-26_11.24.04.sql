PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpState_List]...';


GO
ALTER PROCEDURE [dbo].[BvSpState_List]
@ObjectID     INTEGER
AS
DECLARE @StateGroupID INTEGER

SET @StateGroupID = 0

-- if default group
IF @ObjectID = 0
BEGIN
     DECLARE @MinOrder INTEGER
     SELECT @MinOrder = MIN([Order]) FROM BvStateGroup 
     SELECT @StateGroupID = [ID] FROM BvStateGroup WHERE [Order] = @MinOrder
END
-- if @ObjectID is a SurveySID
ELSE IF EXISTS( SELECT * FROM BvSurvey WHERE SID = @ObjectID AND State <> 2 )
     SELECT @StateGroupID = StateGroupID FROM BvSurvey WHERE SID = @ObjectID
-- if bad id
ELSE IF NOT EXISTS( SELECT * FROM BvStateGroup WHERE [ID] = @ObjectID )
     RETURN -1
ELSE
    SET @StateGroupID = @ObjectID

SELECT StateID, [Name], Priority, [DA], FcdAction  FROM BvState 
     WHERE StateGroupID = @StateGroupID  ORDER BY StateID
GO
PRINT N'Altering [dbo].[BvSpState_ListByGroup]...';


GO
ALTER PROCEDURE [dbo].[BvSpState_ListByGroup]
	@StateGroupID int
AS

-- if default group
IF @StateGroupID = 0
BEGIN
     DECLARE @MinOrder INTEGER
     SELECT @MinOrder = MIN([Order]) FROM [BvStateGroup] 
     SELECT @StateGroupID = [ID] FROM [BvStateGroup] WHERE [Order] = @MinOrder
END

SELECT [StateID], [Name], [Priority], [DA], [FcdAction]  FROM [BvState] 
     WHERE [StateGroupID] = @StateGroupID  
     ORDER BY [StateID]
GO
PRINT N'Altering [dbo].[BvSpState_ListBySurvey]...';


GO
ALTER PROCEDURE [dbo].[BvSpState_ListBySurvey]
	@SurveySID int
AS

SELECT [StateID], [Name], [Priority], [DA], [FcdAction] FROM [BvState]
     WHERE [StateGroupID] = (
		SELECT [StateGroupID] FROM [BvSurvey] WHERE [SID] = @SurveySID )
     ORDER BY [StateID]
GO
PRINT N'Update complete.';


GO
