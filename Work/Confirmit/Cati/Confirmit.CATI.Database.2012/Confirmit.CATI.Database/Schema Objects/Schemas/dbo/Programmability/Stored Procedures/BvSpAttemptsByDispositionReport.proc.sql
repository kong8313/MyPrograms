CREATE PROCEDURE [dbo].[BvSpAttemptsByDispositionReport]
   @SurveySid INT,
   @Itses NVARCHAR(MAX),
   @HideEmpty BIT,
   @StartDateTime DATETIME,
   @EndDateTime DATETIME,
   @CallCenterId INTEGER = NULL

   WITH RECOMPILE
AS
    DECLARE @StateGroupId INT,
	@MaxAttempts int = 11
    SELECT @StateGroupId = s.StateGroupID
    FROM BvSurvey s
    WHERE s.Sid = @SurveySid;
    
    IF(@StartDateTime IS NULL) SET @StartDateTime = '01-01-1753 00:00:00'
    IF(@EndDateTime IS NULL) SET @EndDateTime = '12-31-9999 23:59:59.997'

    ;WITH NecessaryItsList AS
    (
       SELECT s.StateID AS Its,
              s.Name AS [Name]
       FROM dbo.utilSplitNumbers( ISNULL(@Itses, ''), ',') i
       INNER JOIN BvState s ON (s.StateGroupID = @StateGroupId AND
                                s.StateID = i.Item)
       
       UNION 
       
       SELECT s.StateID AS Its,
              s.Name AS [Name]
       FROM BvState s
       WHERE @Itses IS NULL AND
             s.StateGroupID = @StateGroupId
    ),
	AllAttempts AS
	(
	   SELECT ( ROW_NUMBER() over(partition by InterviewID order by FiredTime)) AS NumberAttempts,
	          h.InterviewID AS InterviewId,
	          s.StateId AS Its,
	          s.Name AS ItsName
	   FROM BvState s
	   LEFT JOIN BvHistory h ON s.StateId = h.ITS AND
	                            h.SurveyId = @SurveySid AND
	                            h.FiredTime >= @StartDateTime AND
	                            h.FiredTime <= @EndDateTime AND
	                            h.InterviewId IS NOT NULL AND
	                            h.RoleID = 2
	   WHERE s.StateGroupID = @StateGroupId AND (h.CallCenterID = @CallCenterId OR @CallCenterId IS NULL OR h.ID is NULL) 
	),
	Attempts AS
	(
	   SELECT IIF(NumberAttempts > @MaxAttempts , @MaxAttempts, NumberAttempts ) AS NumberAttempts,
	          InterviewId,
	          Its,
	          ItsName
	   FROM AllAttempts
	),
	AttemptsByDesposition AS
	(
	   SELECT Its AS  Code,
	          ItsName AS Disposition,
              [1] AS Attempts1,
              [2] AS Attempts2,
              [3] AS Attempts3,
              [4] AS Attempts4,
              [5] AS Attempts5,
              [6] AS Attempts6,
              [7] AS Attempts7,
              [8] AS Attempts8,
              [9] AS Attempts9,
              [10] AS Attempts10,
			  [11] AS Attempts11AndMore
       FROM Attempts a
       PIVOT
       (
          COUNT(a.InterviewId) 
          FOR a.NumberAttempts in ( [1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11])
       ) AS p
       WHERE (@HideEmpty = 0 OR
              [1]+[2]+[3]+[4]+[5]+[6]+[7]+[8]+[9]+[10]+[11] > 0)
    )
    SELECT abd.*
    FROM AttemptsByDesposition abd
    INNER JOIN NecessaryItsList il ON il.Its = abd.Code