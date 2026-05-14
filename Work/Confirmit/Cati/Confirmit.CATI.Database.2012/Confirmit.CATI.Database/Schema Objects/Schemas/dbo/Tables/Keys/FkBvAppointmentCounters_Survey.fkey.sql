ALTER TABLE [dbo].[BvAppointmentCounters]
    ADD CONSTRAINT [FkBvAppointmentCounters_Survey] FOREIGN KEY ([SurveySID]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE CASCADE ON UPDATE NO ACTION;

