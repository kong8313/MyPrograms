
GO
PRINT N'Dropping [dbo].[BvInboundTelephoneNumber].[IX_BvInboundTelephoneNumber_SurveyId]...';


GO
DROP INDEX [IX_BvInboundTelephoneNumber_SurveyId]
    ON [dbo].[BvInboundTelephoneNumber];


GO
PRINT N'Altering [dbo].[BvInboundTelephoneNumber]...';


GO
ALTER TABLE [dbo].[BvInboundTelephoneNumber] ALTER COLUMN [SurveyId] INT NULL;


GO
PRINT N'Creating [dbo].[BvInboundTelephoneNumber].[IX_BvInboundTelephoneNumber_SurveyId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvInboundTelephoneNumber_SurveyId]
    ON [dbo].[BvInboundTelephoneNumber]([SurveyId] ASC);


GO
PRINT N'Remove not existed survey id from BvInboundTelephoneNumber table'


GO
UPDATE [dbo].[BvInboundTelephoneNumber]
    SET SurveyId = NULL
FROM [dbo].[BvInboundTelephoneNumber] itn
LEFT JOIN BvSurvey s
    ON s.SID = itn.SurveyId 
WHERE s.SID IS NULL


GO
PRINT N'Creating [dbo].[fk_BvInboundTelephoneNumberBvSurvey]...';


GO
ALTER TABLE [dbo].[BvInboundTelephoneNumber] WITH NOCHECK
    ADD CONSTRAINT [fk_BvInboundTelephoneNumberBvSurvey] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE SET NULL;


GO
PRINT N'Checking existing data against newly created constraints';


GO



GO
ALTER TABLE [dbo].[BvInboundTelephoneNumber] WITH CHECK CHECK CONSTRAINT [fk_BvInboundTelephoneNumberBvSurvey];


GO
PRINT N'Update complete.';


GO
