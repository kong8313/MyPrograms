CREATE PROCEDURE [dbo].[BvSpAsyncOperations_ListPage]
@CallCenterId INT = NULL,
@PageNumber INT, 
@PageSize INT, 
@OrderField NVARCHAR (64), 
@IsOrderASC INT, 
@userName NVARCHAR (255), 
@SearchCondition NVARCHAR (4000) = NULL
AS
SET NOCOUNT ON

 IF @PageNumber IS NULL AND @PageSize IS NULL
 BEGIN
 /* Looks like we're generating code using FMTONLY. So lets return metadata*/

 SELECT  
		0                  AS   Id,
		''                 AS   InitiatorName,
        ''                 AS   ProjectId,
		0                  AS   CallCenterId,
		''                 AS   CallCenterName,
		CAST(0 AS TINYINT) AS   OperationType,
		CAST(0 AS TINYINT) AS   OperationState,
		GETUTCDATE()       AS   InitiatedTime,
		GETUTCDATE()       AS   StartedTime,
		GETUTCDATE()       AS   FinishedTime,
		''                 AS   OperationTitle
     
     RETURN 0;
 END
 
DECLARE @Query as nvarchar(4000)
DECLARE @IDField as nvarchar(64) 
SET @IDField = 'Id'

SET @Query =
    'SELECT
		ao.Id                       AS   Id,
		ao.CreatedBySupervisorName  AS   InitiatorName,
		BvSurvey.Name               AS   ProjectId,
		CallCenterId                AS   CallCenterId,
		cc.Name                     AS   CallCenterName,
		ao.Type                     AS   OperationType,
		ao.State                    AS   OperationState,
		ao.QueuedDate               AS   InitiatedTime,
		ao.StartedDate              AS   StartedTime,
		ao.FinishedDate             AS   FinishedTime,
		ao.Title		            AS   OperationTitle
        FROM    
			BvAsyncOperationQueue ao
			INNER JOIN BvFnSurvey_GetByCallCenterId(' + (case when @CallCenterId IS NULL Then 'NULL' else cast(@CallCenterId AS NVARCHAR) end) + ') BvSurvey ON ao.SurveySid = BvSurvey.SID			
			LEFT JOIN BvUserSurveyPermission ON ( BvUserSurveyPermission.UserName = '''+  @userName + ''' AND BvUserSurveyPermission.SurveySID = ao.SurveySid)
			LEFT JOIN BvCallCenter cc ON CallCenterId = cc.ID
        WHERE
            ((BvUserSurveyPermission.UserName IS NOT NULL) OR ( ''' + @userName + ''' = '''' ))'
				  
DECLARE @TotalCount INT
exec @TotalCount = BvSpGetListPage @PageNumber, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition
RETURN @TotalCount		