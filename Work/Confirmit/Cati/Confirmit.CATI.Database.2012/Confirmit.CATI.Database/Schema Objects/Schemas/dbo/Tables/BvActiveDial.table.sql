CREATE TABLE BvActiveDial
(
	Id BIGINT NOT NULL,
	Type TINYINT NOT NULL,
	DialerId INT NOT NULL,
	DialerTelephoneNumber NVARCHAR(MAX),
	RespondentTelephoneNumber NVARCHAR(MAX),
	StartTime DATETIME NOT NULL,
	AnswerTime DATETIME NULL,
	InboundCallId NVARCHAR(MAX),
	TransferId NVARCHAR(800),
	InitialSurveyId INT NOT NULL,
	State TINYINT NOT NULL,
	SurveyId INT NOT NULL,
	CampaignId BIGINT NOT NULL,
	InterviewId INT NOT NULL,
	CallId INT NOT NULL,
	MainPersonId INT NOT NULL,
    JsonTransferState NVARCHAR(MAX) NULL,
    TransferType TINYINT NULL,
	JsonCallOutcomeMetadata NVARCHAR(MAX) NULL,
	RingTime INT NULL, 
	DialerCallerId NVARCHAR(255) NULL,
	DialerCallOutcome INT  NULL
	CONSTRAINT PK_BvActiveDial PRIMARY KEY ( ID )
)
GO

CREATE INDEX IX_BvActiveDial_CallId ON BvActiveDial(CallId)
GO
CREATE INDEX IX_BvActiveDial_TransferId ON BvActiveDial(TransferId)
GO
