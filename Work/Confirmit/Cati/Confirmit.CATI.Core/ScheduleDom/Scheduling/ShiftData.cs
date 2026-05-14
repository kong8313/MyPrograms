using System;
using System.Xml.Serialization;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ScheduleDom.Resources;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
    /// <summary>
    /// Represents shift data. Shift data contain start day of week, start time, end day of week and 
    /// end time.
    /// </summary>
    [Serializable]
    public struct ShiftData : IIntersectable<ShiftData>
    {
        private DayOfWeek? m_startDayOfWeek;
        private TimeSpan? m_startTime;
        private DayOfWeek? m_endDayOfWeek;
        private TimeSpan? m_endTime;

        /// <summary>
        /// Initializes a new instance of the ShiftData structure to a specified 
        /// start day of week and time and end day of week and time. 
        /// </summary>
        /// <param name="startdayOfWeek">Shift start day of week.</param>
        /// <param name="starttime">Shift start time.</param>
        /// <param name="enddayOfWeek">Shift end day of week.</param>
        /// <param name="endtime">Shift end time.</param>
        public ShiftData(DayOfWeek startdayOfWeek, TimeSpan starttime,
            DayOfWeek enddayOfWeek, TimeSpan endtime)
        {
            m_startDayOfWeek = startdayOfWeek;
            m_startTime = starttime;
            m_endDayOfWeek = enddayOfWeek;
            m_endTime = endtime;
        }

        /// <summary>
        /// Shift start day of week.
        /// </summary>
        [XmlElement]
        public DayOfWeek? StartDayOfWeek
        {
            get { return m_startDayOfWeek; }
            set { m_startDayOfWeek = value; }
        }

        /// <summary>
        /// Shift start time.
        /// </summary>
        [XmlIgnore]
        public TimeSpan? StartTime
        {
            get { return m_startTime; }
            set { m_startTime = value; }
        }

        /// <summary>
        /// It is surrogate property for serialization of TimeSpan property.
        /// Standard XmlSerializer does not serialize TimeSpan structure.
        /// See <![CDATA[http://www.devnewsgroups.net/group/microsoft.public.dotnet.framework/topic18670.aspx]]>
        /// for description.
        /// </summary>
        [XmlElement("StartTime")]
        public string StartTimeString
        {
            get
            {
                return (m_startTime.HasValue ? m_startTime.ToString() : String.Empty);
            }
            set
            {
                m_startTime = (String.IsNullOrEmpty(value) ? (TimeSpan?)null : TimeSpan.Parse(value));
            }
        }

        /// <summary>
        /// Shift end day of week.
        /// </summary>
        [XmlElement]
        public DayOfWeek? EndDayOfWeek
        {
            get { return m_endDayOfWeek; }
            set { m_endDayOfWeek = value; }
        }

        /// <summary>
        /// Shift end time.
        /// </summary>
        [XmlIgnore]
        public TimeSpan? EndTime
        {
            get { return m_endTime; }
            set { m_endTime = value; }
        }

        /// <summary>
        /// It is surrogate property for serialization of TimeSpan property.
        /// Standard XmlSerializer does not serialize TimeSpan structure.
        /// See <![CDATA[http://www.devnewsgroups.net/group/microsoft.public.dotnet.framework/topic18670.aspx]]>
        /// for description.
        /// </summary>
        [XmlElement("EndTime")]
        public string EndTimeString
        {
            get
            {
                return (m_endTime.HasValue ? m_endTime.ToString() : String.Empty);
            }
            set
            {
                m_endTime = (String.IsNullOrEmpty(value) ? (TimeSpan?)null : TimeSpan.Parse(value));
            }
        }

        

        /// <summary>
        /// Determines if current object has intersection with given object.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <returns>true, if object intersects; otherwise false.</returns>
        public bool HasIntersection(ShiftData obj)
        {
            var validator = ServiceLocator.Resolve<ISchedulingObjectValidator>();
            ErrorCollection errors;
            if (!validator.Validate(this, out errors))
            {
                throw new ApplicationException(errors.ToString());
            }

            if (!validator.Validate(obj, out errors))
            {
                throw new ArgumentException(errors.ToString(), "obj");
            }

            // current shift intersects with existing shift
            if (IsIntersection(obj))
                 return true;
            // existing shift intersects with current shift
            if (obj.IsIntersection(this))
                 return true;
            // no intersection
            return false;
        }

        /// <summary>
        /// Returns if shift is across weekend. But we allow shifts that ends at 00:00 of Monday.
        /// </summary>
        /// <returns>True if shift is across weekend; false otherwise.</returns>
        internal bool IsAcrossWeekend()
        {
            bool startOfWeek = EndDayOfWeek.Value == DayOfWeek.Monday && EndTime.Value == new TimeSpan(0, 0, 0);

            return !startOfWeek &&
                   SchedulingUtilities.IsLessPare(EndDayOfWeek.Value, EndTime.Value, StartDayOfWeek.Value, StartTime.Value);
        }

        /// <summary>
        /// Returns if current shift and existShift have intersection.
        /// </summary>
        /// <param name="existShift">Shift to check untersection.</param>
        /// <returns>True if current shift and existShift have intersection; false otherwise.</returns>
        private bool IsIntersection(ShiftData existShift)
        {
            bool startInsideShift = IsPointInLeftInterval(StartDayOfWeek.Value, StartTime.Value, existShift);
            bool endInsideShift = IsPointInRightInterval(EndDayOfWeek.Value, EndTime.Value, existShift);

            return startInsideShift || endInsideShift;
        }

        /// <summary>
        /// Returns if point (date and time) is inside of existing shift interval excluding right point.
        /// [.,.)
        /// </summary>
        /// <param name="day">Day to check</param>
        /// <param name="time">Time to check</param>
        /// <param name="existShift">Existing shift</param>
        /// <returns>True if point (date and time) is inside of existing shift interval; false otherwise.</returns>
        private static bool IsPointInLeftInterval(DayOfWeek day, TimeSpan time, ShiftData existShift)
        {
            if (SchedulingUtilities.IsLessPare(day, time, existShift.EndDayOfWeek.Value, existShift.EndTime.Value) &&
                SchedulingUtilities.IsLessOrEqualPare(existShift.StartDayOfWeek.Value, existShift.StartTime.Value, day, time))
                return true;
            return false;
        }

        /// <summary>
        /// Returns if point (date and time) is inside of existing shift interval excluding left point. 
        /// (.,.]
        /// </summary>
        /// <param name="day">Day to check</param>
        /// <param name="time">Time to check</param>
        /// <param name="existShift">Existing shift</param>
        /// <returns>True if point (date and time) is inside of existing shift interval; false otherwise.</returns>
        private static bool IsPointInRightInterval(DayOfWeek day, TimeSpan time, ShiftData existShift)
        {
            if (SchedulingUtilities.IsLessOrEqualPare(day, time, existShift.EndDayOfWeek.Value, existShift.EndTime.Value) &&
                SchedulingUtilities.IsLessPare(existShift.StartDayOfWeek.Value, existShift.StartTime.Value, day, time))
                return true;
            return false;
        }
    }
}
