CREATE PROCEDURE [dbo].[BvSpSystemSetting_Update]
	@SystemName AS NVARCHAR(256),
	@Value AS NVARCHAR(MAX)
AS
	MERGE BvSystemSettings as target
	USING ( SELECT @SystemName ) AS source( SystemName )
	ON target.SystemName = source.SystemName
	WHEN MATCHED THEN 
        UPDATE SET Value = @Value
	WHEN NOT MATCHED THEN	
		INSERT ( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
		VALUES(  @SystemName, '<NULL>', '<NULL>', '<NULL>', 0, 0, @Value );
