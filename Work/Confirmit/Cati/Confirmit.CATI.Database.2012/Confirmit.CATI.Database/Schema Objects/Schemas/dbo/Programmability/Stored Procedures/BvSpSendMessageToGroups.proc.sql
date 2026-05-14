CREATE PROCEDURE [dbo].[BvSpSendMessageToGroups]
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