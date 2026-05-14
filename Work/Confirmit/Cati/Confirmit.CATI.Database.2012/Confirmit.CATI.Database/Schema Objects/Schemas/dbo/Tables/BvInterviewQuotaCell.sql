CREATE TABLE [dbo].[BvInterviewQuotaCell]
(
    [SurveyID]			INT           NOT NULL,
    [InterviewId]		INT			  NOT NULL,
    [QuotaID]			INT           NOT NULL,
    [CellID]			INT			  NOT NULL,
    CONSTRAINT [PK_BvInterviewQuotaCell] PRIMARY KEY CLUSTERED ([SurveyID] ASC, [InterviewId] ASC, [QuotaID] ASC, [CellID] ASC),
    CONSTRAINT [FK_BvInterviewQuotaCell_SurveyQuotaCell] FOREIGN KEY ([SurveyID], [QuotaID], [CellID]) REFERENCES [BvSurveyQuotaCell]([SurveyID], [QuotaID], [CellID]) ON DELETE CASCADE, 
    CONSTRAINT [FK_BvInterviewQuotaCell_Interview] FOREIGN KEY ([SurveyID], [InterviewId]) REFERENCES [BvInterview]([SurveySID], [ID]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_BvInterviewQuotaCell_SurveyId_QuotaId_CellID_InterviewId] ON [dbo].[BvInterviewQuotaCell] ([SurveyID] ASC, [QuotaID] ASC, [CellID] ASC, [InterviewId] ASC)
