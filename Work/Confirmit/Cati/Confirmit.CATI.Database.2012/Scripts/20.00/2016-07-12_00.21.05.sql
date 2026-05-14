PRINT N'Creating [dbo].[BvAppointmentsAlertStatus].[IX_BvAppointmentsAlertStatus_ExtendedStatus]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvAppointmentsAlertStatus_ExtendedStatus]
    ON [dbo].[BvAppointmentsAlertStatus]([ExtendedStatus] ASC);


GO
PRINT N'Altering [dbo].[BvSpGetAppointmentActivity]...';


GO
ALTER PROCEDURE BvSpGetAppointmentActivity
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
GO
PRINT N'Creating [dbo].[BvSpGetAppointmentActivityExtStatuses]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetAppointmentActivityExtStatuses]
AS
BEGIN

	DECLARE @StateGroupID INT
	SELECT @StateGroupID = MIN(ID) FROM BvStateGroup
	
	;WITH Statuses AS
	(
		SELECT DISTINCT
			ExtendedStatus
		FROM BvAppointmentsAlertStatus 
	)
	SELECT 
		s.StateID AS ExtendedStatusId,
		s.Name AS ExtendedStatusName
        FROM Statuses
  	INNER JOIN [BvState] s
		ON s.StateID = ExtendedStatus AND s.StateGroupID = @StateGroupID
	ORDER BY ExtendedStatus 
END
GO
PRINT N'Update complete.';


GO
