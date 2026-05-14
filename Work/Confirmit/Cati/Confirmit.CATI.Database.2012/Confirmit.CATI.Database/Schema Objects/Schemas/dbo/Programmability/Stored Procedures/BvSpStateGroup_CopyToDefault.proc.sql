CREATE PROCEDURE [dbo].[BvSpStateGroup_CopyToDefault]
 @DefaultStateGroupId INT,
 @SourceStateGroupId INT
 AS
  UPDATE d
  SET
  [Priority] = s.[Priority],
  [Name] = s.[Name],
  [DA] = s.[DA],
  [FcdAction] = s.[FcdAction],
  [AaporCode] = s.[AaporCode]
  FROM BvState d INNER JOIN BvState s ON d.StateID = s.StateID
  WHERE d.StateGroupID = @DefaultStateGroupId AND s.StateGroupID = @SourceStateGroupId