namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    /// <summary>
    /// Type of the sample record based on the result of some operation with the record. 
    /// </summary>
    public enum SampleRecordOperationType
    {
        /// <summary>
        /// Record is valid for the operation.
        /// </summary>
        Correct,

        /// <summary>
        /// Record is invalid for the operation.
        /// </summary>
        Incorrect,

        /// <summary>
        /// Record is considered empty.
        /// </summary>
        Empty
    }
}