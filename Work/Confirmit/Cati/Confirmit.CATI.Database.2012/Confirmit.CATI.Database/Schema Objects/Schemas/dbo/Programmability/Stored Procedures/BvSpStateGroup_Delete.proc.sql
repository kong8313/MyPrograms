CREATE PROCEDURE [dbo].[BvSpStateGroup_Delete]
@ObjectSID INTEGER
AS
DECLARE @Deleted INTEGER
DECLARE @MinOrder INTEGER
DECLARE @Order       INTEGER
DECLARE @GroupName NVARCHAR(MAX)
DECLARE @SurveyName NVARCHAR(MAX)
 
     -- Dont delete state group if it default group
     SELECT @MinOrder   = MIN( [Order] ) FROM BvStateGroup
     SELECT @Order = [Order] FROM BvStateGroup WHERE [ID] = @ObjectSID

     IF @MinOrder = @Order
     BEGIN
         RAISERROR( 'Could not delete default state group.', 12, 1)
         RETURN -1
     END

     -- Dont delete state group if link exist
     IF EXISTS( SELECT * FROM BvSurvey WHERE StateGroupID = @ObjectSID AND State <> 2 )
     BEGIN
		SELECT @GroupName = Name FROM BvStateGroup WHERE [ID] = @ObjectSID
		SELECT TOP(1) @SurveyName = name FROM BvSurvey WHERE StateGroupID = @ObjectSID AND State <> 2
		
        RAISERROR( 'The state group "%s" can not be deleted because survey "%s" references it.', 12, 1, @GroupName, @SurveyName )
		RETURN( -1 )
     END

DECLARE @DefaultStateGroupID INTEGER
SELECT top(1) @DefaultStateGroupID = ID 
FROM BvStateGroup 
ORDER BY [Order] ASC;

	 IF EXISTS( SELECT * FROM BvSurvey WHERE StateGroupID = @ObjectSID AND State = 2 )
	 BEGIN
		UPDATE BvSurvey 
		SET StateGroupID = @DefaultStateGroupID 
		WHERE StateGroupID = @ObjectSID AND State = 2
	 END

     EXEC BvSpMembership_Delete 0, @ObjectSID
     DELETE FROM BvStateGroup WHERE [ID] = @ObjectSID
     DELETE FROM BvState WHERE StateGroupID = @ObjectSID

RETURN (0)