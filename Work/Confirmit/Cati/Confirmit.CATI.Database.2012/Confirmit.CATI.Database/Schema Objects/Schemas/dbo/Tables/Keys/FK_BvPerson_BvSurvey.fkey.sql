ALTER TABLE [BvPerson] ADD CONSTRAINT [FK_BvPerson_BvSurvey] FOREIGN KEY ([AutomaticSurveyID]) 
        REFERENCES [BvSurvey] ([SID])
        ON DELETE SET NULL
