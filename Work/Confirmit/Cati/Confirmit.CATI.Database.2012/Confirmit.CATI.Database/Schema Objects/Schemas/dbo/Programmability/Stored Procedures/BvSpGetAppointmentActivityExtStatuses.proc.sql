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