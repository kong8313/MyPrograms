namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    /// <summary>
    /// Describes a survey.
    /// </summary>
    public class Survey
    {
        /// <summary>
        /// The survey unique identifier
        /// Confirmit ID like pNNNNNNN is used.
        /// </summary>
        public string id;

        /// <summary>
        /// The survey name.
        /// </summary>
        public string name;

        public bool IsRespondentsDynamicCreationAllowed;
    }
}