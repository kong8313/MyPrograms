namespace Confirmit.CATI.REST.SDK.Model
{
    /// <summary>
    /// Enum representing types of the telephone numbers in the blacklist
    /// </summary>
    public enum BlacklistPatternType : byte
    {
        /// <summary>
        /// This type means exact match of the telephone number
        /// </summary>
        Equal = 0,

        /// <summary>
        /// This type means that only the beginning of the number is stored
        /// </summary>
        StartWith = 1
    }
}
