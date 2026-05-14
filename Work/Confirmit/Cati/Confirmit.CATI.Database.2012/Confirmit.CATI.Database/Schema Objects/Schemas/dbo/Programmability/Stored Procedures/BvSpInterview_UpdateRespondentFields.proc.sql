CREATE PROCEDURE [dbo].[BvSpInterview_UpdateRespondentFields]
    @projectId NVARCHAR(64),
    @respId INT,
    @TelephoneNumber NVARCHAR(255),
    @RespondentName NVARCHAR(255),
    @ExtensionNumber NVARCHAR(255),
    @TimeZoneId INT,
    @DialTypeId TINYINT = NULL
AS

    DECLARE @SurveySID INT
    SELECT @SurveySID = SID FROM BvSurvey WHERE Name = @projectId
    IF @SurveySID IS NULL 
    BEGIN
        --RAISERROR( 'survey with projectID = ''%d'' not found', 16, 1, @projectId )
        RETURN (0)
    END

	if @DialTypeId IS NOT NULL
	BEGIN
		SET @DialTypeId = CASE WHEN @DialTypeId = 1 THEN 1 ELSE 0 END

        UPDATE BvInterview
            SET TelephoneNumber = @TelephoneNumber,
                RespondentName = @RespondentName,
                ExtensionNumber = @ExtensionNumber,
                TimezoneId = ISNULL( @TimeZoneId, TimezoneId ),
				DialTypeId = @DialTypeId
        WHERE ID = @respId AND
              SurveySID = @SurveySID

        UPDATE BvSvySchedule
            SET DialTypeId = @DialTypeId
        WHERE InterviewID = @respId AND
              SurveySID = @SurveySID
	END
	ELSE
	BEGIN
    UPDATE BvInterview
        SET TelephoneNumber = @TelephoneNumber,
            RespondentName = @RespondentName,
            ExtensionNumber = @ExtensionNumber,
            TimezoneId = ISNULL( @TimeZoneId, TimezoneId )
    WHERE ID = @respId AND
          SurveySID = @SurveySID
	END
        
	IF @TimeZoneId IS NOT NULL AND @TimeZoneId <> 0
	BEGIN
		UPDATE BvAppointment
		SET TZID = @TimeZoneId
		WHERE SurveySID = @SurveySID AND
			InterviewSID = @respId
	END
