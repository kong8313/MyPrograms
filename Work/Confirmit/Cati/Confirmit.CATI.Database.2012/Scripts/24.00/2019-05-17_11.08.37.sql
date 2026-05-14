
IF NOT EXISTS( SELECT 1 FROM BvVersionHistory WHERE Description LIKE '_2016-11-09_12_23_53_attach%')
	INSERT INTO BvVersionHistory(
		Major,
		Minor,
		BranchName,
		ScriptNumber,
		Description, 
		ScriptAppliedDate,
		Duration,
		ScriptText, 
		ScriptOutput, 
		IsAppliedDuringDBCreation, 
		DbUpateUtilityVersion, 
		ActiveUser)
		VALUES(
		0 /*Major*/,
		0 /*Minor*/,
		'' /*BranchName*/,
		-1 /*ScriptNumber*/,
		'_2016-11-09_12_23_53_attach: CATI-1013 Ability to distinguish (and filter) calls that were disabled by FCD (Attach survey databases script without transaction)' /*Description*/, 
		GETUTCDATE() /*ScriptAppliedDate*/,
		0 /*Duration*/,
		'' /*ScriptText*/, 
		'' /*ScriptOutput*/, 
		1 /*IsAppliedDuringDBCreation*/, 
		'24.0.0.0' /*DbUpateUtilityVersion*/, 
		'update_script' /*ActiveUser*/)

IF NOT EXISTS( SELECT 1 FROM BvVersionHistory WHERE Description LIKE '_2018-07-27_17_08_54_attach%')
	INSERT INTO BvVersionHistory(
		Major,
		Minor,
		BranchName,
		ScriptNumber,
		Description, 
		ScriptAppliedDate,
		Duration,
		ScriptText, 
		ScriptOutput, 
		IsAppliedDuringDBCreation, 
		DbUpateUtilityVersion, 
		ActiveUser)
		VALUES(
		0 /*Major*/,
		0 /*Minor*/,
		'' /*BranchName*/,
		-1 /*ScriptNumber*/,
		'_2018-07-27_17_08_54_attach: CATI-2215 Back end support for quota balancing support for multiple quotas' /*Description*/, 
		GETUTCDATE() /*ScriptAppliedDate*/,
		0 /*Duration*/,
		'' /*ScriptText*/, 
		'' /*ScriptOutput*/, 
		1 /*IsAppliedDuringDBCreation*/, 
		'24.0.0.0' /*DbUpateUtilityVersion*/, 
		'update_script' /*ActiveUser*/)


PRINT N'Update complete.';