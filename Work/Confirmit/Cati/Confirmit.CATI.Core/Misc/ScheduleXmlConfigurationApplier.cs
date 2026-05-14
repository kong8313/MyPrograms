using System;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;

namespace Confirmit.CATI.Core.Misc
{
    public struct ShiftWithTz
    {
        public Shift shift;
        public int tzId;
    }

    public struct ExclusionWithTz
    {
        public Exclusion exclusion;
        public int tzId;
    }

    public class ScheduleXmlConfigurationData
    {
        public List<ShiftWithTz> DeletedShifts { get; set; }
        public List<ShiftWithTz> UpdatedShifts { get; set; }
        public List<ShiftWithTz> CreatedShifts { get; set; }
        public List<ShiftType> DeletedShiftTypes { get; set; }
        public List<ShiftType> UpdatedShiftTypes { get; set; }
        public List<ShiftType> CreatedShiftTypes { get; set; }
        public List<ExclusionWithTz> DeletedExclusions { get; set; }
        public List<ExclusionWithTz> UpdatedExclusions { get; set; }
        public List<ExclusionWithTz> CreatedExclusions { get; set; }
    }

    public class ScheduleXmlConfigurationApplier
    {
        public enum DifferenceState
        {
            NotChanged = 0,
            Changed = 1,
            NotExists = 2
        }

        private readonly int m_scheduleId;
        private static readonly DateTime m_defaultDateForShift = DateTime.Parse( "2007-12-16T00:00:00" );

        private int ScheduleID
        {
            get
            {
                return m_scheduleId;
            }
        }

        private static DateTime DefaultDateForShift
        {
            get
            {
                return m_defaultDateForShift;
            }
        }

        private static int ExclusionShiftTypeId
        {
            get
            {
                return Int32.MaxValue;
            }
        }

        public ScheduleXmlConfigurationApplier( int scheduleId )
        {
            m_scheduleId = scheduleId;
        }

        public void Apply(
            Schedule scheduleNew,
            Schedule scheduleOld )
        {
            if (scheduleNew.Shifts.Count == 0)
            {
                throw new UserMessageException(
                    string.Format(
                        "Script '{0}' should contain at least one shift",
                        scheduleNew.Name));
            }

            var configurationPlan = CreatePlan(
                scheduleNew,
                scheduleOld );

            RenameDeletingShiftTypesToAvoidConflicts( 
                configurationPlan.DeletedShiftTypes );

            //
            // update shift types
            foreach (var shiftType in configurationPlan.UpdatedShiftTypes)
            {
                UpdateShiftType(shiftType);
            }

            //
            // create shift types
            foreach ( var shiftType in configurationPlan.CreatedShiftTypes )
            {
                CreateShiftType( shiftType );
            }

            //
            // delete shifts
            foreach ( var shift in configurationPlan.DeletedShifts )
            {
                DeleteShift( shift );
            }

            //
            // delete exclusions
            foreach ( var exclusion in  configurationPlan.DeletedExclusions )
            {
                DeleteExclusion( exclusion );
            }

            //
            // update shifts
            foreach ( var shift in configurationPlan.UpdatedShifts )
            {
                UpdateShift( shift );
            }

            //
            // update exclusions
            foreach ( var exclusion in configurationPlan.UpdatedExclusions )
            {
                UpdateExclusion( exclusion );
            }

            //
            // create shifts
            foreach ( var shift in configurationPlan.CreatedShifts )
            {
                CreateShift( shift );
            }

            //
            // create exclusions
            foreach ( var exclusion in configurationPlan.CreatedExclusions )
            {
                CreateExclusion( exclusion );
            }

            //
            // delete shift types
            foreach ( var shiftType in configurationPlan.DeletedShiftTypes )
            {
                DeleteShiftType( shiftType );
            }
         
            /*
             * Store  Custom parameters
             */

            //prepare batch of custom parameter s
            int batchID = m_scheduleId;
            BvSpCreateTransferBatchIDAdapter.ExecuteNonQuery(0, out batchID );
            foreach( var customParameter in scheduleNew.CustomParameters )
            {
                BvSpScheduleParam_PrepareAdapter.ExecuteNonQuery(
                    batchID,
                    customParameter.Id,
                    customParameter.Name,
                    customParameter.Description,
                    (int)customParameter.Type,
                    customParameter.Value);
            }

            //Apply batch of custom parameters

            BvSpScheduleParam_LaunchAdapter.ExecuteNonQuery(m_scheduleId, batchID);
            BvScheduleParamCache.Instance.OnTableChanged();
            
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishShiftsUpdated();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishScheduleParamsUpdated();
        }

        //
        // methods to modify specific entities in database
        private void RenameDeletingShiftTypesToAvoidConflicts(
            IEnumerable<ShiftType> shiftTypes)
        {
            foreach ( var shiftType in  shiftTypes )
            {
                string newName = "Deleting " + Guid.NewGuid();
                shiftType.Name = newName;

                UpdateShiftType( shiftType );
            }
        }

        private void CreateShift(
            ShiftWithTz shiftWithTz )
        {
            var shiftData = shiftWithTz.shift.GetDataForTimezone( shiftWithTz.tzId );

            DateTime startTime = CalculateDateForShift(
                    (TimeSpan) shiftData.StartTime,
                    (int) shiftData.StartDayOfWeek );

            DateTime endTime = CalculateDateForShift(
                    (TimeSpan) shiftData.EndTime,
                    (int) shiftData.EndDayOfWeek );

            int shiftId = IdToDbShiftId( (int) shiftWithTz.shift.Id );

            if ( shiftWithTz.tzId > 0 )
            {
                BvSpTimezoneShift_InsertAdapter.ExecuteNonQuery( 
                    ScheduleID,
                    shiftId,
                    shiftWithTz.tzId,
                    (int) shiftData.StartDayOfWeek,
                    startTime,
                    (int) shiftData.EndDayOfWeek,
                    endTime );
            }
            else
            {
                int internalShiftTypeId;

                BvSpShiftType_GetIDAdapter.ExecuteNonQuery( 
                    ScheduleID,
                    shiftWithTz.shift.ShiftTypeId,
                    out internalShiftTypeId );

                BvSpShift_InsertAdapter.ExecuteNonQuery( 
                    ScheduleID,
                    shiftId,
                    (int) ShiftCycleType.Shift,
                    (int) shiftData.StartDayOfWeek,
                    startTime,
                    (int) shiftData.EndDayOfWeek,
                    endTime,
                    internalShiftTypeId );
            }
        }

        private void UpdateShift(
            ShiftWithTz shiftWithTz )
        {
            var shiftData = shiftWithTz.shift.GetDataForTimezone( shiftWithTz.tzId );

            DateTime startTime = CalculateDateForShift(
                    (TimeSpan) shiftData.StartTime,
                    (int) shiftData.StartDayOfWeek );

            DateTime endTime = CalculateDateForShift(
                (TimeSpan) shiftData.EndTime,
                (int) shiftData.EndDayOfWeek );

            int shiftId = IdToDbShiftId( (int) shiftWithTz.shift.Id );

            if ( shiftWithTz.tzId > 0 )
            {                
                BvSpTimezoneShift_UpdateAdapter.ExecuteNonQuery( 
                    ScheduleID,
                    shiftId,
                    shiftWithTz.tzId,
                    (int) shiftData.StartDayOfWeek,
                    startTime,
                    (int) shiftData.EndDayOfWeek,
                    endTime,
                    (int) BvdbsActionMode.BVDBS_ACTION_MODE_STRONG );
            }
            else
            {
                int internalShiftTypeId;

                BvSpShiftType_GetIDAdapter.ExecuteNonQuery(
                    ScheduleID,
                    shiftWithTz.shift.ShiftTypeId,
                    out internalShiftTypeId );

                BvSpShift_UpdateAdapter.ExecuteNonQuery( 
                    ScheduleID,
                    shiftId,
                    shiftId,
                    (int) ShiftCycleType.Shift,
                    (int) shiftData.StartDayOfWeek,
                    startTime,
                    (int) shiftData.EndDayOfWeek,
                    endTime,
                    internalShiftTypeId,
                    (int) BvdbsActionMode.BVDBS_ACTION_MODE_STRONG );
            }
        }

        private void DeleteShift(
            ShiftWithTz shiftWithTz )
        {
            int shiftId = IdToDbShiftId( (int) shiftWithTz.shift.Id );

            if ( shiftWithTz.tzId > 0 )
            {
                BvSpTimezoneShift_DeleteAdapter.ExecuteNonQuery( 
                    ScheduleID,
                    shiftId,
                    shiftWithTz.tzId,
                    (int) BvdbsActionMode.BVDBS_ACTION_MODE_STRONG );
            }
            else
            {
                BvSpShift_DeleteAdapter.ExecuteNonQuery( 
                    ScheduleID,
                    shiftId,
                    (int) BvdbsActionMode.BVDBS_ACTION_MODE_STRONG );
            }
        }

        private void CreateExclusion(
            ExclusionWithTz exclusionWithTz )
        {
            var exclusionData = exclusionWithTz.exclusion.GetDataForTimezone( exclusionWithTz.tzId );

           int exclusionId = IdToDbExclusionId( (int) exclusionWithTz.exclusion.Id );

            if ( exclusionWithTz.tzId > 0 )
            {
                BvSpTimezoneShift_InsertAdapter.ExecuteNonQuery( 
                    ScheduleID,
                    exclusionId,
                    exclusionWithTz.tzId,
                    0, // should be 0 for exclusion
                    exclusionData.StartDate,
                    0, // should be 0 for exclusion
                    exclusionData.EndDate );
            }
            else
            {
                int internalShiftTypeId;

                BvSpShiftType_GetIDAdapter.ExecuteNonQuery(
                    ScheduleID,
                    ExclusionShiftTypeId,
                    out internalShiftTypeId );

                BvSpShift_InsertAdapter.ExecuteNonQuery( 
                    ScheduleID,
                    exclusionId,
                    (int) ShiftCycleType.Exclusion,
                    0, // should be 0 for exclusion
                    exclusionData.StartDate,
                    0, // should be 0 for exclusion
                    exclusionData.EndDate,
                    internalShiftTypeId /* fictive shiftType for exclusion */ );
            }
        }

        private void UpdateExclusion(
            ExclusionWithTz exclusionWithTz )
        {
            var exclusionData = exclusionWithTz.exclusion.GetDataForTimezone( exclusionWithTz.tzId );

            int exclusionId = IdToDbExclusionId( (int) exclusionWithTz.exclusion.Id );

            if ( exclusionWithTz.tzId > 0 )
            {
                BvSpTimezoneShift_UpdateAdapter.ExecuteNonQuery(
                    ScheduleID,
                    exclusionId,
                    exclusionWithTz.tzId,
                    0, // should be 0 for exclusion
                    exclusionData.StartDate,
                    0, // should be 0 for exclusion
                    exclusionData.EndDate,
                    (int) BvdbsActionMode.BVDBS_ACTION_MODE_STRONG );
            }
            else
            {
                int internalShiftTypeId;

                BvSpShiftType_GetIDAdapter.ExecuteNonQuery(
                    ScheduleID,
                    ExclusionShiftTypeId,
                    out internalShiftTypeId );

                BvSpShift_UpdateAdapter.ExecuteNonQuery(
                    ScheduleID,
                    exclusionId,
                    exclusionId,
                    (int) ShiftCycleType.Exclusion,
                    0, // should be 0 for exclusion
                    exclusionData.StartDate,
                    0, // should be 0 for exclusion
                    exclusionData.EndDate,
                    internalShiftTypeId, // fictive shiftType for exclusion
                    (int) BvdbsActionMode.BVDBS_ACTION_MODE_STRONG );
            }
        }

        private void DeleteExclusion(
            ExclusionWithTz exclusionWithTz )
        {
            int exclusionId = IdToDbExclusionId( (int) exclusionWithTz.exclusion.Id );

            if ( exclusionWithTz.tzId > 0 )
            {
                BvSpTimezoneShift_DeleteAdapter.ExecuteNonQuery( 
                    ScheduleID,
                    exclusionId,
                    exclusionWithTz.tzId,
                    (int) BvdbsActionMode.BVDBS_ACTION_MODE_STRONG );
            }
            else
            {
                BvSpShift_DeleteAdapter.ExecuteNonQuery( 
                    ScheduleID,
                    exclusionId,
                    (int) BvdbsActionMode.BVDBS_ACTION_MODE_STRONG );
            }
        }

        private void CreateShiftType(
            ShiftType shiftType )
        {
            int shiftTypeId = IdToDbShiftTypeId( (int) shiftType.Id );

            BvSpShiftType_InsertAdapter.ExecuteNonQuery( 
                ScheduleID,
                shiftTypeId,
                shiftType.Name,
                shiftType.ColorInt,
                0 /* ObjectID not used */);
        }

        private void UpdateShiftType( 
            ShiftType shiftType )
        {
            int shiftTypeId = IdToDbShiftTypeId( (int) shiftType.Id );

            BvSpShiftType_UpdateAdapter.ExecuteNonQuery(
                ScheduleID,
                shiftTypeId,
                shiftTypeId,
                shiftType.Name,
                shiftType.ColorInt,
                (int) BvdbsActionMode.BVDBS_ACTION_MODE_STRONG,
                0 /*ObjectID not used */);
        }

        private void DeleteShiftType(
            ShiftType shiftType )
        {
            int shiftTypeId = IdToDbShiftTypeId( (int) shiftType.Id );

            BvSpShiftType_DeleteAdapter.ExecuteNonQuery( 
                ScheduleID,
                shiftTypeId,
                (int) BvdbsActionMode.BVDBS_ACTION_MODE_STRONG );
        }

        //
        // methods to create change Db entities plans
        private static ScheduleXmlConfigurationData CreatePlan(
            Schedule scheduleNew,
            Schedule scheduleOld)
        {
            var configurationPlan = new ScheduleXmlConfigurationData();

            //
            // expand shifts and exclusions to be corresponded to database:
            // single shift/exclusion object for each timezone
            var oldShifts = ExpandShiftListToShiftsWithTz( scheduleOld.Shifts );
            var newShifts = ExpandShiftListToShiftsWithTz( scheduleNew.Shifts );
            var oldExclusions = ExpandExclusionListToExclusionsWithTz( scheduleOld.Exclusions );
            var newExclusions = ExpandExclusionListToExclusionsWithTz( scheduleNew.Exclusions );

            //
            // processing shift types
            configurationPlan.DeletedShiftTypes = new List<ShiftType>();
            configurationPlan.UpdatedShiftTypes = new List<ShiftType>();
            configurationPlan.CreatedShiftTypes = new List<ShiftType>();

            CreatePlanForShiftTypes( 
                scheduleOld.ShiftTypes,
                scheduleNew.ShiftTypes,
                ref configurationPlan );

            //
            // processing shifts
            configurationPlan.DeletedShifts = new List<ShiftWithTz>();
            configurationPlan.UpdatedShifts = new List<ShiftWithTz>();
            configurationPlan.CreatedShifts = new List<ShiftWithTz>();

            CreatePlanForShifts(
                oldShifts,
                newShifts,
                ref configurationPlan );

            //
            // processing exclusions
            configurationPlan.DeletedExclusions = new List<ExclusionWithTz>();
            configurationPlan.UpdatedExclusions = new List<ExclusionWithTz>();
            configurationPlan.CreatedExclusions = new List<ExclusionWithTz>();

            CreatePlanForExclusions(
                oldExclusions,
                newExclusions,
                ref configurationPlan );

            return configurationPlan;
        }
        
        private static void CreatePlanForShiftTypes(
            ShiftTypeCollection shiftTypesOld,
            ShiftTypeCollection shiftTypesNew,
            ref ScheduleXmlConfigurationData configurationPlan )
        {
            if ( shiftTypesOld == null ) // first launch
            {
                foreach ( var shiftType in shiftTypesNew )
                {
                    configurationPlan.CreatedShiftTypes.Add( shiftType );
                }

                return;
            }

            foreach ( var shiftType in shiftTypesOld )
            {
                if ( GetShiftTypeState(
                    shiftTypesNew,
                    shiftType ) == DifferenceState.NotExists )
                {
                    configurationPlan.DeletedShiftTypes.Add( shiftType );
                }
            }

            foreach ( var shiftType in shiftTypesNew )
            {
                switch ( GetShiftTypeState( shiftTypesOld, shiftType ) )
                {
                    case DifferenceState.Changed:
                        configurationPlan.UpdatedShiftTypes.Add( shiftType );
                        break;
                    case DifferenceState.NotExists:
                        configurationPlan.CreatedShiftTypes.Add( shiftType );
                        break;
                }
            }
        }

        private static void CreatePlanForShifts(
            List<ShiftWithTz> oldShifts,
            List<ShiftWithTz> newShifts,
            ref ScheduleXmlConfigurationData configurationPlan )
        {
            foreach ( var shiftWithTz in oldShifts )
            {
                if ( GetShiftState(
                    newShifts,
                    shiftWithTz ) == DifferenceState.NotExists )
                {
                    //
                    // shifts should be deleted in opposite order
                    if (shiftWithTz.tzId <= 0)
                    {
                        configurationPlan.DeletedShifts.Add( shiftWithTz );
                    }
                    else
                    {
                        configurationPlan.DeletedShifts.Insert( 0, shiftWithTz );
                    }
                }
            }

            foreach ( var shiftWithTz in newShifts )
            {
                switch ( GetShiftState( oldShifts, shiftWithTz ) )
                {
                    case DifferenceState.Changed:
                        if ( shiftWithTz.tzId > 0 )
                        {
                            configurationPlan.UpdatedShifts.Add( shiftWithTz );
                        }
                        else
                        {
                            configurationPlan.UpdatedShifts.Insert( 0, shiftWithTz );
                        }
                        break;
                    case DifferenceState.NotExists:
                        if ( shiftWithTz.tzId > 0 )
                        {
                            configurationPlan.CreatedShifts.Add( shiftWithTz );
                        }
                        else
                        {
                            configurationPlan.CreatedShifts.Insert( 0, shiftWithTz );
                        }
                        break;
                }
            }
        }

        private static void CreatePlanForExclusions(
            List<ExclusionWithTz> oldExclusions,
            List<ExclusionWithTz> newExclusions,
            ref ScheduleXmlConfigurationData configurationPlan )
        {
            foreach ( var exclusionWithTz in oldExclusions )
            {
                if ( GetExclusionState(
                    newExclusions,
                    exclusionWithTz ) == DifferenceState.NotExists )
                {
                    if ( exclusionWithTz.tzId > 0 )
                    {
                        configurationPlan.DeletedExclusions.Add( exclusionWithTz );
                    }
                    else
                    {
                        configurationPlan.DeletedExclusions.Insert( 0, exclusionWithTz );
                    }
                }
            }

            foreach ( var exclusionWithTz in newExclusions )
            {
                switch ( GetExclusionState( oldExclusions, exclusionWithTz ) )
                {
                    case DifferenceState.Changed:
                        if ( exclusionWithTz.tzId > 0 )
                        {
                            configurationPlan.UpdatedExclusions.Add( exclusionWithTz );
                        }
                        else
                        {
                            configurationPlan.UpdatedExclusions.Insert( 0, exclusionWithTz );
                        }
                        break;
                    case DifferenceState.NotExists:
                        if ( exclusionWithTz.tzId > 0 )
                        {
                            configurationPlan.CreatedExclusions.Add( exclusionWithTz );
                        }
                        else
                        {
                            configurationPlan.CreatedExclusions.Insert( 0, exclusionWithTz );
                        }
                        break;
                }
            }
        }

        //
        // methods to check actual state of entities:
        // was it changed, removed or added in new schedule
        private static DifferenceState GetShiftTypeState(
            ShiftTypeCollection shiftTypes,
            ShiftType shiftTypeToCheck)
        {
            foreach (var shiftType in shiftTypes)
            {
                if ( shiftType.Id == shiftTypeToCheck.Id )
                {
                    if ( AreShiftTypesEqual( shiftType, shiftTypeToCheck ) )
                    {
                        return DifferenceState.NotChanged;
                    }
                    else
                    {
                        return DifferenceState.Changed;
                    }
                }
            }

            return DifferenceState.NotExists;
        }

        private static DifferenceState GetShiftState(
            List<ShiftWithTz> shifts,
            ShiftWithTz shiftToCheck )
        {
            foreach ( var shift in shifts )
            {
                if ( shift.shift.Id == shiftToCheck.shift.Id && shift.tzId == shiftToCheck.tzId )
                {
                    if ( AreShiftsEqual( shift, shiftToCheck ) )
                    {
                        return DifferenceState.NotChanged;
                    }
                    else
                    {
                        return DifferenceState.Changed;
                    }
                }
            }

            return DifferenceState.NotExists;
        }

        private static DifferenceState GetExclusionState(
            List<ExclusionWithTz> exclusionCollection,
            ExclusionWithTz exclusionToCheck )
        {
            foreach ( var exclusion in exclusionCollection )
            {
                if ( exclusion.exclusion.Id == exclusionToCheck.exclusion.Id && exclusion.tzId == exclusionToCheck.tzId )
                {
                    if ( AreExclusionsEqual( exclusion, exclusionToCheck ) )
                    {
                        return DifferenceState.NotChanged;
                    }
                    else
                    {
                        return DifferenceState.Changed;
                    }
                }
            }

            return DifferenceState.NotExists;
        }

        //
        // expand shifts to shifts with Tz methods
        private static List<ShiftWithTz> ExpandShiftListToShiftsWithTz( 
            ShiftCollection shiftCollection )
        {
            var result = new List<ShiftWithTz>();

            if ( shiftCollection == null )
            {
                return result;
            }

            foreach ( var shift in shiftCollection )
            {
                result.AddRange(
                    ExpandShiftToShiftsWithTz( shift ) );
            }

            return result;
        }

        private static List<ExclusionWithTz> ExpandExclusionListToExclusionsWithTz(
            ExclusionCollection exclustionCollection )
        {
            var result = new List<ExclusionWithTz>();

            if ( exclustionCollection == null )
            {
                return result;
            }

            foreach ( var exclusion in exclustionCollection )
            {
                result.AddRange(
                    ExpandExclusionToExclusionWithTz( exclusion ) );
            }

            return result;
        }

        private static List<ShiftWithTz> ExpandShiftToShiftsWithTz(Shift shift)
        {
            var result = new List<ShiftWithTz>();

            if ( shift.Timezones.Length == 0 )
            {
                throw new System.Exception( string.Format(
                    "list of timezones of shift {0} should not be empty",
                    shift.Id ) );
            }

            bool defaultTimezoneExists = false;

            foreach (var timezoneId in shift.GetTimezoneIds() )
            {
                if ( timezoneId <= 0 )
                {
                    defaultTimezoneExists = true;
                }

                var shiftWithTz = new ShiftWithTz
                {
                    shift = shift,
                    tzId = timezoneId
                };

                result.Add( shiftWithTz );
            }

            //
            // temporaly UGLY code!!!
            // should be refactored ASAP
            if ( !defaultTimezoneExists )
            {
                var newTimezonesList = new List<BaseTimezoneData<ShiftData>>(shift.Timezones);
                newTimezonesList.Add( new BaseTimezoneData<ShiftData>(
                    -1,
                    new ShiftData()
                        {
                            StartDayOfWeek = DayOfWeek.Sunday,
                            StartTime = TimeSpan.FromSeconds( 0 ),
                            EndDayOfWeek = DayOfWeek.Sunday,
                            EndTime = TimeSpan.FromSeconds( 0 )
                        }
                   ) );
                shift.Timezones = newTimezonesList.ToArray();

                var shiftWithTz = new ShiftWithTz
                {
                    shift = shift,
                    tzId = -1 // default timezone
                };

                result.Add( shiftWithTz );
            }

            return result;
        }

        private static List<ExclusionWithTz> ExpandExclusionToExclusionWithTz( Exclusion exclusion )
        {
            var result = new List<ExclusionWithTz>();

            if ( exclusion.Timezones.Length == 0 )
            {
                throw new System.Exception( string.Format(
                    "list of timezones of exclusion {0} should not be empty",
                    exclusion.Id ) );
            }

            bool defaultTimezoneExists = false;

            foreach ( var timezoneId in exclusion.GetTimezoneIds() )
            {
                if ( timezoneId <= 0 )
                {
                    defaultTimezoneExists = true;
                }

                var exclusionWithTz = new ExclusionWithTz
                {
                    exclusion = exclusion,
                    tzId = timezoneId
                };

                result.Add( exclusionWithTz );
            }

            //
            // temporaly UGLY code!!!
            // should be refactored ASAP
            if ( !defaultTimezoneExists )
            {
                var newTimezonesList = new List<BaseTimezoneData<ExclusionData>>( exclusion.Timezones );
                newTimezonesList.Add( new BaseTimezoneData<ExclusionData>(
                    -1,
                    new ExclusionData()
                    {
                        StartDate = DefaultDateForShift,
                        EndDate = DefaultDateForShift
                    }
                   ) );
                exclusion.Timezones = newTimezonesList.ToArray();

                var exclusionWithTz = new ExclusionWithTz
                {
                    exclusion = exclusion,
                    tzId = -1 // default timezone
                };

                result.Add( exclusionWithTz );
            }

            return result;
        }

        //
        // aux methods
        private static DateTime CalculateDateForShift(
            TimeSpan date,
            int days)
        {
            return DefaultDateForShift + date + TimeSpan.FromDays( days );
        }

        private static int IdToDbShiftId(int sid)
        {
            return ( sid * 2 + 1 );
        }

        private static int IdToDbExclusionId(int sid)
        {
            return ( sid * 2 );
        }

        private static int IdToDbShiftTypeId(int sid)
        {
            return ( sid == 0 ) ? ExclusionShiftTypeId : sid;
        }

        //
        // methods to compare entities
        private static bool AreShiftsEqual(
            ShiftWithTz shiftWithTz1,
            ShiftWithTz shiftWithTz2 )
        {
            Shift shift1 = shiftWithTz1.shift;
            Shift shift2 = shiftWithTz2.shift;

            if ( shift1.Id != shift2.Id ||
                shift1.ShiftTypeId != shift2.ShiftTypeId )
            {
                return false;
            }

            var shiftData1 = shiftWithTz1.shift.GetDataForTimezone( shiftWithTz1.tzId );
            var shiftData2 = shiftWithTz2.shift.GetDataForTimezone( shiftWithTz2.tzId );

            return ( shiftData1.EndDayOfWeek == shiftData2.EndDayOfWeek &&
                shiftData1.EndTime == shiftData2.EndTime &&
                shiftData1.StartDayOfWeek == shiftData2.StartDayOfWeek &&
                shiftData1.StartTime == shiftData2.StartTime );
        }

        private static bool AreExclusionsEqual(
            ExclusionWithTz exclusionWithTz1,
            ExclusionWithTz exclusionWithTz2 )
        {
            Exclusion exclusion1 = exclusionWithTz1.exclusion;
            Exclusion exclusion2 = exclusionWithTz2.exclusion;

            if ( exclusion1.Id != exclusion2.Id ||
                exclusion1.ShiftTypeId != exclusion2.ShiftTypeId )
            {
                return false;
            }

            var shiftData1 = exclusionWithTz1.exclusion.GetDataForTimezone( exclusionWithTz1.tzId );
            var shiftData2 = exclusionWithTz2.exclusion.GetDataForTimezone( exclusionWithTz2.tzId );

            return ( shiftData1.EndDate == shiftData2.EndDate &&
                shiftData1.StartDate == shiftData2.StartDate );
        }

        private static bool AreShiftTypesEqual(
            ShiftType shiftType1,
            ShiftType shiftType2)
        {
            return ( shiftType1.Name == shiftType2.Name &&
                     shiftType1.Color == shiftType2.Color &&
                     shiftType1.Id == shiftType2.Id );
        }
    }
}
