CREATE TABLE BvAnswerSubmissionAlertHistory
(
	PersonId			INT,
	SubmissionTime		DATETIME,
	QuestionId			NVARCHAR(256), 
	SurveyId			INT, 
	InterviewId			INT, 
	AnswerDuration		INT, /*in seconds there is should no be int overflow because auto logout is performed every 2 hour. so max duration will be 2*60*60*/
	AnswerSubmissionAlert		BIT NULL,
	QuickAnswerSubmissionAlert	BIT NULL,
	InterviewState		TINYINT NOT NULL
)
