
PRINT N'Dropping [dbo].[BvTelephoneBlacklist].[IX_BvTelephoneBlacklist]...';
GO

DROP INDEX [IX_BvTelephoneBlacklist] ON [dbo].[BvTelephoneBlacklist]
GO

PRINT N'Altering [dbo].[BvTelephoneBlacklist] table...';
GO

ALTER TABLE [dbo].[BvTelephoneBlacklist] ADD [Type] TINYINT NOT NULL CONSTRAINT DF_BvTelephoneBlacklist_Type DEFAULT(0)
GO

PRINT N'Creating [dbo].[BvTelephoneBlacklist].[IX_BvTelephoneBlacklist]...';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_BvTelephoneBlacklist]
    ON [dbo].[BvTelephoneBlacklist]([Type] ASC, [TelephoneNumber] ASC) WITH (IGNORE_DUP_KEY = ON)
    ON [PRIMARY];
GO
PRINT N'Dropping [dbo].[BvTelephoneBlacklist].[DF_BvTelephoneBlacklist_Type] constraint...';
GO

ALTER TABLE [dbo].[BvTelephoneBlacklist] DROP CONSTRAINT DF_BvTelephoneBlacklist_Type

GO
PRINT N'Creating [dbo].[BvFnBlacklist_IsTelephoneNumberFiltered]...';


GO
CREATE FUNCTION BvFnBlacklist_IsTelephoneNumberFiltered( @TelephoneNumber VARCHAR(255))
RETURNS TABLE
AS
	RETURN SELECT CASE WHEN EXISTS( select top(1) 1 as Filtered FROM BvTelephoneBlacklist WHERE Type = 1 /*StartWith*/ AND TelephoneNumber BETWEEN  SUBSTRING(@TelephoneNumber, 0, 1) AND @TelephoneNumber AND TelephoneNumber = SUBSTRING( @TelephoneNumber, 0, LEN(TelephoneNumber) + 1 ) ORDER BY TelephoneNumber DESC ) OR 
							EXISTS( select 1 FROM BvTelephoneBlacklist WHERE Type = 0 /*Equal*/ AND TelephoneNumber = @TelephoneNumber ) THEN 1 ELSE 0 END as IsFiltered
GO
PRINT N'Altering [dbo].[BvSpTelephoneBlacklist_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpTelephoneBlacklist_Insert]
@Type TINYINT,
@TelephoneNumber varchar(255)
AS
SET NOCOUNT ON

INSERT INTO [dbo].[BvTelephoneBlacklist]([Type], [TelephoneNumber])
    VALUES (@Type, @TelephoneNumber)
GO
PRINT N'Creating [dbo].[BvSpTelephoneBlacklist_Filter]...';


GO
CREATE PROCEDURE [dbo].[BvSpTelephoneBlacklist_Filter]
  @TelephoneNumbers BvStringArrayType READONLY
AS
SELECT t.Value AS TelephoneNumber, f.IsFiltered as IsFiltered 
	FROM @TelephoneNumbers AS t 
	CROSS APPLY [dbo].[BvFnBlacklist_IsTelephoneNumberFiltered]( Value) AS f
GO
PRINT N'Altering [dbo].[BvSpSvyShedule_DeleteCallsByBlacklist]...';


GO
ALTER PROCEDURE [dbo].[BvSpSvyShedule_DeleteCallsByBlacklist]
    @PatternType TINYINT, @phoneNumber VARCHAR(255)
AS 
CREATE TABLE #UpdatedRows(interviewId INT, surveySID INT)
IF @PatternType = 0 /*Equal*/
BEGIN
	UPDATE ss SET CallState = 0/*ToBeDeleted*/ OUTPUT inserted.InterviewID, inserted.SurveySID  INTO #UpdatedRows FROM BvSvySchedule ss 
		INNER JOIN BvInterview i ON ss.InterviewID = i.ID AND i.SurveySID = ss.SurveySID
		INNER JOIN BvSurvey s ON i.SurveySID = s.SID
		WHERE s.IsTelephoneBlacklistSupported = '1' AND i.TelephoneNumber = @phoneNumber AND ss.CallState IN (1, 2)
END
ELSE IF @PatternType = 1 /*StartWith*/
BEGIN
	UPDATE ss SET CallState = 0/*ToBeDeleted*/ OUTPUT inserted.InterviewID, inserted.SurveySID  INTO #UpdatedRows FROM BvSvySchedule ss 
		INNER JOIN BvInterview i ON ss.InterviewID = i.ID AND i.SurveySID = ss.SurveySID
		INNER JOIN BvSurvey s ON i.SurveySID = s.SID
		WHERE s.IsTelephoneBlacklistSupported = '1' AND i.TelephoneNumber BETWEEN @phoneNumber AND (@phoneNumber + 'A') AND ss.CallState IN (1, 2)
END

UPDATE i SET TransientState = 17/*Blacklisted*/ FROM BvInterview i INNER JOIN #UpdatedRows ur ON i.ID = ur.interviewId AND i.SurveySID = ur.surveySID

GO

PRINT N'Update complete.';


GO
