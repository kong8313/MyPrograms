ALTER TABLE [dbo].[BvMessageToPerson]  ADD  CONSTRAINT [FK_BvMessageToPerson_BvPerson] FOREIGN KEY([InterviewerId])
REFERENCES [dbo].[BvPerson] ([SID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BvMessageToPerson] CHECK CONSTRAINT [FK_BvMessageToPerson_BvPerson]