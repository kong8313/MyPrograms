ALTER TABLE [dbo].[BvMessageToPerson]  ADD  CONSTRAINT [FK_BvMessageToPerson_BvMessages] FOREIGN KEY([MessageId])
REFERENCES [dbo].[BvMessages] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BvMessageToPerson] CHECK CONSTRAINT [FK_BvMessageToPerson_BvMessages]
