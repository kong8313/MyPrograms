CREATE PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]
 @SurveySID INT,
 @Count  INT, --number of requested calls
 @SuitableTimeForCalls DATETIME,
 @DialType INT = 0
AS
--best if it should be established at the connection level
--it may influence on count of recompilations
SET NOCOUNT ON

	DECLARE @FixeNumberCallsPerPerson INT = 10

      --it should be at end of the SP. it uses for decrease recompilation
	DECLARE @CachedCalls TABLE (
	  [ExplicitSID] [int] NOT NULL,
	  [ID] [int] NOT NULL,
	  [InterviewID] [int] NOT NULL,
	  [TimeInShift] [datetime] NULL,
	  [Priority] [INT] NOT NULL,
	  [CallOrder] [INT] NOT NULL,
	  [ApptID] [int] not null,
	  [ExpireTime] [datetime],
	  [ShiftPriority] TINYINT NOT NULL )
        
	;WITH orderedUpdateTable AS
	(
		SELECT TOP(@Count) c.*, a.ShiftPriority
        FROM BvActiveShiftTypeZone a
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](@DialType, @SurveySID, a.Id, @SurveySID, @SuitableTimeForCalls, @Count) c
		WHERE a.Surveyid = @SurveySID
		ORDER BY Priority DESC,
			     a.ShiftPriority DESC,
                 TimeInShift,
				 ExplicitType DESC,
				 CallOrder
	)
    UPDATE orderedUpdateTable
    SET CallState = -2 
	OUTPUT 0,
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
    ORDER BY Priority DESC,
		     c.ShiftPriority DESC,
             TimeInShift,
 			 CallOrder
 
RETURN (@@ROWCOUNT)
