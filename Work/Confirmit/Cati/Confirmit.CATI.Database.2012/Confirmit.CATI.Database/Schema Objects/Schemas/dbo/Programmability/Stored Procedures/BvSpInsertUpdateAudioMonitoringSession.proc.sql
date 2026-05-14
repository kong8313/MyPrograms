CREATE PROCEDURE [dbo].[BvSpInsertUpdateAudioMonitoringSession]
	@SupervisorName nvarchar(255),
	@InterviewerId int,
	@TelephoneNumber nvarchar(255),
	@SessionId nvarchar(255)
AS
	UPDATE[AudioMonitoring]
		SET [InterviewerSID] = @InterviewerId,
			[TelephoneNumber] = @TelephoneNumber,
			[SessionID] = @SessionId
		WHERE [SupervisorName] = @SupervisorName

	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO [AudioMonitoring] ([SupervisorName], [InterviewerSID], [TelephoneNumber], [SessionID])
			VALUES (@SupervisorName, @InterviewerId, @TelephoneNumber, @SessionId)
	END
	RETURN 0