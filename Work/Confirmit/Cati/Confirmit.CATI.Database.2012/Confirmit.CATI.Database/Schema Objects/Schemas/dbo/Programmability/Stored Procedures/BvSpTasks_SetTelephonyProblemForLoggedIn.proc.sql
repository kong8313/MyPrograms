CREATE PROCEDURE [dbo].[BvSpTasks_SetTelephonyProblemForLoggedIn]
@DialerId INT,
@ProblemCode INT 
AS
IF (@DialerId = 0)
BEGIN -- proceed for all dialers
	UPDATE BvTasks SET [ProblemId] = @ProblemCode
	 WHERE ([LoggedInToDialerState] = 2 /* LoginState.LOGGED_IN */
	  OR [LoggedInToDialerState] = 1 /* LoginState.LOGGING_IN */)
END
ELSE
BEGIN -- proceed for concrete dialer
	UPDATE BvTasks SET [ProblemId] = @ProblemCode
	 WHERE ([LoggedInToDialerState] = 2 /* LoginState.LOGGED_IN */
	  OR [LoggedInToDialerState] = 1 /* LoginState.LOGGING_IN */)
	  AND DialerId = @DialerId
END	
