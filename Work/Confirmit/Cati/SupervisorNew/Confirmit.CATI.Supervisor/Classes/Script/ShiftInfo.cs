using System;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;

namespace Confirmit.CATI.Supervisor.Classes.Script
{
    /// <summary>
    /// Represents intermediate shift class.
    /// used for connect shifts and grid
    /// </summary>
    public class ShiftInfo : IShiftInfo, IComparable<ShiftInfo>
    {
        #region Fields

        private int? m_id;
        private int m_shiftTypeId;
        private ShiftStatus m_shiftStatus;
        private DayOfWeek m_startDay;
        private TimeSpan m_startTime;
        private DayOfWeek m_endDay;
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
        /// Returns true if shift has respondent zone othewise false
        /// </summary>
        public bool HasRespondentTimeZone
        {
            get;
            set;
        }

        /// <summary>
        /// Shift start day of week.
        /// </summary>
        [RowRead( "StartDayName" )]
        public DayOfWeek StartDay
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
        public DayOfWeek EndDay
        {
            get { return m_endDay; }
            set { m_endDay = value; }
        }

        /// <summary>
        /// Shift end time.
        /// </summary>
        [RowRead("EndTimeToString")]
        public TimeSpan EndTime
        {
            get { return m_endTime; }
            set { m_endTime = value; }
        }

        /// <summary>
        /// Shift string representation of day name
        /// </summary>
        public string StartDayName
        {
            get
            {
                return m_startDay.ToString();
            }
        }

        /// <summary>
        /// Shift string representation of day name
        /// </summary>
        public string EndDayName
        {
            get
            {
                return m_endDay.ToString();
            }
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

        #endregion

        #region IComparable<ShiftInfo> Members

        public int CompareTo( ShiftInfo other )
        {
            //Temporary comparison, will be remake .....
            if (m_startDay == other.m_startDay && m_startTime == other.m_startTime)
            {
                return 0;
            }
            bool compare = SchedulingUtilities.IsLessOrEqualPare( m_startDay, m_startTime, other.m_startDay, other.m_startTime );
            return compare ? -1 : 1;
        }

        #endregion
    }

}
