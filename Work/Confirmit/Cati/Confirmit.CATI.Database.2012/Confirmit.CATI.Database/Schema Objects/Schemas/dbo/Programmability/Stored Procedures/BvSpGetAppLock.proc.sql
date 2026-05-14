CREATE PROCEDURE [dbo].[BvSpGetAppLock]
	@ResourceName NVARCHAR(255),
	@LockMode NVARCHAR(32),
	@LockTimeout INT,
	@ServerName NVARCHAR(MAX),
	@WaitPeriod INT, --milliseconds
	@ResourceOwner NVARCHAR(MAX)
AS
    DECLARE @ReturnValue INT = 0;
    
    EXEC @ReturnValue = sp_getapplock @ResourceName, @LockMode, N'Session', @LockTimeout
    
    IF @ReturnValue >= 0
    BEGIN
		MERGE INTO BvAppLocks AS Target
		  USING ( SELECT @ResourceName AS ResourceName ) 
		  AS Source (ResourceName)
			ON Target.ResourceName = Source.ResourceName
		  WHEN MATCHED AND
			   (TimeLockLeave IS NULL OR
				DATEADD(millisecond, @WaitPeriod, TimeLockLeave) <= GETUTCDATE())
		  THEN
			 UPDATE SET TimeLockEnter = GETUTCDATE(),
						TimeLockLeave = NULL,
						ServerName = @ServerName,
						IsLockHeld = 1,
						ResourceOwner = @ResourceOwner
		  WHEN NOT MATCHED THEN
			 INSERT(ResourceName, TimeLockEnter, TimeLockLeave, IsLockHeld, ServerName, ResourceOwner)
			 VALUES(@ResourceName, GETUTCDATE(), NULL, 1, @ServerName, @ResourceOwner);
         
		IF(@@ROWCOUNT = 0)
		BEGIN
		   SET @ReturnValue = 2 --period is not expired
		   EXEC sp_releaseapplock @ResourceName, N'Session'
		END
    END
	   
	
RETURN @ReturnValue