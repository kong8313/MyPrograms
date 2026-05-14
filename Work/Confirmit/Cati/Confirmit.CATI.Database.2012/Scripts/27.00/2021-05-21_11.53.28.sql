GO
PRINT N'Altering [dbo].[BvExternalTransferTelephoneNumber]...';


GO
ALTER TABLE [dbo].[BvExternalTransferTelephoneNumber]
    ADD [Hidden] BIT CONSTRAINT [DF_BvExternalTransferTelephoneNumber_Hidden] DEFAULT (0) NOT NULL;


GO
PRINT N'Refreshing [dbo].[BvSpTransfer_GetExternalList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTransfer_GetExternalList]';


GO
PRINT N'Altering [dbo].[BvSpTransfer_GetExternalTargets]...';


GO
ALTER PROCEDURE [dbo].[BvSpTransfer_GetExternalTargets]
        @SurveyId INT
AS
	SELECT TelephoneNumber, Description, Hidden FROM BvExternalTransferTelephoneNumber n
		INNER JOIN BvExternalTransferTelephoneNumberAssignment a ON n.Id = a.ExternalTransferTelephoneNumberId
		WHERE a.SurveyId = @SurveyId

GO
PRINT N'Update complete.';


GO