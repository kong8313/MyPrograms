CREATE TABLE [dbo].[BvReplicationTables](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SurveySid] [int] NOT NULL,
	[TableName] [nvarchar](255) NOT NULL,
	[LastVersion] [bigint] NULL,
	[PrimaryKey] [nvarchar](255) NOT NULL
);

