GO
PRINT N'Altering [dbo].[BvSpInterview_UpdateRespondentFields]...';


GO
ALTER PROCEDURE [dbo].[BvSpInterview_UpdateRespondentFields]
    @projectId NVARCHAR(64),
    @respId INT,
    @TelephoneNumber NVARCHAR(255),
    @RespondentName NVARCHAR(255),
    @ExtensionNumber NVARCHAR(255),
    @TimeZoneId INT
AS

    DECLARE @SurveySID INT
    SELECT @SurveySID = SID FROM BvSurvey WHERE Name = @projectId
    IF @SurveySID IS NULL 
    BEGIN
        --RAISERROR( 'survey with projectID = ''%d'' not found', 16, 1, @projectId )
        RETURN (0)
    END

    UPDATE BvInterview
        SET TelephoneNumber = @TelephoneNumber,
            RespondentName = @RespondentName,
            ExtensionNumber = @ExtensionNumber,
            TimezoneId = ISNULL( @TimeZoneId, TimezoneId )
    WHERE ID = @respId AND
          SurveySID = @SurveySID
        
	IF @TimeZoneId IS NOT NULL AND @TimeZoneId <> 0
	BEGIN
		UPDATE BvAppointment
		SET TZID = @TimeZoneId
		WHERE SurveySID = @SurveySID AND
			InterviewSID = @respId
	END
GO
PRINT N'Update complete.';


GO
