using System;
using Confirmit.Logging;
using Confirmit.CATI.Core.Logger;

namespace Confirmit.CATI.Core.Logger.Fakes
{
    public class StubILogWriter : ILogWriter 
    {
        private ILogWriter _inner;

        public StubILogWriter()
        {
            _inner = null;
        }

        public ILogWriter Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void WriteLogLevelStringArrayOfCustomFieldStringDelegate(LogLevel logLevel, string message, CustomField[] eventFields, string loggerName);
        public WriteLogLevelStringArrayOfCustomFieldStringDelegate WriteLogLevelStringArrayOfCustomFieldString;

        void ILogWriter.Write(LogLevel logLevel, string message, CustomField[] eventFields, string loggerName)
        {

            if (WriteLogLevelStringArrayOfCustomFieldString != null)
            {
                WriteLogLevelStringArrayOfCustomFieldString(logLevel, message, eventFields, loggerName);
            } else if (_inner != null)
            {
                ((ILogWriter)_inner).Write(logLevel, message, eventFields, loggerName);
            }
        }

    }
}