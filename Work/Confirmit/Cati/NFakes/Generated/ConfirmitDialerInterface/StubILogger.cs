using System;
using System.Diagnostics;
using ConfirmitDialerInterface;

namespace ConfirmitDialerInterface.Fakes
{
    public class StubILogger : ILogger 
    {
        private ILogger _inner;

        public StubILogger()
        {
            _inner = null;
        }

        public ILogger Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void WriteLineTraceEventTypeStringStringDelegate(TraceEventType logLevel, string comment, string message);
        public WriteLineTraceEventTypeStringStringDelegate WriteLineTraceEventTypeStringString;

        void ILogger.WriteLine(TraceEventType logLevel, string comment, string message)
        {

            if (WriteLineTraceEventTypeStringString != null)
            {
                WriteLineTraceEventTypeStringString(logLevel, comment, message);
            } else if (_inner != null)
            {
                ((ILogger)_inner).WriteLine(logLevel, comment, message);
            }
        }

        public delegate void ErrorStringStringArrayOfObjectDelegate(string sourceCodeLocation, string message, Object[] args);
        public ErrorStringStringArrayOfObjectDelegate ErrorStringStringArrayOfObject;

        void ILogger.Error(string sourceCodeLocation, string message, Object[] args)
        {

            if (ErrorStringStringArrayOfObject != null)
            {
                ErrorStringStringArrayOfObject(sourceCodeLocation, message, args);
            } else if (_inner != null)
            {
                ((ILogger)_inner).Error(sourceCodeLocation, message, args);
            }
        }

        public delegate void ErrorStringFuncOfStringDelegate(string sourceCodeLocation, Func<string> messageFunc);
        public ErrorStringFuncOfStringDelegate ErrorStringFuncOfString;

        void ILogger.Error(string sourceCodeLocation, Func<string> messageFunc)
        {

            if (ErrorStringFuncOfString != null)
            {
                ErrorStringFuncOfString(sourceCodeLocation, messageFunc);
            } else if (_inner != null)
            {
                ((ILogger)_inner).Error(sourceCodeLocation, messageFunc);
            }
        }

        public delegate void WarningStringStringArrayOfObjectDelegate(string sourceCodeLocation, string message, Object[] args);
        public WarningStringStringArrayOfObjectDelegate WarningStringStringArrayOfObject;

        void ILogger.Warning(string sourceCodeLocation, string message, Object[] args)
        {

            if (WarningStringStringArrayOfObject != null)
            {
                WarningStringStringArrayOfObject(sourceCodeLocation, message, args);
            } else if (_inner != null)
            {
                ((ILogger)_inner).Warning(sourceCodeLocation, message, args);
            }
        }

        public delegate void WarningStringFuncOfStringDelegate(string sourceCodeLocation, Func<string> messageFunc);
        public WarningStringFuncOfStringDelegate WarningStringFuncOfString;

        void ILogger.Warning(string sourceCodeLocation, Func<string> messageFunc)
        {

            if (WarningStringFuncOfString != null)
            {
                WarningStringFuncOfString(sourceCodeLocation, messageFunc);
            } else if (_inner != null)
            {
                ((ILogger)_inner).Warning(sourceCodeLocation, messageFunc);
            }
        }

        public delegate void InfoStringStringArrayOfObjectDelegate(string sourceCodeLocation, string message, Object[] args);
        public InfoStringStringArrayOfObjectDelegate InfoStringStringArrayOfObject;

        void ILogger.Info(string sourceCodeLocation, string message, Object[] args)
        {

            if (InfoStringStringArrayOfObject != null)
            {
                InfoStringStringArrayOfObject(sourceCodeLocation, message, args);
            } else if (_inner != null)
            {
                ((ILogger)_inner).Info(sourceCodeLocation, message, args);
            }
        }

        public delegate void InfoStringFuncOfStringDelegate(string sourceCodeLocation, Func<string> messageFunc);
        public InfoStringFuncOfStringDelegate InfoStringFuncOfString;

        void ILogger.Info(string sourceCodeLocation, Func<string> messageFunc)
        {

            if (InfoStringFuncOfString != null)
            {
                InfoStringFuncOfString(sourceCodeLocation, messageFunc);
            } else if (_inner != null)
            {
                ((ILogger)_inner).Info(sourceCodeLocation, messageFunc);
            }
        }

        public delegate void VerboseStringStringArrayOfObjectDelegate(string sourceCodeLocation, string message, Object[] args);
        public VerboseStringStringArrayOfObjectDelegate VerboseStringStringArrayOfObject;

        void ILogger.Verbose(string sourceCodeLocation, string message, Object[] args)
        {

            if (VerboseStringStringArrayOfObject != null)
            {
                VerboseStringStringArrayOfObject(sourceCodeLocation, message, args);
            } else if (_inner != null)
            {
                ((ILogger)_inner).Verbose(sourceCodeLocation, message, args);
            }
        }

        public delegate void VerboseStringFuncOfStringDelegate(string sourceCodeLocation, Func<string> messageFunc);
        public VerboseStringFuncOfStringDelegate VerboseStringFuncOfString;

        void ILogger.Verbose(string sourceCodeLocation, Func<string> messageFunc)
        {

            if (VerboseStringFuncOfString != null)
            {
                VerboseStringFuncOfString(sourceCodeLocation, messageFunc);
            } else if (_inner != null)
            {
                ((ILogger)_inner).Verbose(sourceCodeLocation, messageFunc);
            }
        }

        public delegate ILogger NewLoggerStringDelegate(string source);
        public NewLoggerStringDelegate NewLoggerString;

        ILogger ILogger.NewLogger(string source)
        {


            if (NewLoggerString != null)
            {
                return NewLoggerString(source);
            } else if (_inner != null)
            {
                return ((ILogger)_inner).NewLogger(source);
            }

            return default(ILogger);
        }

    }
}