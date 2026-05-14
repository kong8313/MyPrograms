using System.Runtime.Serialization;

namespace Confirmit.CATI.Monitoring.Common.Contracts
{
    /// <summary>
    /// Class of response to file request.
    /// </summary>
    [DataContract]
    public class FileResponse
    {
        /// <summary>
        /// Gets or sets data of file.
        /// </summary>
        [DataMember]
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets name of file.
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets total number of bytes in file.
        /// </summary>
        [DataMember]
        public int Total { get; set; }
    }
}