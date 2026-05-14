CREATE TABLE [dbo].[BvDialers]
(
	[Id] int NOT NULL,
	[Name] NVARCHAR(255) NOT NULL CONSTRAINT DF_BvDialers_Name DEFAULT(''),
	[ConnectionParameters] NVARCHAR(MAX),
	[ConfigurationParameters] NVARCHAR(MAX),
	[TenantId] INT NOT NULL  CONSTRAINT DF_BvDialers_TenantId DEFAULT(0),
	[DialerOperationalStateNotification] BIT NOT NULL CONSTRAINT DF_BvDialers_DialerOperationalStateNotification DEFAULT(0),
	[WhiteList] NVARCHAR(MAX) CONSTRAINT DF_BvDialers_WhiteList DEFAULT(NULL),
	[DialTypeId] TINYINT NOT NULL CONSTRAINT DF_BvDialers_DialTypeId DEFAULT(0/*Landline*/),
	[IsActive] BIT NOT NULL CONSTRAINT DF_BvDialers_IsActive DEFAULT (1), 
    [LastSelected] TIMESTAMP NULL, 
	[DialerConfigurationTypeId] INT NULL,
	[ReconnectionDuration] INT CONSTRAINT DF_BvDialers_ReconnectionDuration DEFAULT (7200000),
	[ExpectedState] INT  NOT NULL CONSTRAINT DF_BvDialers_ExpectedState DEFAULT (2/*DisconnectedAndDiactivated*/),
    [DialerInterfaceVersion] NVARCHAR(255) NOT NULL CONSTRAINT DF_BvDialers_DialerInterfaceVersion DEFAULT(''),
    [DialerDriver] NVARCHAR(255) NOT NULL CONSTRAINT DF_BvDialers_DialerDriver DEFAULT(''),
    [DialerDriverVersion] NVARCHAR(255) NOT NULL CONSTRAINT DF_BvDialers_DialerDriverVersion DEFAULT(''),
	[Features] NVARCHAR(MAX),
    CONSTRAINT PK_BvDialers_Id PRIMARY KEY ([Id])
)
