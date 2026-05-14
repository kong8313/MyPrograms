PRINT N'Altering [dbo].[BvSpReleaseAppLock]...';
GO

ALTER PROCEDURE [dbo].[BvSpReleaseAppLock]
	@ResourceName NVARCHAR(255),
	@Succesfull BIT, --if some errors was occured then last execution for lock is not changed
	@DeleteFromBvAppLocks BIT -- Delete lock record from the BvAppLocks if true
AS
	DECLARE @ReturnValue INT = 0

	IF @DeleteFromBvAppLocks = 1
		DELETE FROM BvAppLocks
		WHERE ResourceName = @ResourceName
	ELSE
		IF @Succesfull = 1
			UPDATE BvAppLocks
			SET TimeLockLeave = GETUTCDATE(),
				IsLockHeld = 0
			WHERE ResourceName = @ResourceName
	
    EXEC @ReturnValue = sp_releaseapplock @ResourceName, N'Session'
	
RETURN @ReturnValue
GO

PRINT N'Update complete.';
GO
