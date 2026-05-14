using System;

namespace Confirmit.CATI.Common.Logging
{
    /// <summary>DTO for describing log file item.</summary>
    [Serializable]
    public class LogFileInfo
    {
        /// <summary>Gets the full name of the file with extension.</summary>
        public string Name { get; }

        /// <summary>Gets the size, in bytes, of the current file.</summary>
        public long Length { get; }

        /// <summary>Gets the creation time, in coordinated universal time (UTC), of the file.</summary>
        public DateTime CreationTimeUtc { get; }

        /// <summary>Gets the time, in coordinated universal time (UTC), when the current file was last written to.</summary>
        public DateTime LastWriteTimeUtc { get; }

        public LogFileInfo(string name, long length, DateTime creationTimeUtc, DateTime lastWriteTimeUtc)
        {
            Name = name;
            Length = length;
            CreationTimeUtc = creationTimeUtc;
            LastWriteTimeUtc = lastWriteTimeUtc;
        }

        /// <summary>
        /// Empty constructor for deserializer
        /// </summary>
        protected LogFileInfo()
        {
        }

        public override string ToString()
        {
            return $"{nameof(Name)}={Name}, {nameof(Length)}={Length}, {nameof(CreationTimeUtc)}={CreationTimeUtc}, {nameof(LastWriteTimeUtc)}={LastWriteTimeUtc}";
        }
    }
}