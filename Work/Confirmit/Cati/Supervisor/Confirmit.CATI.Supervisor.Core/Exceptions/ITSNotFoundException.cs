using System;
using Confirmit.CATI.Supervisor.Core.Resources;

namespace Confirmit.CATI.Supervisor.Core.Exceptions
{
    /// <summary>
    /// ITS not found exception.
    /// </summary>
    public class ITSNotFoundException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ITSNotFoundException"/> class.
        /// </summary>
        public ITSNotFoundException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ITSNotFoundException"/> class.
        /// </summary>
        /// <param name="itsID">ITS id</param>
        public ITSNotFoundException(int itsID)
            : base(string.Format(Strings.ITSNotFoundMessage, itsID))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ITSNotFoundException"/> class.
        /// </summary>
        /// <param name="itsID">ITS id</param>
        /// <param name="innerException">Inner exception.</param>
        public ITSNotFoundException(int itsID, Exception innerException)
            : base(string.Format(Strings.ITSNotFoundMessage, itsID), innerException)
        { }
    }
}
