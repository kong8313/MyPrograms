using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
    /// <summary>
    /// Contains information about scheduling script.
    /// </summary>
    [Serializable]
    public class SchedulingScript
    {
        #region Fields
        private Schedule m_schedule;
        private string m_name;
        private string m_text;
        #endregion

        #region Properties
        /// <summary>
        /// Schedule object associated with scheduling script.
        /// </summary>
        [XmlElement]
        public Schedule Schedule
        {
            get
            {
                return m_schedule;
            }
            set
            {
                m_schedule = value;
            }
        }

        /// <summary>
        /// Scheduling script name.
        /// </summary>
        [XmlElement]
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// Scheduling script text.
        /// </summary>
        [XmlElement]
        public string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                m_text = value;
            }
        }
        #endregion

        #region Methodes
        /// <summary>
        /// Constructor.
        /// </summary>
        public SchedulingScript(string name, Schedule schedule)
        {
            Name = name;
            Schedule = schedule;
        }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public SchedulingScript()
        {
        }
        #endregion
    }
}
