GO
PRINT N'Starting rebuilding table [dbo].[BvActiveDial]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_BvActiveDial] (
    [Id]                        BIGINT         NOT NULL,
    [Type]                      TINYINT        NOT NULL,
    [DialerId]                  INT            NOT NULL,
    [DialerTelephoneNumber]     NVARCHAR (MAX) NULL,
    [RespondentTelephoneNumber] NVARCHAR (MAX) NULL,
    [StartTime]                 DATETIME       NOT NULL,
    [AnswerTime]                DATETIME       NULL,
    [InboundCallId]             NVARCHAR (MAX) NULL,
    [TransferId]                NVARCHAR (800) NULL,
    [InitialSurveyId]           INT            NOT NULL,
    [State]                     TINYINT        NOT NULL,
    [SurveyId]                  INT            NOT NULL,
    [CampaignId]                BIGINT         NOT NULL,
    [InterviewId]               INT            NOT NULL,
    [CallId]                    INT            NOT NULL,
    [MainPersonId]              INT            NOT NULL,
    [JsonTransferState]         NVARCHAR (MAX) NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_BvActiveDial1] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[BvActiveDial])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_BvActiveDial] ([Id], [Type], [DialerId], [DialerTelephoneNumber], [RespondentTelephoneNumber], [StartTime], [AnswerTime], [InboundCallId], [TransferId], [InitialSurveyId], [State], [SurveyId], [CampaignId], [InterviewId], [CallId], [MainPersonId])
        SELECT   [Id],
                 [Type],
                 [DialerId],
                 [DialerTelephoneNumber],
                 [RespondentTelephoneNumber],
                 [StartTime],
                 [AnswerTime],
                 [InboundCallId],
                 [TransferId],
                 [InitialSurveyId],
                 [State],
                 [SurveyId],
                 [CampaignId],
                 [InterviewId],
                 [CallId],
                 [MainPersonId]
        FROM     [dbo].[BvActiveDial]
        ORDER BY [Id] ASC;
    END

DROP TABLE [dbo].[BvActiveDial];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BvActiveDial]', N'BvActiveDial';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_BvActiveDial1]', N'PK_BvActiveDial', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [dbo].[BvActiveDial].[IX_BvActiveDial_CallId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvActiveDial_CallId]
    ON [dbo].[BvActiveDial]([CallId] ASC);


GO
PRINT N'Creating [dbo].[BvActiveDial].[IX_BvActiveDial_TransferId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvActiveDial_TransferId]
    ON [dbo].[BvActiveDial]([TransferId] ASC);


GO
PRINT N'Refreshing [dbo].[BvSpActiveDial_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpActiveDial_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpActiveDial_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpActiveDial_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpActiveDial_InsertOutboundBatch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpActiveDial_InsertOutboundBatch]';


GO
PRINT N'Refreshing [dbo].[BvSpActiveDial_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpActiveDial_Update]';


GO
PRINT N'Update complete.';


GO
