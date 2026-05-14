using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{

    public abstract class SurveyInstance
    {
        public long SurveyId { get; private set; }
        public DialingMode DialingMode { get; private set; }

        public CallOutcomeSequence OutcomeSequence { get; private set; }

        protected SurveyInstance(long surveyId, DialingMode dialingMode, CallOutcomeSequence outcomeSequence)
        {
            SurveyId = surveyId;
            DialingMode = dialingMode;
            OutcomeSequence = outcomeSequence;
        }
    }
}