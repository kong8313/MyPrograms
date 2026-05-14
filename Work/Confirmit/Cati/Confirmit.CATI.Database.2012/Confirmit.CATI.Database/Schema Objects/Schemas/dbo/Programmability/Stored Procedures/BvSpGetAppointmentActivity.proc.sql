CREATE PROCEDURE BvSpGetAppointmentActivity
   @batchID int,
   @top int = 100
AS
BEGIN

	DECLARE @StateGroupID INT
	SELECT @StateGroupID = MIN(ID) FROM BvStateGroup

   SET @top = ISNULL(@top, 100)
   SELECT TOP(@top) 
         aas.[ID],
         aas.[SurveySID],
         aas.[SurveyName],
         aas.[ProjectID],
         aas.[InterviewID],
         aas.[AppointmentTime],
         aas.[TZID],
         tz.[Bias],
         aas.[Resource] InterviewerName,
         aas.[Contact],
         aas.[AlertStatus],
         aas.[CallID],
         aas.[ExtendedStatus],
		 s.[Name] AS ExtendedStatusName
   FROM BvTransferArrays
   INNER JOIN BvAppointmentsAlertStatus aas ON (ItemID = aas.SurveySID)
   INNER JOIN BvTimezone tz ON (aas.TZID = tz.ID)
   INNER JOIN [BvState] s
			ON s.StateID = aas.ExtendedStatus AND s.StateGroupID = @StateGroupID
   WHERE aas.AlertStatus > 0 AND
         @batchID = BatchID
   ORDER BY aas.AppointmentTime DESC
END