using System;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Contains information about a dialer error
    /// </summary>
    [Serializable] // Note! It must be serializable as it's being used in FaultException
    public class DialerParameterError
    {
        /// <summary>
        /// Indicates that parameter is unknown
        /// </summary>
        public const string UnknownParameter = "unknown parameter";

        /// <summary>
        /// Indicates that parameter is not specified
        /// </summary>
        public const string NotSpecified = "parameter is not specified";

        /// <summary>
        /// Indicates that parameter has invalid value
        /// </summary>
        public const string InvalidValue = "Invalid value";

        /// <summary>
        /// Constructor w/o full error description
        /// </summary>
        /// <param name="id">the parameter identifier</param>
        /// <param name="name">the parameter name</param>
        /// <param name="errorDescription">Short error description</param>
        public DialerParameterError(string id, string name, string errorDescription)
        {
            Id = id;
            Name = name;
            ErrorDescription = errorDescription;
            FullDescription = errorDescription;
        }

        /// <summary>
        /// Constructor with full error description
        /// </summary>
        /// <param name="id">the parameter identifier</param>
        /// <param name="name">the parameter name</param>
        /// <param name="errorDescription">Short error description</param>
        /// <param name="errorFullDescription">Full error description</param>
        public DialerParameterError(string id, string name, string errorDescription, string errorFullDescription)
            : this(id, name, errorDescription)
        {
            FullDescription = errorFullDescription;
        }


        /// <summary>
        /// The parameter identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The parameter name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Short error description
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Full error description
        /// </summary>
        public string FullDescription { get; set; }
    }
}
