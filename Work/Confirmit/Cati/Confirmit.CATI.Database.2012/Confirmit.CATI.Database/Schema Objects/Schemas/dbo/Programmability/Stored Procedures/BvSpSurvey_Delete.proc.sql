CREATE PROCEDURE [dbo].[BvSpSurvey_Delete]
        @surveyID int
AS
    DECLARE @State INTEGER

	IF EXISTS( SELECT 1 FROM BvTasks WHERE SurveySID = @surveyID )
	BEGIN
		DECLARE @Name NVARCHAR(MAX) 
		SELECT @Name = name FROM BvSurvey WHERE SID = @surveyID
		RAISERROR( 'Survey with name = ''%s'' can''t be deleted, because active users exist for it survey', 16, 1, @name )
		RETURN -1
	END

    DELETE FROM BvThresholdITS WHERE SurveySID = @surveyID

    DELETE FROM BvMembership WITH(ROWLOCK)
    WHERE ObjectSID = @surveyID
    
    DELETE BvAppointment 
    WHERE SurveySID = @surveyID
    
    DELETE FROM BvSvySchedule 
    WHERE SurveySID = @surveyID

    DELETE BvPersonOrGroupAssignmentOnSurvey WHERE SurveyId = @surveyID 

	DELETE BvSurveyAssignmentOnCallCenter WHERE SurveyId = @surveyID 

    DELETE BvInterview WHERE SurveySID = @surveyID
    
    EXEC BvSpMembership_Delete 0, @surveyID
    
    --delete specific survey schedule params
    DELETE FROM BvScheduleParam WHERE SurveySID = @surveyID

    DELETE  BvSurvey WHERE SID = @surveyID
    DELETE FROM BvSampleStatusSummary WHERE SurveySID = @surveyID
    
    DECLARE @FilterSID INTEGER
    SELECT @FilterSID = SID FROM BvFilters WHERE [Name] = CAST( @surveyID AS NVARCHAR(255) )
    IF @FilterSID IS NOT NULL
    BEGIN
        DELETE FROM BvFilterFields WHERE FilterSID = @FilterSID
        DELETE FROM BvFilters WHERE SID = @FilterSID
    END
    
    DELETE FROM BvFilterFields
    FROM BvFilterFields
    INNER JOIN BvFilters ON ( SID = FilterSid )
    WHERE SurveySID = @surveyID

    DELETE FROM BvFilters WHERE SurveySID = @surveyID
    
    delete from bvpersonrel where type = 2 and objectsid = @surveyID
    
    delete from bvlogingroup where surveysid = @surveyID

RETURN (0)
