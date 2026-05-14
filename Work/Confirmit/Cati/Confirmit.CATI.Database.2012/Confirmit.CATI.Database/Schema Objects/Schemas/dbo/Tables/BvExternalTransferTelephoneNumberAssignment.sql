CREATE TABLE [dbo].[BvExternalTransferTelephoneNumberAssignment]
(
    [ExternalTransferTelephoneNumberId] INT NOT NULL,
    [SurveyId] INT NOT NULL,
	CONSTRAINT FK_BvExternalTransferTelephoneNumberAssignment_ExternalTransferTelephoneNumberId FOREIGN KEY (ExternalTransferTelephoneNumberId) REFERENCES BvExternalTransferTelephoneNumber (Id) ON DELETE CASCADE,
    CONSTRAINT FK_BvExternalTransferTelephoneNumberAssignment_SurveyId FOREIGN KEY (SurveyId) REFERENCES BvSurvey (SID) ON DELETE CASCADE
)

GO

CREATE UNIQUE CLUSTERED INDEX [PK_BvExternalTransferTelephoneNumberAssignment] ON [dbo].[BvExternalTransferTelephoneNumberAssignment]( [ExternalTransferTelephoneNumberId],[SurveyId])

GO

CREATE NONCLUSTERED INDEX [IX_BvExternalTransferTelephoneNumberAssignment_SurveyId] ON [dbo].[BvExternalTransferTelephoneNumberAssignment]([SurveyId])
