using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public class SurveyInstancePreview : SurveyInstance
    {
        public SurveyInstancePreview(long surveyId, CallOutcomeSequence outcomeSequence)
            : base(surveyId, DialingMode.Preview, outcomeSequence)
        {
        }
    }
}