ALTER TABLE [dbo].[BvSurvey]
	ADD CONSTRAINT [FK_BvSurvey_Schedule] 
	FOREIGN KEY (ScheduleID)
	REFERENCES BvSchedule (ScheduleID)	

