CREATE VIEW [dbo].[RestView_Group]
	AS SELECT 
		[SID] as GroupId,
		[Name],
		[Description]
	FROM 
		[BvPersonGroup]
