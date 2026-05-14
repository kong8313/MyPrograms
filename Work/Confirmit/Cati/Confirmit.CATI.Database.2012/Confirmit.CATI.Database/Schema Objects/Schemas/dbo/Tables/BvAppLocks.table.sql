CREATE TABLE [dbo].[BvAppLocks]
(
	ResourceName NVARCHAR(255),
	TimeLockEnter DATETIME,
	TimeLockLeave DATETIME,
	IsLockHeld BIT,
	ServerName NVARCHAR(255),
	ResourceOwner NVARCHAR(MAX)
	CONSTRAINT PK_BvAppLocks_ResourceName PRIMARY KEY CLUSTERED (ResourceName)
)
