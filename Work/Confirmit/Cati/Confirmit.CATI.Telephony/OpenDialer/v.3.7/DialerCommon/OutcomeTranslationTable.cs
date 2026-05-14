using System.Collections.Generic;
using System.Diagnostics;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony
{
    public abstract class OutcomeTranslationTable
    {
        protected Dictionary<int, CallOutcome> translationTable;

        public OutcomeTranslationTable()
        {
            translationTable = new Dictionary<int, CallOutcome>();
            FillOutcomeTranslationTable();
        }

        public bool TranslateOutcome(int internalOutcome, out CallOutcome externalOutcome)
        {
            if (translationTable.TryGetValue(internalOutcome, out externalOutcome))
            {
                return true;
            }
            else
            {
                Trace.TraceWarning(
                    "OutcomeTranslationTable.TranslateOutcome: Unknown outcome {0} has been translated to 'TelephonyFailure' outcome.",
                    internalOutcome);
                externalOutcome = CallOutcome.TelephonyFailure;
                return false;
            }
        }

        public abstract void FillOutcomeTranslationTable();
    }
}
