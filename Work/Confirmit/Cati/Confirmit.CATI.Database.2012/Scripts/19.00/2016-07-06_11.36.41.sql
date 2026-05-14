PRINT N'Creating [dbo].[BvSupervisorSettings]...';


GO
CREATE TABLE [dbo].[BvSupervisorSettings]
(
	 [UserName] [nvarchar](255) NOT NULL,
	 [SurveyId] [int] NOT NULL,
	 [Settings] [nvarchar](MAX),
	 CONSTRAINT [PK_BvSupervisorSettings_UserName_SurveyId] PRIMARY KEY CLUSTERED 
	(
		 [UserName] ASC,
		 [SurveyId] ASC
	)
) 

GO
PRINT N'Update complete.';


GO
