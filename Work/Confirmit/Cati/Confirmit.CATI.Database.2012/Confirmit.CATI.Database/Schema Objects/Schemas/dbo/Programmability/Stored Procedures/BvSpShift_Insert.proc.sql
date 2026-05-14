CREATE PROCEDURE [dbo].[BvSpShift_Insert]
        @OwnerSID int,
        @ID int,
        @CycleType int,
        @StartDayOfWeek int,
        @StartTime datetime,
        @FinishDayOfWeek int,
        @FinishTime datetime,
        @ShiftTypeID int

AS
DECLARE @Rows int

SELECT  @Rows = COUNT(*)
    FROM    BvShift
    WHERE   ID = @ID
    AND OwnerSID = @OwnerSID
IF @Rows <> 0
--  return 50001    /* BVDBS_STORED_PROCEDURE_DUPLICATED_OBJECT */
    return 0

SELECT  @Rows = COUNT(*)
    FROM    BvShiftType
    WHERE   ObjectID = @ShiftTypeID
    AND OwnerSID = @OwnerSID
IF @Rows <> 1
--  return 50002    /* BVDBS_STORED_PROCEDURE_OBJECT_NOT_EXIST */
    return 0

INSERT  BvShift( 
        OwnerSID, 
        ID, 
        CycleType,
        StartDayOfWeek,
        StartTime,
        FinishDayOfWeek,
        FinishTime,
        ShiftTypeID )
    VALUES( @OwnerSID, 
        @ID, 
        @CycleType,
        @StartDayOfWeek,
        @StartTime,
        @FinishDayOfWeek,
        @FinishTime,
        @ShiftTypeID )
return  @ID