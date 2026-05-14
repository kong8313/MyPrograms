CREATE TABLE [dbo].[BvSearchableFields](
	[SurveyId] [int] NOT NULL,
	[ColumnId] [int] NOT NULL,
	[TableId] [int] NOT NULL
 CONSTRAINT [PK_BvSearchableFields] PRIMARY KEY CLUSTERED 
(
	[SurveyId] ASC,
	[ColumnId] ASC,
	[TableId] ASC
))