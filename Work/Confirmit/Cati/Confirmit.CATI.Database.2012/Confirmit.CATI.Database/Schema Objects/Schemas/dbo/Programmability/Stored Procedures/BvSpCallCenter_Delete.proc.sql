CREATE PROCEDURE [dbo].[BvSpCallCenter_Delete]
	@CallCenterID INT,
	@DescCallCenterID INT,
	@PersonAction INT
AS
	DECLARE @IsCanBeDeleted BIT = ( SELECT CanBeDeleted FROM BvCallCenter WHERE ID = @CallCenterID )

	IF ISNULL( @IsCanBeDeleted, 1 ) = 1 
	BEGIN
		RAISERROR( 'Call center with ID = %d can''t be deleted, because call center doesn''t exists or is marked as can''t be deleted', 12, 1, @CallCenterID )
		RETURN (0)
	END

	DECLARE @Surveys TABLE( SurveyId INT )	

	DELETE BvSurveyAssignmentOnCallCenter 
		OUTPUT deleted.SurveyId INTO @Surveys
		WHERE CallCenterId = @CallCenterID

	INSERT INTO BvSurveyAssignmentOnCallCenter( SurveyId, CallCenterId )
		SELECT s.SurveyId, @DescCallCenterID FROM @Surveys s
		LEFT JOIN BvSurveyAssignmentOnCallCenter a
			ON s.SurveyId = a.SurveyId AND a.CallCenterId = @DescCallCenterID
		WHERE a.CallCenterId IS NULL
	
	UPDATE BvSupervisorAssignment SET CallCenterId = @DescCallCenterID WHERE CallCenterID = @CallCenterID

	DECLARE @PersonId INT

	IF @PersonAction = 0 --delete
	BEGIN
		
		DECLARE crPerson CURSOR FOR 
			SELECT SID FROM BvPerson WHERE CallCenterID = @CallCenterID
		
		OPEN crPerson
		FETCH NEXT FROM crPerson INTO @PersonId
		
		WHILE ( @@FETCH_STATUS = 0 ) 
		BEGIN
			EXEC BvSpPerson_Delete @PersonId
			FETCH NEXT FROM crPerson INTO @PersonId
		END

		CLOSE crPerson
		DEALLOCATE crPerson
	END
	ELSE IF @PersonAction = 1
	BEGIN
		DECLARE @Persons TABLE( SID INT )

		UPDATE BvPerson 
			SET CallCenterID = @DescCallCenterID
			OUTPUT deleted.SID INTO @Persons
			WHERE CallCenterID = @CallCenterID

		DELETE FROM BvPersonOrGroupAssignmentOnSurvey
			WHERE CallCenterID = @CallCenterID
		
		DECLARE crPerson CURSOR FOR 
			SELECT SID FROM @Persons
		
		OPEN crPerson
		FETCH NEXT FROM crPerson INTO @PersonId
		
		WHILE ( @@FETCH_STATUS = 0 ) 
		BEGIN
			EXEC BvSpPerson_SpinUp @PersonId
			FETCH NEXT FROM crPerson INTO @PersonId
		END

		CLOSE crPerson
		DEALLOCATE crPerson
	END
	ELSE
	BEGIN
		RAISERROR( 'Call center with ID = %d can''t be deleted, because wrong PersonAction = %d.', 12, 1, @PersonAction )
		RETURN 0
	END

	DELETE BvPersonOrGroupAssignmentOnSurvey WHERE CallCenterID = @CallCenterID
	DELETE BvCallCenter WHERE ID = @CallCenterID

	RETURN(0)
