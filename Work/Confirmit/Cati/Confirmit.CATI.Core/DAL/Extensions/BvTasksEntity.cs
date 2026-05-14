using System.Xml.Serialization;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Tasks;
using ConfirmitDialerInterface;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.DAL.Generated.Entity.Table
{
    public partial class BvTasksEntity
    {
        public DialType DialType
        {
            get { return (DialType)DialTypeId; }
            set { DialTypeId = (byte) value; }
        }

        [XmlIgnore]
        public DialingMode DialingMode
        {
            get => (DialingMode) DiallingMode;
            set => DiallingMode = (int)value;
        }

        private TaskContext _context;

        public TaskContext Context =>
            _context ??
            (_context = JsonContext != null
                ? JsonConvert.DeserializeObject<TaskContext>(JsonContext)
                : new TaskContext());

        partial void OnBeforeGetJsonContext()
        {
            if (_context != null)
            {
                m_jsoncontext = JsonConvert.SerializeObject(_context);
            }
        }

        partial void OnBeforeSetJsonContext()
        {
            _context = null;
        }

        public override string ToString()
        {
            return $"{nameof(PersonSID)}: {PersonSID}, {nameof(SurveySID)}: {SurveySID}, {nameof(InterviewID)}: {InterviewID}, {nameof(InterviewState)}: {InterviewState}, {nameof(State)}: {State}, {nameof(TimeCallDelivered)}: {TimeCallDelivered}, {nameof(TimeStateChanged)}: {TimeStateChanged}, {nameof(TzID)}: {TzID}, {nameof(DiallingMode)}: {DiallingMode}, {nameof(CallOutcome)}: {CallOutcome}, {nameof(StatusLogout)}: {StatusLogout}, {nameof(LoggedInToDialerState)}: {LoggedInToDialerState}, {nameof(IsLoginRCToDialer)}: {IsLoginRCToDialer}, {nameof(CallID)}: {CallID}, {nameof(LastKeepAliveTime)}: {LastKeepAliveTime}, {nameof(ProblemId)}: {ProblemId}, {nameof(LockTime)}: {LockTime}, {nameof(StationId)}: {StationId}, {nameof(StartTime)}: {StartTime}, {nameof(AuthenticationKey)}: {AuthenticationKey}, {nameof(StartSessionTime)}: {StartSessionTime}, {nameof(EncryptionKey)}: {EncryptionKey}, {nameof(EncryptionIV)}: {EncryptionIV}, {nameof(DialerId)}: {DialerId}, {nameof(StationExtensionNumber)}: {StationExtensionNumber}, {nameof(IsDialerAgentLocal)}: {IsDialerAgentLocal}, {nameof(CallCenterID)}: {CallCenterID}, {nameof(SessionId)}: {SessionId}, {nameof(NewSurveySID)}: {NewSurveySID}, {nameof(DialTypeId)}: {DialTypeId}, {nameof(OpenEndReviewStartTime)}: {OpenEndReviewStartTime}, {nameof(CurrentUtcTime)}: {CurrentUtcTime}, {nameof(CallType)}: {CallType}, {nameof(LinkedCallId)}: {LinkedCallId}, {nameof(LinkedChain)}: {LinkedChain}, {nameof(LinkedInterviewSessionId)}: {LinkedInterviewSessionId}, {nameof(SelectedSurveyId)}: {SelectedSurveyId}, {nameof(JsonContext)}: {JsonContext}, {nameof(CallConnectionState)}: {CallConnectionState}, {nameof(BreakTypeId)}: {BreakTypeId}";
        }
    }
}
