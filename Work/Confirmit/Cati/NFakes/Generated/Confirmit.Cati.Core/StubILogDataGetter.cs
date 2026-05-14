using System;
using Confirmit.CATI.Core.Logger;
using Confirmit.Logging;

namespace Confirmit.CATI.Core.Logger.Fakes
{
    public class StubILogDataGetter : ILogDataGetter 
    {
        private ILogDataGetter _inner;

        public StubILogDataGetter()
        {
            _inner = null;
        }

        public ILogDataGetter Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetMessageFooterForDatabaseLoggingLogDataDelegate(LogData logData);
        public GetMessageFooterForDatabaseLoggingLogDataDelegate GetMessageFooterForDatabaseLoggingLogData;

        string ILogDataGetter.GetMessageFooterForDatabaseLogging(LogData logData)
        {


            if (GetMessageFooterForDatabaseLoggingLogData != null)
            {
                return GetMessageFooterForDatabaseLoggingLogData(logData);
            } else if (_inner != null)
            {
                return ((ILogDataGetter)_inner).GetMessageFooterForDatabaseLogging(logData);
            }

            return default(string);
        }

        public delegate CustomField[] MakeKibanaCustomFieldsLogDataDelegate(LogData logData);
        public MakeKibanaCustomFieldsLogDataDelegate MakeKibanaCustomFieldsLogData;

        CustomField[] ILogDataGetter.MakeKibanaCustomFields(LogData logData)
        {


            if (MakeKibanaCustomFieldsLogData != null)
            {
                return MakeKibanaCustomFieldsLogData(logData);
            } else if (_inner != null)
            {
                return ((ILogDataGetter)_inner).MakeKibanaCustomFields(logData);
            }

            return default(CustomField[]);
        }

    }
}