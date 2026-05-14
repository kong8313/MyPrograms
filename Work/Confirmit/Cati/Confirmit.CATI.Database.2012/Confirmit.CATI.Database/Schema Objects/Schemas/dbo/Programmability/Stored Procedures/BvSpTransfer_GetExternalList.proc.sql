CREATE PROCEDURE [dbo].[BvSpTransfer_GetExternalList]
AS
	SELECT e.*, 
		( 
			SELECT COUNT(*) 
				FROM BvExternalTransferTelephoneNumberAssignment a 
				WHERE e.Id = a.ExternalTransferTelephoneNumberId 
		) as Count
		FROM BvExternalTransferTelephoneNumber e
