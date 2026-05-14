PRINT N'Renaming Toggle.EnableTransfer system settings to Toggle.EnableInternalTransfer ...';
GO

UPDATE BvSystemSettings SET SystemName =  'Toggle.EnableInternalTransfer', DisplayName = 'Enable internal transfer', Description = 'Enable internal call transfer functionality'
	WHERE SystemName =  'Toggle.EnableTransfer'

GO
PRINT N'Creating [dbo].[BvExternalTransferTelephoneNumberAssignment].[IX_BvExternalTransferTelephoneNumberAssignment_SurveyId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvExternalTransferTelephoneNumberAssignment_SurveyId]
    ON [dbo].[BvExternalTransferTelephoneNumberAssignment]([SurveyId] ASC);


GO
PRINT N'Creating [dbo].[BvExternalTransferTelephoneNumberAssignment].[PK_BvExternalTransferTelephoneNumberAssignment]...';


GO
CREATE UNIQUE CLUSTERED INDEX [PK_BvExternalTransferTelephoneNumberAssignment]
    ON [dbo].[BvExternalTransferTelephoneNumberAssignment]([ExternalTransferTelephoneNumberId] ASC, [SurveyId] ASC);


GO
PRINT N'Creating [dbo].[BvSpTransfer_GetExternalTargets]...';


GO
CREATE PROCEDURE [dbo].[BvSpTransfer_GetExternalTargets]
        @SurveyId INT
AS
	SELECT TelephoneNumber, Description FROM BvExternalTransferTelephoneNumber n
		INNER JOIN BvExternalTransferTelephoneNumberAssignment a ON n.Id = a.ExternalTransferTelephoneNumberId
		WHERE a.SurveyId = @SurveyId
GO
PRINT N'Creating [dbo].[BvSpTransfer_GetExternalList]...';


GO
CREATE PROCEDURE [dbo].[BvSpTransfer_GetExternalList]
AS
	SELECT e.*, 
		( 
			SELECT COUNT(*) 
				FROM BvExternalTransferTelephoneNumberAssignment a 
				WHERE e.Id = a.ExternalTransferTelephoneNumberId 
		) as Count
		FROM BvExternalTransferTelephoneNumber e
GO
PRINT N'Update complete.';

GO

GO
