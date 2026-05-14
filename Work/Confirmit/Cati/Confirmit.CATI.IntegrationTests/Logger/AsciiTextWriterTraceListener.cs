using System.Diagnostics;

namespace Confirmit.CATI.IntegrationTests.Logger
{
    public class AsciiTextWriterTraceListener : TraceListener
    {
        private readonly AsciiTextWriter textWriter;

        public AsciiTextWriterTraceListener(string path)
        {
            this.textWriter = new AsciiTextWriter(path);
        }

        public override void Write(string message)
        {
            this.textWriter.Write(message);
        }

        public override void WriteLine(string message)
        {
            this.textWriter.WriteLine(message);
        }

        public override void Close()
        {
            this.Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.textWriter.Close();
            }
        }
    }
}
