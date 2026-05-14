
PRINT N'Altering [dbo].[BvInboundTelephoneNumber]...';
GO

ALTER TABLE [dbo].[BvInboundTelephoneNumber]
    ADD [AudioMessagesJson] NVARCHAR (MAX) NULL;
GO

PRINT N'Update complete.';
GO
