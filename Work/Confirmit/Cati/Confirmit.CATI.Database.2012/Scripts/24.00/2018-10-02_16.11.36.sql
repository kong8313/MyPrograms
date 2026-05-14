PRINT N'Update name of its 1001';

GO
UPDATE [BvState]
SET [BvState].[Name] = 'Dropped by respondent'
WHERE [BvState].[StateID] = 1001


GO
IF NOT EXISTS(SELECT 1 FROM [BvConfirmitStatus] WHERE StatusCode_BvFEE = 1000)
begin
	INSERT INTO [BvConfirmitStatus] ([StatusCode_Cnf], [StatusName_Cnf], [StatusCode_BvFEE]) VALUES( '1000', 'Inbound Call', 1000 )
end;


GO
IF NOT EXISTS(SELECT 1 FROM [BvConfirmitStatus] WHERE StatusCode_BvFEE = 1001)
begin
	INSERT INTO [BvConfirmitStatus] ([StatusCode_Cnf], [StatusName_Cnf], [StatusCode_BvFEE]) VALUES( '1001', 'Dropped by respondent', 1001 )
end;


GO
IF NOT EXISTS(SELECT 1 FROM [BvConfirmitStatus] WHERE StatusCode_BvFEE = 1020)
begin
	INSERT INTO [BvConfirmitStatus] ([StatusCode_Cnf], [StatusName_Cnf], [StatusCode_BvFEE]) VALUES( '1020', 'Dial interrupted by interviewer', 1020 )
end;


GO
PRINT N'Update complete.';