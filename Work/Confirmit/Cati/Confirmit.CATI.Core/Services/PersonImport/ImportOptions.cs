namespace Confirmit.CATI.Core.Services.PersonImport
{
    /// <summary>
    /// Class represents options for interviewer importing.
    /// </summary>
    public class ImportOptions
    {
        public bool ImportFirstRow { get; set; }
        public bool OverwriteExistentRelations { get; set; }
        public bool OverwriteExistentData { get; set; }
    }
}