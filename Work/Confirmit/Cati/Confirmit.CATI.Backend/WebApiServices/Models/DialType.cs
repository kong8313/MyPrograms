namespace Confirmit.CATI.Backend.WebApiServices.Models
{
    /// <summary>
    /// Enum with possible dial types
    /// </summary>
    public enum DialType
    {
        /// <summary>
        /// Automatic dial type - indicates that interview or interviewer can use automatic dialing modes (Predictive or Automatic)
        /// </summary>
        Automatic = 0,

        /// <summary>
        /// Manual dial type - indicates that interview or interviewer can only use Manual or Preview dialing modes
        /// </summary>
        Manual = 1,

        /// <summary>
        /// Manual Dialling with agent assistance
        /// </summary>
        Assisted = 2
    }
}