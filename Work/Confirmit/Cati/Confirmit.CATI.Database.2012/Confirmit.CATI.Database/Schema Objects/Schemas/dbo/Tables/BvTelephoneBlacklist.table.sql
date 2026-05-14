CREATE TABLE [dbo].[BvTelephoneBlacklist](
	[Id] [int] NOT NULL,
	[TelephoneNumber] [varchar](255) NOT NULL,
	[Type] TINYINT NOT NULL,
    [Timestamp] DATETIME2(0) CONSTRAINT [DF_BvTelephoneBlacklist_TestTemp] DEFAULT (GETUTCDATE()) NOT NULL,
    [Comment] [varchar](74) NULL,
 CONSTRAINT [PK_BvTelephoneBlacklist] PRIMARY KEY CLUSTERED
(
	[Id] ASC
))
GO

ALTER TABLE [dbo].[BvTelephoneBlacklist]
ADD CONSTRAINT DF_BvTelephoneBlacklist_Id DEFAULT NEXT VALUE FOR [dbo].[BvTelephoneBlacklistIdSequence] FOR ID