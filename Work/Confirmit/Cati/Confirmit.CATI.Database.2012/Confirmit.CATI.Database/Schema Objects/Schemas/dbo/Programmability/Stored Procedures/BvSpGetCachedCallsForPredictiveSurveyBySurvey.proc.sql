CREATE PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]
    @SurveySID INT,
    @DialerId INT = 1,
	@Count  INT,  --number of requested calls
	@SuitableTimeForCalls DATETIME,
	@DialType INT = 0
AS
	DECLARE @Groups TABLE(
		[ObjectSid] [int] NOT NULL,
		[GroupSize] [int] NOT NULL)
		
    DECLARE @MinDistributedCalls INT = 5
	
	IF ( (SELECT COUNT(*) FROM BvDialers ) > 1 )
	BEGIN
		;WITH Logins AS (
			select lg.ObjectSID as sid, lg.SurveySID, count(*) as cnt
				from dbo.BvLoginGroup lg
				INNER JOIN BvTasks t 
					ON lg.PersonSID = t.PersonSID
				WHERE t.DialerId = @DialerId AND lg.DialTypeId = @DialType AND t.BreakTypeId IS NULL
				group by lg.ObjectSID, lg.SurveySID
		)
		INSERT INTO @Groups
		SELECT c.sid, count(*)
		FROM Logins c 
		INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = @SurveySid
		CROSS APPLY dbo.GetCallsForPredictiveMode(@DialType, c.cnt*20, a.Id, c.sid, @SurveySID, @SuitableTimeForCalls)
		where c.SurveySID = @SurveySID
		group by c.sid
	END
	ELSE
	BEGIN
		;WITH Logins AS (
			select lg.ObjectSID as sid, lg.SurveySID, count(*) as cnt
				from dbo.BvLoginGroup lg
				INNER JOIN BvTasks t 
					ON lg.PersonSID = t.PersonSID
				WHERE lg.DialTypeId = @DialType AND t.BreakTypeId IS NULL
				group by lg.ObjectSID, lg.SurveySID
		)
		INSERT INTO @Groups
		SELECT c.sid, count(*)
		FROM Logins c 
		INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = @SurveySid
		CROSS APPLY dbo.GetCallsForPredictiveMode(@DialType, c.cnt*20, a.Id, c.sid, @SurveySID, @SuitableTimeForCalls)
		where c.SurveySID = @SurveySID
		group by c.sid
	END
    
    DECLARE @totalCount INT 
    SELECT @totalCount = SUM(GroupSize) FROM @Groups
    DECLARE @part FLOAT = CAST(@Count AS FLOAT)/CAST(@totalCount AS FLOAT)
    DECLARE @current INT
    DECLARE @currentMinValue INT
    
    UPDATE @Groups
    SET @current = GroupSize*@part+0.5,
        @currentMinValue = CASE WHEN GroupSize < @MinDistributedCalls THEN GroupSize ELSE @MinDistributedCalls END,
        @current = CASE WHEN @current < @MinDistributedCalls THEN @currentMinValue ELSE @current END,
        GroupSize = @current
        
    DECLARE @usedCalls TABLE(
        [ObjectSid] [int] NOT NULL,
        [ID] [int] NOT NULL, 
        [Interview] [int] NOT NULL,
        [TimeInShift] [datetime] NOT NULL,
		[Priority] [INT] NOT NULL,
	    [CallOrder] [INT] NOT NULL,
		[ApptID] [INT],
	    [ExpireTime] [datetime],
		[ShiftPriority] TINYINT NOT NULL )
        
	;WITH orderedUpdateTable as
	(    
		SELECT calls.*
		FROM @Groups g
		CROSS APPLY dbo.GetCallsForGroupForPredictiveSurvey(@DialType, g.GroupSize, @SurveySID, g.ObjectSid, @SuitableTimeForCalls) calls
	)
	UPDATE orderedUpdateTable WITH(READPAST)
    SET CallState = -2 
	OUTPUT inserted.ExplicitSid,
		   inserted.[ID],
		   inserted.[interviewID],
		   inserted.[TimeInShift],
		   inserted.[Priority],
		   inserted.[CallOrder],
		   inserted.[ApptID],
		   inserted.[ExpireTime],
		   deleted.[ShiftPriority]
	INTO @usedCalls

	INSERT INTO bvCallsSentToDialer
	SELECT @SuitableTimeForCalls AS [Time], 
		   @SurveySID AS SurveySID, 
		   ObjectSid AS ExplicitSid, 
		   Count(*) AS CallsCount
	 FROM @usedCalls GROUP BY ObjectSid
    
    SELECT c.ID, 
           ISNULL( p.Sid, 0 ) AS ExplicitSid, --person id (if person is assigned) or 0 (if survey or person group)
           @SurveySID AS SurveySid,
           i.DialingMode DiallingMode,
		   Interview AS InterviewID, 
		   TelephoneNumber,
		   ExtensionNumber,
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   (CASE WHEN p.Sid IS NULL AND @SurveySID <> ObjectSid THEN ObjectSid
                 ELSE 0
            END) AS GroupID, --Explicit Group ID or 0 in all other cases (when call is assigned to  implicit survey group or a user)
		   c.ExpireTime
    FROM @usedCalls c
    INNER JOIN BvInterview i ON Interview = i.ID AND   --we should avoid this join. this field can be stored in bvsvyschedule or somewhere else
                                SurveySID = @SurveySID
    LEFT JOIN BvPerson p on p.SID = ObjectSid
	ORDER BY Priority DESC,
			 c.ShiftPriority DESC,
             TimeInShift,
			 CallOrder
	
RETURN (@@ROWCOUNT)