CREATE PROCEDURE BvSpAlert_RecalculateAppointment
	@AppointmentAlert_ShortInterval INT,
	@AppointmentAlert_LongInterval INT,
	@defaultTimeZone INT
AS
   DECLARE @Now DATETIME = GETUTCDATE()

   DECLARE @Red INT
   DECLARE @Amber INT 

   SELECT @Red = Red, @Amber = Amber
   FROM BvThresholds
   WHERE ObjectSID = 0 AND
         ThresholdsTypeID = 15

   DECLARE @StartDate DATETIME

   SET @StartDate = DATEADD(millisecond, -DATEPART(millisecond, @Now), @Now)
   SET @StartDate = DATEADD(second, -DATEPART(second, @StartDate), @StartDate)
   SET @StartDate = DATEADD(minute, -DATEPART(minute, @StartDate), @StartDate)
   SET @StartDate = DATEADD(Hour, -DATEPART(hour, @StartDate), @StartDate)

   DECLARE @ShortIntervalStart DATETIME = @Now
   DECLARE @ShortIntervalFinish DATETIME = DateAdd(second, @AppointmentAlert_ShortInterval, @Now)

   DECLARE @LongIntervalStart DATETIME = (CASE WHEN @AppointmentAlert_LongInterval >= 0
                                                THEN @Now
                                                ELSE @StartDate
                                                END)
   DECLARE @LongIntervalFinish DATETIME = (CASE WHEN @AppointmentAlert_LongInterval >= 0
                                                THEN DateAdd(hour, @AppointmentAlert_LongInterval, @Now)
                                                ELSE DateAdd(day, -@AppointmentAlert_LongInterval, @StartDate)
                                                END)
   ----------------------BvAppointmentCounters----------------------
   UPDATE BvAppointmentCounters
   SET CountForShortInterval = (SELECT COUNT(*)
                                FROM BvAppointment a
                                WHERE a.State = 1 AND/*with call*/
                                      a.SurveySID = BvAppointmentCounters.SurveySID AND
                                      a.Time BETWEEN @ShortIntervalStart AND @ShortIntervalFinish),
       CountForLongInterval = (SELECT COUNT(*)
                               FROM BvAppointment a
                               WHERE a.State = 1 AND/*with call*/
                                     a.SurveySID = BvAppointmentCounters.SurveySID AND
                                     a.Time between @LongIntervalStart AND @LongIntervalFinish )
   ----------------------BvAppointmentsAlertStatus----------------------
   TRUNCATE TABLE BvAppointmentsAlertStatus
  
   INSERT INTO BvAppointmentsAlertStatus( 
     [ID],
     [SurveySID],
     [SurveyName],
     [ProjectID],
     [InterviewID],
     [AppointmentTime],
     [TZID],
     [Resource],
     [Contact],
     [AlertStatus],
     [CallID],
     [ExtendedStatus])
   SELECT a.ID,
          a.SurveySID,
          s.Description,
          s.Name,
          a.InterviewSID,
          a.Time,
          ISNULL(a.TZID, @defaultTimeZone),
          NULL,
          a.ContactName,
          a.AlertStatus,
          0,
          0
   FROM BvSurvey s 
   CROSS APPLY GetSurveyAlertAppointments( s.SID, 100, @Amber, @Red, @Now ) a
   WHERE s.State = 1 
   
   UPDATE BvAppointmentsAlertStatus
                SET [Resource] = pag.Name,
                       [CallID] = ISNULL( ss.ID, 0 ),
                       [ExtendedStatus] = i.TransientState
                FROM BvInterview i 
                LEFT JOIN BvSvySchedule ss ON  ss.SurveySID = i.SurveySID AND ss.InterviewID = i.ID
                LEFT JOIN BvViewPersonAndGroup pag ON(ss.ExplicitType = 2 AND 
                                                       pag.SID = ss.ExplicitSID)
                WHERE  BvAppointmentsAlertStatus.SurveySID = i.SurveySID AND
                                               BvAppointmentsAlertStatus.[InterviewID] = i.ID
