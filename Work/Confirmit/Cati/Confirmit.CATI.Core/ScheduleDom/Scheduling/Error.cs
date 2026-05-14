using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
    /// <summary>
    /// Represents validation error. This class does nothing, it's just container
    /// for errors.
    /// </summary>
    public class Error
    {
        #region Fields

        private string m_message = String.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Error()
        {
        }

        /// <summary>
        /// Constructs new object with given description.
        /// </summary>
        /// <param name="description">Error description.</param>
        public Error(string description)
        {
            m_message = description;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Error description.
        /// </summary>
        public string Message
        {
            get { return m_message ?? String.Empty; }
            set { m_message = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a String that represents the current Error. 
        /// </summary>
        /// <returns>Returns a String that represents the current Error.</returns>
        public override string ToString()
        {
            return Message;
        }

        #endregion
    }

    /// <summary>
    /// Represents the collection of validation errors.
    /// </summary>
    public class ErrorCollection : List<Error>
    {
        /// <summary>
        /// Returns the array string representation of Error objects
        /// </summary>
        /// <returns>Array of string.</returns>
        public string[] ToStringArray()
        {
            var result = new List<string>();

            foreach (Error error in this)
            {
                result.Add(error.ToString());
            }

            return result.ToArray();
        }

        /// <summary>
        /// Returns the string representation of all errors in the collection
        /// </summary>
        /// <returns>Array of string.</returns>
        public override string ToString()
        {
            return string.Join("\r\n", ToStringArray());
        }
    }
}
