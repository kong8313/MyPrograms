CREATE PROCEDURE [dbo].[BvSpPromoteCalls]
	@surveyId INT,
	@quotaId INT,
	@cellId INT,
	@promotionPriority INT,
	@promotionCount INT,
	@promotionTime DATETIME
AS
    DECLARE @WhereCondition NVARCHAR(MAX)

    exec Bv_QuotaService_GetWhereForFilteredCell @surveyId, @quotaId, @cellId, 'repl', @WhereCondition OUTPUT
    
    DECLARE @sql NVARCHAR(MAX) = '
	WITH PromotedRespID AS
	(
	   SELECT respId
	   FROM BvReplicatedData_' + CAST(@surveyId AS NVARCHAR(255)) + ' AS repl
	   WHERE (' + @WhereCondition + ')
	),
	PromotedCalls AS
	(
	   SELECT TOP(@promotionCoun)  BvSvySchedule.*
	   FROM PromotedRespID
	   INNER JOIN BvSvySchedule ON SurveySID = @surveyId AND respId = InterviewID
	   WHERE TimeInShift <= @promotionTime AND
	         Priority <= @promotionPriority AND
	         CallState > 0
	   ORDER BY Priority DESC,
                TimeInShift,
                SurveySID,
                CallOrder
	)
	UPDATE PromotedCalls
	SET OldPriority = CASE WHEN OldPriority > 0 THEN OldPriority ELSE Priority END, 
	    Priority = @promotionPriority
	'
	
	DECLARE @sqlQueryParams NVARCHAR(MAX) = N'@surveyId INT, @promotionTime DATETIME, @promotionPriority INT, @promotionCoun INT';
	
	EXEC sp_executesql @sql, @sqlQueryParams, @surveyId, @promotionTime, @promotionPriority, @promotionCount
	RETURN @@ROWCOUNT