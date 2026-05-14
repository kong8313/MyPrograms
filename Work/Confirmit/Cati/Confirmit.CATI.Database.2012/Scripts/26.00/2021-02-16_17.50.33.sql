GO
PRINT N'Altering [dbo].[BvMessageToPerson]...';


GO
ALTER TABLE [dbo].[BvMessageToPerson]
    ADD [IsSeen] BIT NULL;


GO
PRINT N'Altering [dbo].[BvSpSendMessageToGroups]...';


GO
ALTER PROCEDURE [dbo].[BvSpSendMessageToGroups]
	@BatchId int,	
	@OnlineOnly bit,
    @MessageBody nvarchar(1024),	
	@MessageSupervisorName nvarchar(50),
	@CallCenterID INT
AS

BEGIN

	IF @BatchId IS NULL
	BEGIN
		SELECT 0 AS InterviewerId		
		RETURN 0;
	END

	DECLARE @MessageId int
	INSERT INTO BvMessages (Body, CreateTime, SupervisorName) VALUES(@MessageBody, GETUTCDATE(), @MessageSupervisorName);
	SET @MessageId = SCOPE_IDENTITY();

	/* Interviewer group contains all interviewers including ones in nested groups */	
	WITH CTE (ObjectSID) 
	AS
	(
		SELECT m.ObjectSID
			FROM bvMembership as m
			Inner join bvTransferArrays ON BatchId = @BatchId 
			WHERE
			 [m].[ContainerSID] = ItemId 

		UNION ALL
		
			SELECT m.ObjectSID
			FROM bvMembership as m
			INNER JOIN CTE as c
			ON m.ContainerSID = c.ObjectSID
	),
	CTE_ALL_INTERVIEWERS AS
	(
		SELECT DISTINCT p.[SID] FROM
			BvFnPerson_Get(@CallCenterID) AS p
			INNER Join CTE AS c
				ON c.ObjectSID = p.SID		
	)

	/* Save into temporary table all interviewers for whom we should send message.
	If flag @OnlineOnly is true save only online interviewers otherwise all interviewers */
	SELECT SID INTO #INTERVIEWERS 
	FROM CTE_ALL_INTERVIEWERS as C
	LEFT JOIN BvTasks as L ON C.SID = L.PersonSID
	WHERE (@OnlineOnly = 0 OR (@OnlineOnly=1 AND L.PersonSID IS NOT NULL))

	BEGIN TRANSACTION

		INSERT INTO BvMessageToPerson (MessageId, InterviewerId) 
					SELECT @MessageId, I.SID FROM #INTERVIEWERS as I

			UPDATE BvPerson SET HasNewMessage = 1 
				WHERE SID IN (SELECT #INTERVIEWERS.SID FROM #INTERVIEWERS )					
					
	COMMIT TRANSACTION

	Select SID AS InterviewerId from #INTERVIEWERS

	DROP TABLE #INTERVIEWERS

END
GO
PRINT N'Altering [dbo].[BvSpSendMessageToSurveys]...';


GO
ALTER PROCEDURE  [dbo].[BvSpSendMessageToSurveys]
	@BatchId int,	
    @MessageBody nvarchar(1024),
	@MessageSupervisorName nvarchar(50),
	@CallCenterID INT
AS

BEGIN
	
	IF @BatchId IS NULL
	BEGIN
		SELECT 0 AS InterviewerId		
		RETURN 0;
	END

	DECLARE @MessageId int
	INSERT INTO BvMessages (Body, CreateTime, SupervisorName) VALUES(@MessageBody, GETUTCDATE(), @MessageSupervisorName);
	SET @MessageId = SCOPE_IDENTITY();

	/* Survey group contains all interviewer working on survey*/
	BEGIN TRANSACTION
			DECLARE @MessageToPerson TABLE( MessageId INT, InterviewerId INT )

			UPDATE BvPerson 
				SET HasNewMessage = 1
			OUTPUT @MessageId, inserted.SID INTO @MessageToPerson (MessageId, InterviewerId)
			FROM											
				BvPerson as p
				INNER JOIN  bvTasks as t ON p.SID = t.PersonSID
				INNER JOIN 	bvTransferArrays a ON t.SurveySID = a.ItemId 
			WHERE p.CallCenterID = @CallCenterID AND a.BatchId = @BatchId

			INSERT INTO BvMessageToPerson(MessageId, InterviewerId) SELECT MessageId, InterviewerId FROM @MessageToPerson

	Select InterviewerId from @MessageToPerson

	COMMIT TRANSACTION
	
END
GO
PRINT N'Refreshing [dbo].[BvSpGetMessages]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetMessages]';


GO
PRINT N'Refreshing [dbo].[BvSpSendMessageToInterviewers]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSendMessageToInterviewers]';


GO
PRINT N'Update complete.';


GO
