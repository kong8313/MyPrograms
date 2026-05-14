using System;

namespace Confirmit.CATI.Supervisor.Core.Exceptions
{
    /// <summary>
    /// Represents exception which occurs when you are trying to delete logged in person.
    /// </summary>
    public class PersonLoggedInException : ApplicationException
    {
        #region Constructors

        /// <summary>
        /// Initializes new instance of PersonLoggedInException class.
        /// </summary>
        public PersonLoggedInException() : base() { }

        /// <summary>
        /// Initializes new instance of PersonLoggedInException class and fills it with given data.
        /// </summary>
        /// <param name="personId">Person identifier.</param>
        public PersonLoggedInException(int personId)
        {
            PersonId = personId;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets person identifier.
        /// </summary>
        public int PersonId
        {
            get;
            set;
        }

        #endregion
    }
}
