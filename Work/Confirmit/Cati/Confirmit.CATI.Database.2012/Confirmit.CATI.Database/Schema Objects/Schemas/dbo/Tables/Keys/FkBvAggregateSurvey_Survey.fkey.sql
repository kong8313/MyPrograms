ALTER TABLE [dbo].[BvAggregateSurvey]
    ADD CONSTRAINT [FkBvAggregateSurvey_Survey] FOREIGN KEY ([SID]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE CASCADE ON UPDATE NO ACTION;

