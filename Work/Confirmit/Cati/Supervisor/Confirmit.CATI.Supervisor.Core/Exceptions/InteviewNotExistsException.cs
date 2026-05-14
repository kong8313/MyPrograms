using System;
using Confirmit.CATI.Supervisor.Core.Resources;

namespace Confirmit.CATI.Supervisor.Core.Exceptions
{
    /// <summary>
    /// Inteview not exists exception.
    /// </summary>
    public class InteviewNotExistsException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteviewNotExistsException"/> class.
        /// </summary>
        public InteviewNotExistsException()
            : base(Strings.InterviewWithTheSpecifiedIDNotExsists)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteviewNotExistsException"/> class.
        /// </summary>
        /// <param name="interviewID">The interview ID.</param>
        public InteviewNotExistsException(int interviewID)
            : base(String.Format(Strings.InterviewWithIDNotExists, interviewID))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteviewNotExistsException"/> class.
        /// </summary>
        /// <param name="interviewID">The interview ID.</param>
        /// <param name="innerException">Inner exception.</param>
        public InteviewNotExistsException(int interviewID, Exception innerException)
            : base(String.Format(Strings.InterviewWithIDNotExists, interviewID), innerException)
        { }
    }
}
