CREATE TABLE [dbo].[BvExternalTransferTelephoneNumber]
(
    [Id] INT NOT NULL IDENTITY(1,1) CONSTRAINT PK_BvExternalTransferTelephoneNumber PRIMARY KEY,
	[TelephoneNumber] NVARCHAR(256),
	[Description] NVARCHAR(256),
	[Hidden] BIT NOT NULL CONSTRAINT DF_BvExternalTransferTelephoneNumber_Hidden DEFAULT(0)
)

