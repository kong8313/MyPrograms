CREATE TABLE [dbo].[BvIvrSettings]
(
	[LanguageId] INT NOT NULL CONSTRAINT PK_BvIvrSettings_LanguageId PRIMARY KEY,
	[LanguageDescription] NVARCHAR(255) NOT NULL,
	[WrongInputAudioUrl] NVARCHAR(255) NOT NULL,
	[WrongInputText] NVARCHAR(255) NOT NULL,
	[WrongInputExitAudioUrl] NVARCHAR(255) NOT NULL,
	[WrongInputExitText] NVARCHAR(255) NOT NULL
)