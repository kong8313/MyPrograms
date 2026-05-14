ALTER TABLE [dbo].[BvSvyScheduleRuntimeStatistics]
    ADD CONSTRAINT [PK_BvSvyScheduleRuntimeStatistics] PRIMARY KEY CLUSTERED 
	(
		[SurveyId] ASC, 
		[ShiftTypeID] ASC, 
		[ExplicitSID] ASC ) 
		WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);
