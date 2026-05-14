namespace Confirmit.CATI.Core.Services.PersonImport
{
    /// <summary>
    /// Class represents information about imported interviewer.
    /// </summary>
    public class AssignmentData
    {
        public string GroupName { get; set; }
        public string PersonName { get; set; }
        public string PersonPassword { get; set; }
        public string PersonDescription { get; set; }
        public string TaskChoice { get; set; }
        public string PersonLocation { get; set; }
        public string AutomaticSurvey { get; set; }
    }
}