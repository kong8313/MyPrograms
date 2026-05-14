
GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
	;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
	(
	SELECT 'InterviewerPassword.IsExpirationEnabled', 'IsCatiInterviewerPasswordExpirationEnabled', 'Supervisor', 'Is CATI interviewer password expiration enabled. Possible values: true or false.', 3, 0, 'False'
	UNION ALL
	SELECT 'InterviewerPassword.ExpirationPeriodInDays', 'CatiInterviewerPasswordExpirationPeriodInDays', 'Supervisor', 'Cati interviewer expiration period in days.', 1, 0, '30'
	UNION ALL
	SELECT 'InterviewerPassword.IsResetToSamePasswordEnabled', 'IsCatiInterviwerAllowedToSetTheSamePasswordAtChangePasswordProcedure', 'Supervisor', 'Can Cati interviewer set the same password during password change procedure. Possible values: true or false.', 3, 0, 'False'
	UNION ALL
	SELECT 'InterviewerPassword.IsMinimumPasswordLengthEnforced', 'CatiInterviewerPasswordMinimalLength', 'Supervisor', 'Minimal allowed length of Cati interviewer password. If 0 then any length is allowed.', 3, 0, 'False'
	UNION ALL
	SELECT 'InterviewerPassword.MinimumPasswordLength', 'CatiInterviewerPasswordMinimalLength', 'Supervisor', 'Minimal allowed length of Cati interviewer password. This option works only when IsMinimumPasswordLengthEnforced is True.', 1, 0, '6'
	UNION ALL
	SELECT 'InterviewerPassword.IsComplexPasswordEnforced', 'IsComplexPasswordRuleEnforcedForCatiInterviewerPassword', 'Supervisor', 'Is complex password rule enforced for Cati interviewer password. Possible values: true or false. Complex password must must have at least 1 upper case character and 1 non-alphanumeric character.', 3, 0, 'False'
	)
	INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
		SELECT * FROM Data
END

PRINT N'Update complete.';

GO
