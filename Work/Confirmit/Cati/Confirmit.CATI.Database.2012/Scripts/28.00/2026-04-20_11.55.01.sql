PRINT N'Dropping Index [dbo].[BvSvySChedule].[IX_BvSvySchedule_ShiftTypeID]...';


GO
DROP INDEX [IX_BvSvySchedule_ShiftTypeID]
    ON [dbo].[BvSvySChedule];

GO
PRINT N'Dropping Function [dbo].[BvFnBlacklist_GetInterviewIdsForBlacklistRange]...';


GO
DROP FUNCTION [dbo].[BvFnBlacklist_GetInterviewIdsForBlacklistRange];


GO
PRINT N'Dropping Procedure [dbo].[BvSpTelephoneBlacklist_GetSurveysToDeleteCalls]...';


GO
DROP PROCEDURE [dbo].[BvSpTelephoneBlacklist_GetSurveysToDeleteCalls];


GO
PRINT N'Altering Function [dbo].[RemoveNonNumericCharacters]...';


GO
ALTER FUNCTION dbo.RemoveNonNumericCharacters (@strText VARCHAR(1000))
    RETURNS VARCHAR(1000)
AS
BEGIN
    DECLARE @result VARCHAR(1000) = '';

    ;WITH N AS (
        SELECT TOP (LEN(@strText)) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
        FROM sys.objects
    )
     SELECT @result = @result + SUBSTRING(@strText, n, 1)
     FROM N
     WHERE SUBSTRING(@strText, n, 1) LIKE '[0-9]';

    RETURN @result;
END
GO
PRINT N'Refreshing Procedure [dbo].[BvSpTelephoneBlacklist_Filter]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTelephoneBlacklist_Filter]';


GO
PRINT N'Update complete.';
