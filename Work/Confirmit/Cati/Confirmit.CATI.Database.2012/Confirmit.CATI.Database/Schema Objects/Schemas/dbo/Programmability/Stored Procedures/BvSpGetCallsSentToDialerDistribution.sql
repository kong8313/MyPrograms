/* Stored procedure dbo.BvSpGetCallsSentToDialerDistribution returns a breakdown of calls sent to dialer per ExplicitSid ( user/group )
   for 20 min starting from @StartTime for a specified suurvey @SurveySid */
CREATE PROCEDURE [dbo].[BvSpGetCallsSentToDialerDistribution]
	@StartTime DATETIME = NULL,                      -- expects UTC time
	@SurveySid INT,
	@timezoneId INT
AS
 
DECLARE @Total INT
DECLARE @tableStartTime AS DateTime,
		@tableEndTime AS DateTime,
		@cols         AS NVARCHAR(MAX),
		@sql         AS NVARCHAR(MAX),
		@bias         AS INT;
	
IF(@StartTime IS NULL)
BEGIN
	SET @tableEndTime = GETUTCDATE();
	SET @tableStartTime =  (SELECT TOP(1) * 
							FROM(SELECT DISTINCT TOP(20) [Time] 
								 FROM [BvCallsSentToDialer] 
							     WHERE SurveySid = @SurveySid AND [Time] <= @tableEndTime  ORDER BY [Time] desc) AS r 
							ORDER BY r.[Time] ASC)
END
ELSE 
BEGIN
	SET @tablestartTime = @StartTime;
	SET @tableEndTime =  (SELECT TOP(1) * 
						  FROM (SELECT DISTINCT TOP(20) [Time] 
							    FROM [BvCallsSentToDialer] 
							    WHERE SurveySid = @SurveySid AND [Time] >= @tableStartTime ORDER BY [Time]) AS r 
						  ORDER BY r.[Time] DESC);
END
 

SELECT  @bias = DATEDIFF( [mi], @tablestartTime,  dbo.UTC2LT( @tablestartTime, Bias, DaylightType,
							StandardDayOfWeek, StandardStart, StandardBias,
							DaylightDayOfWeek, DaylightStart, DaylightBias ))
FROM  [BvTimezone]
WHERE [ID] = @timezoneId

;WITH timeList AS
(
	SELECT DISTINCT TOP(20) [Time] 
	FROM [BvCallsSentToDialer] 
	WHERE SurveySid= @SurveySid AND [Time] >= @tableStartTime ORDER BY [Time]
)
SELECT  
@cols = STUFF((SELECT N', ' +  QUOTENAME(CONVERT(nvarchar(max),  DATEADD( mi, @bias, [Time]), 21)) FROM timeList FOR XML PATH('')), 1, 2, '');
 
-- Construct the full T-SQL statement and execute dynamically. Query could look like this
/*
SELECT *
FROM (SELECT ISNULL ( g.Name,'*Survey Assignment*') as [Group/User Name], convert(char(5), DATEADD( mi,-300, [time]), 108) AS [minutes], [CallsCount]
          FROM dbo.BvActiveCallsInfo LEFT JOIN ( SELECT SID, Name FROM BvPerson UNION SELECT SID, Name from BvPersonGroup ) as g on ExplicitSid = g.SID 
          where surveysid=1 and [time] >='Dec  7 2009 11:50AM' and [time] <='Dec  7 2009 12:10PM' ) AS D
  PIVOT(MAX(CallsCount) FOR minutes IN([07:00],[07:01],[07:02],[07:03],[07:04],[07:05],[07:07],[07:08],[07:09],[07:10]) )  as  P order by [Group/User Name] ;
*/
SET @sql = N'SELECT *
FROM (SELECT ISNULL ( g.[Name],' + '''' + '*Survey Assignment*' + '''' + ') as [Group or User], 
			 CONVERT(nvarchar(max), DATEADD( mi,' + CAST( @bias AS VARCHAR(MAX)) + ', [time]), 21) AS [requestTime], 
			 [CallsCount]
          FROM [dbo].[BvCallsSentToDialer]
          LEFT JOIN BvViewPersonAndGroup g 
			  ON [ExplicitSid] = g.[SID] 
		  WHERE [SurveySID]=' + CAST( @SurveySid  AS VARCHAR(32))+ ' AND [Time] >=' + '''' + 
			  + Convert(nvarchar(max), @tableStartTime , 21) + '''' + ' AND [Time] <=' + '''' + 
			  Convert(nvarchar(max), @tableEndTime , 21) + '''' + ') AS D
  PIVOT( MAX([CallsCount]) FOR [requestTime] IN(' + @cols + N') ) AS P ORDER BY [Group or User];';
  
EXEC sp_executesql @sql;

SELECT  @total = SUM(CallsCount) FROM BvCallsSentToDialer WHERE SurveySID = @SurveySid AND [Time] BETWEEN @tableStartTime AND @tableEndTime
RETURN @total