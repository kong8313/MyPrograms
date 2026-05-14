ALTER TABLE [dbo].[BvUserSurveyPermission]
    ADD CONSTRAINT [FkBvUserSurveyPermission_Survey] FOREIGN KEY ([SurveySID]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE CASCADE ON UPDATE NO ACTION;

