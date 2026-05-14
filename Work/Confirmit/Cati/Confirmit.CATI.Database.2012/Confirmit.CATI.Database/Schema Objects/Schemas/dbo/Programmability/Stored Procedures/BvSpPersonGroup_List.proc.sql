CREATE PROCEDURE [dbo].[BvSpPersonGroup_List]
        @ParentGroupId int 

AS

IF @ParentGroupId = 0 --only root groups
	SELECT DISTINCT
	   BvPersonGroup.SID,
	   BvPersonGroup.Name,
	   BvPersonGroup.Description,
	   BvPersonGroup.InboundCallBehavior,
	   BvPersonGroup.CallTransferBehavior,
	   BvPersonGroup.IsAdministrative
	FROM BvPersonGroup
	LEFT JOIN BvMembership ON BvPersonGroup.SID = BvMembership.ObjectSID AND
							  BvMembership.ContainerSID = @ParentGroupId
	WHERE BvMembership.ObjectSID IS NULL
ELSE --child groups for passed parent group
	SELECT DISTINCT
	   BvPersonGroup.SID,
	   BvPersonGroup.Name,
	   BvPersonGroup.Description,
	   BvPersonGroup.InboundCallBehavior,
	   BvPersonGroup.CallTransferBehavior,
	   BvPersonGroup.IsAdministrative
	FROM BvPersonGroup
	INNER JOIN BvMembership ON BvPersonGroup.SID = BvMembership.ObjectSID AND
							  BvMembership.ContainerSID = @ParentGroupId