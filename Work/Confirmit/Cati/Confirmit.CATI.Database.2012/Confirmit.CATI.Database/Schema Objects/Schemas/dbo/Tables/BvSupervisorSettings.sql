CREATE TABLE [dbo].[BvSupervisorSettings]
(
	 [UserName] [nvarchar](255) NOT NULL,
	 [SurveyId] [int] NULL,
	 [Settings] [nvarchar](MAX),
     [SettingType] VARCHAR(64) NOT NULL 
) 


GO

CREATE CLUSTERED INDEX [IX_BvSupervisorSettings_SettingType_UserName] ON [dbo].[BvSupervisorSettings] (
	[SettingType] ASC,
	[UserName] ASC
)

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_BvSupervisorSettings_SettingType_UserName_SurveyId] ON [dbo].[BvSupervisorSettings] ([SettingType], [UserName], [SurveyId])
