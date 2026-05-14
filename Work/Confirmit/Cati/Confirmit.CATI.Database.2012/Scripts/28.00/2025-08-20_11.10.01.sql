GO
PRINT N'Creating Index [dbo].[BvHistory].[IX_BvHistory_SurveyId_InterviewId_FiredTime]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvHistory_SurveyId_InterviewId_FiredTime]
    ON [dbo].[BvHistory]([SurveyId] ASC, [InterviewId] ASC, [FiredTime] ASC);


GO
PRINT N'Update complete.';


GO
