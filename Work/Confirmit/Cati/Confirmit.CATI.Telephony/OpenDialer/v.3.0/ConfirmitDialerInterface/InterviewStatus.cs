namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Interview status is the state that interview acquires when it is completed.
    /// Interview status is a CATI system concept, but it can be used by dialer to collect some statistics.
    /// Interview status is defined by a unique code and (possibly non-unique) name.
    /// <seealso cref="UpdateInterviewStatus"/>
    /// </summary>
    public class InterviewStatus
    {
        /// <summary>
        /// Interview transient state unique code
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Interview transient state name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The interview status string representation
        /// </summary>
        /// <returns>The interview status string representation in the form of \"[Code, Name]\""</returns>
        public override string ToString()
        {
            return string.Format("[{0}, {1}]", Code, Name);
        }
    }
}
