CREATE PROCEDURE [dbo].[BvSpState_List]
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

SELECT StateID, [Name], Priority, [DA], FcdAction, [AaporCode]  FROM BvState 
     WHERE StateGroupID = @StateGroupID  ORDER BY StateID