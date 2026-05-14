CREATE  PROCEDURE [dbo].[BvSpLogin_SpinUp]
@PersonSID INTEGER
AS
declare @SurveySID int
declare @PersonMode int
declare @DialType TINYINT    
	select @SurveySID = SurveySID,
           @DialType = DialTypeId
	from BvTasks where PersonSID = @PersonSID
    
    if @SurveySID is not null 
    begin
	    select @PersonMode = ManualSelection from BvPerson where sid = @PersonSID

        if(@PersonMode != 2) --is not survey selection
           SET @SurveySID = 0
    
        delete from BvLoginGroup where PersonSID = @PersonSID
        insert into BvLoginGroup select PersonSID, ObjectSID, @SurveySID, @DialType
            from BvPersonRel where PersonSID = @PersonSID
    end
 
return (0)