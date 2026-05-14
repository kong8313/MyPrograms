CREATE PROCEDURE [dbo].[BvSpState_ListByGroup]
	@StateGroupID int
AS

-- if default group
IF @StateGroupID = 0
BEGIN
     DECLARE @MinOrder INTEGER
     SELECT @MinOrder = MIN([Order]) FROM [BvStateGroup] 
     SELECT @StateGroupID = [ID] FROM [BvStateGroup] WHERE [Order] = @MinOrder
END

SELECT [StateID], [Name], [Priority], [DA], [FcdAction], [AaporCode]  FROM [BvState] 
     WHERE [StateGroupID] = @StateGroupID  
     ORDER BY [StateID]