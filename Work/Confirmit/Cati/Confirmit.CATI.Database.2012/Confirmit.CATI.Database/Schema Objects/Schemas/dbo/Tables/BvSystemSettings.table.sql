CREATE TABLE BvSystemSettings
	(
		[SystemName] NVARCHAR(256) NOT NULL,
		[DisplayName] NVARCHAR(256),
		[Group] NVARCHAR(256) NOT NULL,
		[Description] NVARCHAR(MAX) NOT NULL,
		[Type] INT NOT NULL,
		[Hidden] BIT NOT NULL,
		[Value] NVARCHAR(MAX)
	)
	GO