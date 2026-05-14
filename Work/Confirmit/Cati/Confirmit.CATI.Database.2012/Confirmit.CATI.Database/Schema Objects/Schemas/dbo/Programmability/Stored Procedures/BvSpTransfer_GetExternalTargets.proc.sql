CREATE PROCEDURE [dbo].[BvSpTransfer_GetExternalTargets]
        @SurveyId INT
AS
	SELECT TelephoneNumber, Description, Hidden FROM BvExternalTransferTelephoneNumber n
		INNER JOIN BvExternalTransferTelephoneNumberAssignment a ON n.Id = a.ExternalTransferTelephoneNumberId
		WHERE a.SurveyId = @SurveyId
