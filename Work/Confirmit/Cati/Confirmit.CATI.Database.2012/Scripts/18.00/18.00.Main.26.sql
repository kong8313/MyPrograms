
GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpGetActiveCallsForSurvey]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetActiveCallsForSurvey]
	@SurveySID INT,	
	@SuitableTimeForCalls DATETIME
AS
	
	IF @SurveySID  IS NULL
	BEGIN
		SELECT  '' AS Name,
				0 AS ResultCount,
				0 AS RequestCount		
		RETURN 0;
	END
				    			    
	SELECT ISNULL (g.[Name], '*Survey Assignment*' ) as [Name], 
	count(*)  AS ResultCount, 
	CAST(c.cnt*10  AS INT) AS RequestCount
    FROM vLogins c with ( noexpand, INDEX([pk_vLogins]) )
	INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = @SurveySid
	CROSS APPLY dbo.GetCallsForPredictiveMode(c.cnt*10, a.Id, c.sid, @SurveySID, @SuitableTimeForCalls)
	LEFT JOIN (SELECT [SID], [Name] FROM [BvPerson]
			   UNION 
			   SELECT [SID], [Name] from [BvPersonGroup] ) as g on [ExplicitSid] = g.[SID]
	where c.SurveySID = @SurveySID
	group by g.Name, c.cnt   	
	
RETURN (@@ROWCOUNT)
GO
PRINT N'Update complete.';


GO
