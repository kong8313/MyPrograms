using System;
using System.IO;
using System.Text;

namespace Confirmit.CATI.IntegrationTests.Logger
{
    public class AsciiTextWriter
    {
        private static readonly object FileLock = new object();

        private static FileStream fileStream;

        public AsciiTextWriter(string path)
        {
            if (fileStream == null)
            {
                lock (FileLock)
                {
                    if (fileStream == null)
                    {
                        fileStream = new FileStream(
                            path,
                            FileMode.Append,
                            FileAccess.Write,
                            FileShare.Read);
                    }
                }
            }
        }

        public void Close()
        {
            lock (FileLock)
            {
                if (fileStream != null)
                {
                    fileStream.Flush();
                    fileStream.Close();
                }
            }
        }

        public void Write(string message)
        {
            var encoding = new ASCIIEncoding();
            var bytes = encoding.GetBytes(message);

            lock (FileLock)
            {
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        public void WriteLine(string message)
        {
            this.Write(message + Environment.NewLine);
        }
    }
}
