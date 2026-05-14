GO
PRINT N'Creating [dbo].[BvSpCall_GetNewInbound]...';


GO
CREATE PROCEDURE [dbo].[BvSpCall_GetFreshSampleWithEmptyTelNumber]
    @SurveyID int
AS
	DECLARE @Call TABLE( 
	CallId INT,
	ApptId INT,
	SurveySID INT,
	IID INT,
	CallState INT,
	ShiftID INT,
	Priority INT,
	TimeInShift DATETIME,
	TimeToExpire DATETIME,
	Resource INT,
	Resource_Type INT,
	OldPriority INT,
	RuleNumber UNIQUEIDENTIFIER,
	ConditionValue INT,
	CellId INT,
	DialTypeId TINYINT,
	Type TINYINT)

	;WITH Call AS 
	( 
		SELECT TOP(1) c.* FROM BvSvySchedule c
			INNER JOIN BvInterview i ON c.SurveySID = i.SurveySID AND c.InterviewID = i.ID
			WHERE c.CallState = 2 AND c.SurveySID = @SurveyId AND i.TransientState = 16/*FreshSample*/ AND i.TelephoneNumber = ''
	)
	UPDATE call SET CallState = -1 
		OUTPUT inserted.[ID] callid,
		inserted.ApptID,
		inserted.SurveySID,
		inserted.InterviewID,
		inserted.CallState,
		ISNULL( sz.[ShiftTypeID], inserted.[ShiftTypeID] ) ShiftID,
		inserted.Priority,
		inserted.TimeInShift,
		inserted.ExpireTime TimeToExpire,
		CASE WHEN inserted.ExplicitType = 2 THEN inserted.ExplicitSID ELSE 0 END AS Resource,
		inserted.ExplicitType Resource_Type,
		inserted.OldPriority,
		inserted.RuleNumber,
		inserted.ConditionValue,
		inserted.CellId,
		inserted.DialTypeId,
		inserted.Type
		INTO @Call
	FROM call 
		LEFT JOIN BvShiftZones sz ON call.ShiftTypeID = sz.[ID]
					
	SELECT * FROM @Call
GO
PRINT N'Update complete.';


GO
