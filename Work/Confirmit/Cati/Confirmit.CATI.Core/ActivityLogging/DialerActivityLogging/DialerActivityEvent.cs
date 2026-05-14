using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Telephony;
using Confirmit.Logging;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.ActivityLogging.DialerActivityLogging
{
    public class DialerActivityEvent : DialerActivityEventBase
    {


        public class SurveyRef
        {
            public string ProjectId { get; set; }
            public int SurveySid { get; set; }
        }

        public DialerActivityEvent(int dialerId, [CallerMemberName] string eventName = "") : base(dialerId, eventName)
        {
        }

        public DialerActivityEvent Survey(string projectId, int surveySid)
        {
            var fields = new[] {
                new CustomField("ProjectId", projectId),
                new CustomField("SurveySid", surveySid),
            };

            EventFields = EventFields.ConcatWithReplace(fields);

            return this;
        }

        public DialerActivityEvent Survey(SurveyRef surveyRef) => Survey(surveyRef.ProjectId, surveyRef.SurveySid);

        public DialerActivityEvent Interviewer(string interviewerSid)
        {
            var parsed = int.TryParse(interviewerSid, out var interviewerId);
            Interviewer(parsed ? interviewerId : -1);

            return this;
        }

        public DialerActivityEvent Interviewer(int interviewerSid)
        {
            var fields = new[] {
                new CustomField("InterviewerSid", interviewerSid),
            };

            EventFields = EventFields.ConcatWithReplace(fields);

            return this;
        }

        public DialerActivityEvent Interview(int interviewId)
        {
            var fields = new[] {
                new CustomField("InterviewId", interviewId),
            };

            EventFields = EventFields.ConcatWithReplace(fields);

            return this;
        }

        public DialerActivityEvent Error(int errorCode)
        {
            var errorName = Enum.IsDefined(typeof(DialerErrorCode), errorCode) ? ((DialerErrorCode)errorCode).ToString() : null;

            var fields = new[] {
                new CustomField("ErrorCode", errorCode),
                new CustomField("ErrorName", errorName ?? "-"),
            };

            EventFields = EventFields.ConcatWithReplace(fields);

            return this;
        }

        public DialerActivityEvent Error(DialerErrorCode errorCode)
        {
            Error((int)errorCode);

            return this;
        }

        public DialerActivityEvent Exception(Exception ex)
        {
            var fields = new[] {
                new CustomField("Exception", ex.ToString()),
                new CustomField("ExceptionType", ex.GetType().ToString()),
            };

            EventFields = EventFields.ConcatWithReplace(fields);

            return this;
        }

        public DialerActivityEvent Detail(string name, string value)
        {
            Details[name] = value;

            return this;
        }

        public DialerActivityEvent Detail(string name, object value) => Detail(name, GetValueForDetails(value));

        public DialerActivityEvent Parameters(string value) => Detail("Parameters", value);

        public DialerActivityEvent Result(object result)
        {
            string value;
            if (ResultFormatterFunc != null)
                value = ResultFormatterFunc(result);
            else if (result is string s)
                value = s;
            else
                value = GetValueForDetails(result);

            Detail("Result", value);

            return this;
        }

        private string GetValueForDetails(object value) => value.ToJson();

        protected Func<object, string> ResultFormatterFunc { get; private set; }

        public DialerActivityEvent ResultFormatter(Func<object, string> resultFormatter)
        {
            ResultFormatterFunc = resultFormatter;

            return this;
        }

        public void LogInfo(object result)
        {
            Result(result);
            WriteLog(LogLevel.Info);
        }

        public void LogError(int errorCode, Exception ex = null)
        {
            Error(errorCode);
            if (ex != null)
                Exception(ex);

            if (DialerErrorSeverityProvider.IsWarning((DialerErrorCode)errorCode))
                WriteLog(LogLevel.Warn);
            else
                WriteLog(LogLevel.Error);
        }

        public void LogError(DialerErrorCode errorCode, Exception ex = null)
        {
            LogError((int)errorCode, ex);
        }

        public void LogError(Exception ex)
        {
            Exception(ex);

            WriteLog(LogLevel.Error);
        }
    }
}