using System;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Represents result of getting audio file from dialer
    /// </summary>
    public class AudioFile
    {
        /// <summary>
        /// The name of the file in the following format: "filename.extension". For example, "file1.wav"
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The UTC time when the recording file has been created.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// The file content
        /// </summary>
        public byte[] Content { get; set; }
    }
}