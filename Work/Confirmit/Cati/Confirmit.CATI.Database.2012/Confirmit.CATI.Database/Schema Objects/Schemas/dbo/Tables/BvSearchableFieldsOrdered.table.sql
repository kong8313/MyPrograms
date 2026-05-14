CREATE TABLE [dbo].[BvSearchableFieldsOrdered](
    [SurveyId] INT NOT NULL,
    [FieldName] NVARCHAR(128) NOT NULL,
    [IsSystem] BIT NOT NULL,
    [IsEnabled] BIT NOT NULL,
    [OrderNumber] INT NOT NULL
    CONSTRAINT FK_BvSearchableFieldsOrdered_SurveyId FOREIGN KEY (SurveyId) REFERENCES BvSurvey (SID) ON DELETE CASCADE,
    CONSTRAINT [PK_BvSearchableFieldsOrdered] PRIMARY KEY CLUSTERED
    (
        [SurveyId] ASC,
        [FieldName] ASC
    )
)
