CREATE PROCEDURE BvSpGetAllAppointmentsForUser
   @PersonSID INT
AS
   SELECT a.ID, a.InterviewSID, a.ContactName, a.Time, a.ExpTime, s.Name as ProjectID, s.Description as projectName, a.TZID
   FROM BvSurvey s
   INNER JOIN BvAppointment a ON ( a.State = 1 AND --call was created
                                   a.SurveySID = s.SID )
   INNER JOIN BvSvySchedule ss ON ( ss.SurveySID = s.SID AND
                                    ss.InterviewID = a.InterviewSID AND
                                    ss.CallState > 0 AND
                                    ss.ExplicitSID = @PersonSID )
   WHERE s.State = 1 --open survey
   ORDER BY a.Time

RETURN @@ROWCOUNT