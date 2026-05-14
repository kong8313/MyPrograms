PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
 
 
GO
PRINT N'Altering [dbo].[BvSpLogin_SpinUp]...';


GO
ALTER  PROCEDURE [dbo].[BvSpLogin_SpinUp]
@PersonSID INTEGER
AS
declare @SurveySID int
declare @PersonMode int
    
	select @SurveySID = SurveySID
	from BvTasks where PersonSID = @PersonSID
    
    if @SurveySID is not null 
    begin
	    select @PersonMode = ManualSelection from BvPerson where sid = @PersonSID

        if(@PersonMode != 2) --is not survey selection
           SET @SurveySID = 0
    
        delete from BvLoginGroup where PersonSID = @PersonSID
        insert into BvLoginGroup select PersonSID, ObjectSID, @SurveySID
            from BvPersonRel where PersonSID = @PersonSID
    end
 
return (0)


GO
PRINT N'Update complete.';
