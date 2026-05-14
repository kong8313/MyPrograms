CREATE TABLE [dbo].[BvInboundTelephoneNumber]
(
	[TelephoneNumber] NVARCHAR(256) CONSTRAINT PK_BvInboundTelephoneNumber PRIMARY KEY,
	[DialerId] INT NOT NULL,
	[SurveyId] INT NULL, 
	[AudioMessagesJson] NVARCHAR(MAX) NULL,
	CONSTRAINT fk_BvInboundTelephoneNumberBvSurvey
    FOREIGN KEY (SurveyId)
    REFERENCES BvSurvey (SID)
	ON DELETE SET NULL
)

