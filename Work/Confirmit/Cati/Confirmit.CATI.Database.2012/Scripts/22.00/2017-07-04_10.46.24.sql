ALTER TABLE BvSupervisorSettings Drop Constraint PK_BvSupervisorSettings_UserName_SurveyId
ALTER TABLE BvSupervisorSettings ALTER COLUMN SurveyId INT NULL
ALTER TABLE BvSupervisorSettings ADD SettingType VARCHAR(64) NULL
GO
UPDATE BvSupervisorSettings set SettingType = 'Quota_Columns'
ALTER TABLE BvSupervisorSettings ALTER COLUMN SettingType VARCHAR(64) NOT NULL
GO
CREATE CLUSTERED INDEX [IX_BvSupervisorSettings_SettingType_UserName] ON [dbo].[BvSupervisorSettings]
(
	[SettingType] ASC,
	[UserName] ASC
)

CREATE UNIQUE NONCLUSTERED INDEX [IX_BvSupervisorSettings_SettingType_UserName_SurveyId] ON [dbo].[BvSupervisorSettings] ([SettingType], [UserName], [SurveyId])

GO
PRINT N'Update complete.';