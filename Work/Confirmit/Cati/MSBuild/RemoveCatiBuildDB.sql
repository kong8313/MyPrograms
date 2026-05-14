/*************************/
/**** Drop CATI BUILD databases *****/
/*************************/
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'ConfirmitCATIV15_BUILD')
BEGIN
	ALTER DATABASE [ConfirmitCATIV15_BUILD] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
	DROP DATABASE [ConfirmitCATIV15_BUILD]
END
GO

