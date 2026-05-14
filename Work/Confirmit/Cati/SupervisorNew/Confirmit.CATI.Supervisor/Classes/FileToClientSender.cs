using System;
using System.IO;
using System.Xml.Serialization;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Supervisor.Classes
{
    public class FileToClientSender
    {
        private readonly BaseForm _page;
        private readonly IFileToBrowserSender _fileToBrowserSender;
        private readonly IPgpEncryptionService _pgpEncryptionService;

        public FileToClientSender(BaseForm page, IFileToBrowserSender fileToBrowserSender, IPgpEncryptionService pgpEncryptionService)
        {
            _page = page;
            _fileToBrowserSender = fileToBrowserSender;
            _pgpEncryptionService = pgpEncryptionService;
        }

        /// <summary>
        /// Sends specified file to client
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="fileName">Name of file that user will recieve</param>
        /// <param name="useEncryption">Should PGP encryption be used</param>
        public void SendFileContent(string filePath, string fileName, bool useEncryption = false)
        {

            try
            {
                var fileBytes = _pgpEncryptionService.EncryptIfNeeded(filePath, ref fileName, useEncryption);

                _fileToBrowserSender.Send(
                    _page,
                   fileBytes.ToArray(),
                   fileName,
                   false);
            }
            finally
            {
                try
                {
                    if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Sends specified file to client and adds timestamp to file name.
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="fileName">Name of file that user will receive</param>
        /// /// <param name="useEncryption">Should PGP encryption be used</param>
        public void SendWithTimeStamp(string filePath, string fileName, bool useEncryption = false)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);

            SendFileContent(filePath, String.Format("{0}-{1}{2}", name, DateTime.UtcNow.ToString("yyMMddHHss"), extension), useEncryption);
        }

        /// <summary>
        /// Serialize object and send it to client as a file.
        /// </summary>
        /// <typeparam name="T">Type of object to serialize.</typeparam>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="fileName">Name of the file.</param>
        public void Send<T>(T obj, string fileName)
        {
            byte[] buffer;

            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, obj);

                buffer = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(buffer, 0, buffer.Length);
            }

            _fileToBrowserSender.Send(_page, buffer, fileName, false);
        }

        /// <summary>
        /// Send buffer to client as a file
        /// </summary>
        /// <param name="buffer">Buffer contained file body</param>
        /// <param name="fileName">Name of file that user will receive</param>
        public void SendBuffer(byte[] buffer, string fileName)
        {
            _fileToBrowserSender.Send(_page, buffer, fileName, false);
        }
    }
}