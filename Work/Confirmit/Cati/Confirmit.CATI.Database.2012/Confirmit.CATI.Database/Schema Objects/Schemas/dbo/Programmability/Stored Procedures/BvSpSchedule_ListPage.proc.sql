CREATE PROCEDURE [dbo].[BvSpSchedule_ListPage]
    @PageNumber INT, 
    @PageSize INT, 
    @OrderField NVARCHAR (64), 
    @IsOrderASC INT,  
    @SearchCondition NVARCHAR (4000)=NULL
AS
SET NOCOUNT ON

IF @PageNumber IS NULL AND @PageSize IS NULL
BEGIN
/* Looks like we're generating code using FMTONLY. So lets return metadata*/
 SELECT  
        0 AS [SID],
        '' AS [Name],
        getdate() AS [CreateDate],
        getdate() AS [ModifyDate],
        0 AS [State],
        0 AS [DesignStateGroupID],
        '' AS [DesignStateGroupName]
END

DECLARE @Query AS NVARCHAR(4000)
DECLARE @IDField AS NVARCHAR(64)
DECLARE @DefaultStateGroupID AS INT

SET @IDField = 'SID';
SELECT @DefaultStateGroupID = MIN(ID) FROM [BvStateGroup] 

SET @Query =
    'SELECT  
        ScheduleID      AS SID,
        sch.Name            AS Name,
        CreateDate      AS CreateDate,
        ModifyDate      AS ModifyDate,
        CASE WHEN LEN( XmlInUse ) = 0 THEN 0 --Not launched
			 WHEN XmlInUse <> XmlUnderDev THEN 1 -- Pending synchronized
			 ELSE 2 -- Synchronized
		END as State,
		sch.DesignStateGroupID,
		gr.Name as DesignStateGroupName
    FROM BvSchedule sch inner join BvStateGroup as gr on gr.ID = isnull(sch.DesignStateGroupID, ' + CONVERT(NVARCHAR, @DefaultStateGroupID) + ')'

DECLARE @TotalCount INT
exec @TotalCount = BvSpGetListPage @PageNumber, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition
RETURN @TotalCount

