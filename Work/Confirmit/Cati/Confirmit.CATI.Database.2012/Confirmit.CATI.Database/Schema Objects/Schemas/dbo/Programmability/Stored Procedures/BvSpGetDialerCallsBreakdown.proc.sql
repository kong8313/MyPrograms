CREATE PROCEDURE [dbo].[BvSpGetDialerCallsBreakdown]
@SurveySID INT
AS

DECLARE @Total INT
DECLARE @CallsBreakdown TABLE
(
	[Group or User] VARCHAR(MAX),
	[Count] INT,
	[Total] INT
)
 
;WITH DialerCalls AS
(
SELECT ISNULL ( g.[Name],'*Survey Assignment*') as [Group or User], ExplicitSid
FROM BvSvySchedule c
LEFT JOIN BvViewPersonAndGroup g 
	ON [ExplicitSid] = g.[SID] 
WHERE [SurveySID]=@SurveySID AND c.CallState = -2 
),
CallsBreakdownd AS
(
SELECT 
	[Group or User],
	COUNT(*) AS [Count],
	SUM(COUNT(*)) OVER() AS [Total]
FROM DialerCalls 
GROUP BY ExplicitSid, [Group or User]
)
INSERT INTO @CallsBreakDown 
SELECT * FROM CallsBreakdownd

SELECT TOP 1 @Total = [Total] FROM @CallsBreakdown
SELECT [Group or User], [Count] FROM @CallsBreakdown
ORDER BY [Group or User]

RETURN(@Total)
