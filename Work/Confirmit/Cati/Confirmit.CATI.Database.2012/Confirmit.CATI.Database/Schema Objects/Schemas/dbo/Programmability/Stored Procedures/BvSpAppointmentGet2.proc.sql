CREATE Procedure [dbo].[BvSpAppointmentGet2]
    @SurveySID int,
    @InterviewID int
AS
    SELECT 
        SurveySID,
        InterviewSID,
        Appt.Time,
        ExpTime,
        RespondentName,
        ID,
        State,
        ContactName
    FROM BvAppointment Appt
    WHERE 
        SurveySID = @SurveySID AND 
        InterviewSID = @InterviewID AND 
        State = 0
    return (0)