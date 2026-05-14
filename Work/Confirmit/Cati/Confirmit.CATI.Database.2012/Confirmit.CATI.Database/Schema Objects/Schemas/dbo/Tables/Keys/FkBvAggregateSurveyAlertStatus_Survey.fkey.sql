ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [FkBvAggregateSurveyAlertStatus_Survey] FOREIGN KEY ([SID]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE CASCADE ON UPDATE NO ACTION;

