CREATE TABLE BvDialHistory
(
	ID INT NOT NULL,
	Type TINYINT NOT NULL,
	DialerId INT NOT NULL,
	InitialSurveyId INT NOT NULL,
	DialerTelephoneNumber NVARCHAR(MAX),
	RespondentTelephoneNumber NVARCHAR(MAX),
	InboundCallId NVARCHAR(MAX),
	CallCompleteStatus TINYINT NOT NULL,
	StartTime DATETIME NOT NULL,
	AnswerTime DATETIME NULL,
	FinishTime DATETIME NOT NULL,
	JsonCallOutcomeMetadata NVARCHAR(MAX) NULL,
	RingTime INT NULL,
	DialerCallerId NVARCHAR(255) NULL,
	DialerCallOutcome INT NULL,
	CONSTRAINT PK_BvDialHistory PRIMARY KEY ( ID )
)
