CREATE TABLE BvDialHistoryToInterviewHistory
(
	DialHistoryId BIGINT NOT NULL,
	InterviewHistoryId INT NOT NULL,
	StartTime DATETIME NOT NULL,
	FinishTime DATETIME NOT NULL,
	PersonId INT NOT NULL,
    CONSTRAINT PK_BvDialHistoryToInterviewHistory PRIMARY KEY ( InterviewHistoryId, DialHistoryId )
)

GO

CREATE INDEX IX_BvDialHistoryToInterviewHistory_DialHistoryId ON BvDialHistoryToInterviewHistory( DialHistoryId )
