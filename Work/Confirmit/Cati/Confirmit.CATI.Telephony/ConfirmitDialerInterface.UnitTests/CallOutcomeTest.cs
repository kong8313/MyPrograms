using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConfirmitDialerInterface.UnitTests
{
    [TestClass]
    public class CallOutcomeTest
    {
        [TestMethod, Owner(@"FIRM\MaximG")]
        public void CheckOutcomeEnumerationRange()
        {
            const int callOutcomeEnumExpectedLength = 40;
            const int callOutcomeEnumExpectedMinValue = -1;
            const int callOutcomeEnumExpectedMaxValue = 1052;

            var callOutcomeEnumValues = Enum.GetValues(typeof(CallOutcome));

            var callOutcomeEnumActualLength = callOutcomeEnumValues.Length;
            var callOutcomeEnumActualMinValue = (int) callOutcomeEnumValues.Cast<CallOutcome>().Min();
            var callOutcomeEnumActualMaxValue = (int) callOutcomeEnumValues.Cast<CallOutcome>().Max();

            Assert.AreEqual(callOutcomeEnumExpectedLength, callOutcomeEnumActualLength,
                $"CallOutcome enum is expected to contain {callOutcomeEnumExpectedLength} outcomes");
            Assert.AreEqual(callOutcomeEnumExpectedMinValue, callOutcomeEnumActualMinValue,
                $"CallOutcome enum minimum value is expected to be {callOutcomeEnumExpectedMinValue}");
            Assert.AreEqual(callOutcomeEnumExpectedMaxValue, callOutcomeEnumActualMaxValue,
                $"CallOutcome enum maximum value is expected to be {callOutcomeEnumExpectedMaxValue}");

            for (var val = callOutcomeEnumExpectedMinValue; val <= callOutcomeEnumExpectedMaxValue; val++)
            {
                if (ValueMustBeIncludedInCallOutcomeEnum(val))
                {
                    Assert.IsTrue(Enum.IsDefined(typeof(CallOutcome), val),
                        string.Format("Outcome {0} must be contained in the CallOutcome enum", val));    
                }
                else
                {
                    // The value reserved for future use
                    Assert.IsFalse(Enum.IsDefined(typeof(CallOutcome), val),
                        string.Format("Outcome {0} is reserved and should not be contained in the CallOutcome enum", val));
                }
            }
        }

        private bool ValueMustBeIncludedInCallOutcomeEnum(int value)
        {
            const int callOutcomeEnumOriginalRangeEndValue = 30;
            int[] otherAvaialbleOutcomes = {1000, 1001, 1010, 1011, 1012, 1020, 1021, 1051, 1052};

            if (value == 19)
            {
                // This value from original (old) range is not being used
                return false;
            }

            if (value <= callOutcomeEnumOriginalRangeEndValue)
            {
                return true;
            }

            return otherAvaialbleOutcomes.Contains(value);
        }
    }
}