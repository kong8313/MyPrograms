CREATE PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]
 @SurveySID INT,
 @DialerId INT = 0,
 @Count  INT,  --number of requested calls
 @SuitableTimeForCalls DATETIME,
 @DialType INT = 0
AS

SET NOCOUNT ON
	DECLARE @FixeNumberCallsPerPerson INT = 10

      --it should be at end of the SP. it uses for decrease recompilation
	DECLARE @CachedCalls TABLE (
	  [ExplicitSID] [int] NOT NULL,
	  [ID] [int] NOT NULL,
	  [InterviewID] [int] NOT NULL,
	  [TimeInShift] [datetime] NULL,
	  [Priority] [int] NOT NULL,
	  [CallOrder] [int] NOT NULL,
	  [ApptId] [int],
	  [ExpireTime] [datetime],
	  [ShiftPriority] TINYINT NOT NULL )
     
	;WITH orderedUpdateTable AS
	(
		SELECT c.*, a.ShiftPriority
		FROM BvLoginGroup g
		INNER JOIN BvTasks t ON g.PersonSID = t.PersonSID
		INNER JOIN BvActiveShiftTypeZone a on a.SurveyId=@SurveySID and a.SurveyId=g.SurveySID
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](@DialType, g.PersonSID, a.Id, @SurveySID, @SuitableTimeForCalls, @FixeNumberCallsPerPerson) c
		WHERE g.PersonSid = g.ObjectSID AND t.DialerId = @DialerId and g.DialTypeId = @DialType AND t.BreakTypeId IS NULL
	)
    UPDATE orderedUpdateTable 
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
	INTO @CachedCalls

	INSERT INTO bvCallsSentToDialer
	SELECT @SuitableTimeForCalls AS [Time], 
		   @SurveySID AS SurveySID, 
		   ExplicitSID AS ExplicitSid, 
		   Count(*) AS CallsCount
	 FROM @CachedCalls GROUP BY ExplicitSID

    SELECT c.[ID], 
           c.[ExplicitSid], 
           @SurveySID SurveySid, 
           i.DialingMode DiallingMode,
		   c.[InterviewID], 
		   i.[TelephoneNumber],
		   i.[ExtensionNumber],
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   0 as [GroupID],
		   c.ExpireTime
    FROM @CachedCalls c 
    INNER JOIN BvInterview i ON c.[Interviewid] = i.[ID] AND
                                i.[SurveySID] = @SurveySID
    ORDER BY Priority DESC, c.ShiftPriority DESC, TimeInShift, CallOrder
 
RETURN (@@ROWCOUNT)

