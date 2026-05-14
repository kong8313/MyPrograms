using System;

namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    /// <summary>
    /// Represents time zone. This class is wrapper for CATI Web Service time zone.
    /// </summary>
    public class Timezone
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Timezone() { }

        /// <summary>
        /// Initializes a new instance of the Timezone and fills it with the 
        /// given values.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="name">Name.</param>
        public Timezone(
            string name,
            int bias,
            string standardName,
            DateTime? standardDate,
            int standardDayOfWeek,
            int standardBias,
            string daylightName,
            DateTime? daylightDate,
            int daylightDayOfWeek,
            int daylightBias,
            int id, 
            DaylightType daylightType)
        {
            Name = name;
            Bias = bias;

            StandardName = standardName;
            StandardDate = standardDate;
            StandardDayOfWeek = standardDayOfWeek;
            StandardBias = standardBias;

            DaylightName = daylightName;
            DaylightDate = daylightDate;
            DaylightDayOfWeek = daylightDayOfWeek;
            DaylightBias = daylightBias;
            DaylightType = daylightType;

            Id = id;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Timezone name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current bias for local time translation on this computer, in minutes. 
        /// The bias is the difference, in minutes, between Coordinated Universal Time (UTC) and local time.
        /// </summary>
        public int Bias { get; set; }

        /// <summary>
        /// Timezone name for standard time period.
        /// </summary>
        public string StandardName { get; set; }

        /// <summary>
        /// A date and local time when the transition from daylight saving time to standard time occurs.
        /// </summary>
        public DateTime? StandardDate { get; set; }

        /// <summary>
        /// The day of week when the transition from daylight saving time to standard time occurs.
        /// </summary>
        public int StandardDayOfWeek { get; set; }

        /// <summary>
        /// The bias value to be used during local time translations that occur during standard time. 
        /// In most time zones, the value of this member is zero.
        /// </summary>
        public int StandardBias { get; set; }

        /// <summary>
        /// Timezone name for daylight time period.
        /// </summary>
        public string DaylightName { get; set; }

        /// <summary>
        /// A date and local time when the transition from standard time to daylight saving time occurs.
        /// </summary>
        public DateTime? DaylightDate { get; set; }

        /// <summary>
        /// The day of week when the transition from standard time to daylight saving time occurs.
        /// </summary>
        public int DaylightDayOfWeek { get; set; }

        /// <summary>
        /// The bias value to be used during local time translations that occur during daylight saving time.
        /// </summary>
        public int DaylightBias { get; set; }

        /// <summary>
        /// Timezone identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Timezone daylight type.
        /// </summary>
        public DaylightType DaylightType { get; set; }

        #endregion

        #region ClassData
        // *  according to BvdbsTimezoneFields (see Projects\Units\bv7\BvDbs\include\BvdbsFields.h)

        #endregion
    }
}