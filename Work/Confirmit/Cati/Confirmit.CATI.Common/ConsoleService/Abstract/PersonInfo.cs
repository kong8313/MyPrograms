using System;

namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    public class PersonInfo
    {
        /// <summary>
        /// The interviewer identifier.
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// Gets/sets flag indicated is person already logged in or not.
        /// True if the person was already logged in, false otherwise
        /// </summary>
        public bool AlreadyLoggedIn { get; set; }

        /// <summary>
        /// Gets/sets the logged in person mode (actually Confirmit.CATI.Common.PersonMode)
        /// </summary>
        public int PersonMode { get; set; }

        /// <summary>
        /// Gets/sets Confirmit survey id (e.g. 'p0000000') which is automatic
        /// login survey for current user. If this id is not empty, we use it to automatically start
        /// interviewing in "Survey selection" mode.
        /// </summary>
        public string AutoSurveyId { get; set; }

        /// <summary>
        /// Gets/sets permission of task choice
        /// </summary>
        public int? TaskChoicePermissions { get; set; }

        /// <summary>
        /// Gets or sets the authentication key associated with current interviewing session and used in console state service.
        /// </summary>
        public Guid AuthenticationKey { get; set; }

        public byte[] EncryptionKey { get; set; }

        public byte[] EncryptionIV { get; set; }

        public DialType DialType { get; set; }
    }
}
