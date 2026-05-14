using System;

namespace Confirmit.CATI.Supervisor.Core.Exceptions
{
    /// <summary>
    /// Represents exception which occurs when you are trying to delete not empty group.
    /// </summary>
    public class GroupNotEmptyException : ApplicationException
    {
        #region Constructors

        /// <summary>
        /// Initializes new instance of GroupNotEmptyException.
        /// </summary>
        public GroupNotEmptyException() { }

        /// <summary>
        /// Initializes new instance of GroupNotEmptyException and fills it
        /// with given data.
        /// </summary>
        /// <param name="groupId">Group identifier.</param>
        public GroupNotEmptyException(int groupId)
        {
            GroupId = groupId;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Group identifier.
        /// </summary>
        public int GroupId
        {
            get;
            set;
        }

        #endregion
    }
}
