PRINT N'Altering [dbo].[BvSpAppointmentUpdate]...';


GO
ALTER PROCEDURE [dbo].[BvSpAppointmentUpdate]
        @apptID         INT,
        @surveySID      INT,
        @interviewID    INT,
        @time           DATETIME,
        @expired        DATETIME,
        @contact        NVARCHAR( 255 ),
        @state          INT,
        @TZID           INT
AS
    SET NOCOUNT ON

    IF @apptID = 0
    BEGIN
        INSERT INTO BvAppointment
        (
            SurveySID, 
            InterviewSID, 
            Time, 
            ExpTime,
            State, 
            ContactName,
            TZID
        )
        VALUES
        (
            @surveySID, 
            @interviewID, 
            @time, 
            @expired,
            0, 
            @contact,
            @TZID
        )
        SET @apptID = @@IDENTITY
    END
    ELSE
    BEGIN
        UPDATE BvAppointment SET
            SurveySID = @surveySID,
            InterviewSID = @interviewID,
            Time = @time, 
            ExpTime = @expired,
            ContactName = @contact,
            State = @state,
            TZID = @TZID
        WHERE [ID] = @apptID
    END
    RETURN (@apptID)
GO
PRINT N'Update complete.';


GO
