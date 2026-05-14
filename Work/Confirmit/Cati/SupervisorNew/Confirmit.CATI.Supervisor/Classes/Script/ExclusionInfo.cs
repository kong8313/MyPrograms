using System;

namespace Confirmit.CATI.Supervisor.Classes.Script
{
    public class ExclusionInfo : IShiftInfo, IComparable<ExclusionInfo>
    {
        #region Fields

        private int? m_id;
        private int m_shiftTypeId;
        private ShiftStatus m_shiftStatus;
        private DateTime m_startDay;
        private TimeSpan m_startTime;
        private DateTime m_endDay;
        private TimeSpan m_endTime;

        #endregion

        #region Properties

        /// <summary>
        /// Unique identifier.
        /// </summary>
        [RowRead( "Id" )]
        public int? Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        /// <summary>
        /// Shift type identifier.
        /// </summary>
        [RowRead( "ShiftTypeId" )]
        public int ShiftTypeId
        {
            get { return m_shiftTypeId; }
            set { m_shiftTypeId = value; }
        }

        public ShiftStatus ShiftStatus
        {
            get
            {
                return m_shiftStatus;
            }
            set
            {
                m_shiftStatus = value;
            }
        }

        /// <summary>
        /// Shift start day of week.
        /// </summary>
        [RowRead( "StartDayName" )]
        public DateTime StartDay
        {
            get { return m_startDay; }
            set { m_startDay = value; }
        }

        /// <summary>
        /// Shift start time.
        /// </summary>
        [RowRead( "StartTimeToString" )]
        public TimeSpan StartTime
        {
            get { return m_startTime; }
            set { m_startTime = value; }
        }

        /// <summary>
        /// Shift end day of week.
        /// </summary>
        [RowRead( "EndDayName" )]
        public DateTime EndDay
        {
            get { return m_endDay; }
            set { m_endDay = value; }
        }

        /// <summary>
        /// Shift end time.
        /// </summary>
        [RowRead( "EndTimeToString" )]
        public TimeSpan EndTime
        {
            get { return m_endTime; }
            set { m_endTime = value; }
        }

        /// <summary>
        /// Shift string representation of day name
        /// Needed because we used one grid for shifts and exclusion
        /// </summary>
        public string StartDayName
        {
            get { return m_startDay.Date.ToShortDateString(); }
        }

        /// <summary>
        /// Shift string representation of day name
        /// Needed because we used one grid for shifts and exclusion
        /// </summary>
        public string EndDayName
        {
            get { return m_endDay.Date.ToShortDateString(); }
        }

        public string StartTimeToString
        {
            get
            {
                return string.Format( "{0:00}:{1:00}", m_startTime.Hours, m_startTime.Minutes );
            }
        }

        public string EndTimeToString
        {
            get
            {
                return string.Format( "{0:00}:{1:00}", m_endTime.Hours, m_endTime.Minutes );
            }
        }

        /// <summary>
        /// This property is not used for exclusions but it must be present 
        /// because the same grid is used as for shifts list as for exclusions list.
        /// </summary>
        public bool HasRespondentTimeZone
        {
            get;
            set;
        }

        #endregion

        #region IComparable<ExclusionInfo> Members

        public int CompareTo( ExclusionInfo other )
        {
            DateTime currentDateTime = m_startDay.AddTicks( m_startTime.Ticks );
            DateTime otherDateTime = other.m_startDay.AddTicks( other.m_startTime.Ticks );
            return currentDateTime.CompareTo( otherDateTime );
        }

        #endregion
    }

}
