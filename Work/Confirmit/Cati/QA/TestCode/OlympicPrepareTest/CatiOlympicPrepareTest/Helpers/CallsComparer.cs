using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using Confirmit.CATI.REST.SDK.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace CatiOlympicPrepareTest.Helpers
{
    public abstract class CallsComparer : IComparer
    {
        internal int Index;
        internal int Size;
        internal bool NotEqual;
        internal string FieldName;
        internal TestContext CtxTestContext;

        public abstract int Compare(object x, object interviewIts);

        public void WriteLine(string output)
        {
            CtxTestContext.WriteLine(output);
            //Console.WriteLine(output);
        }
    }
    public class CallHistoryWithIntArrayComparer : CallsComparer
    {
        public CallHistoryWithIntArrayComparer(TestContext ctx, int startIndex, int sizeIndex)
        {
            Index = startIndex;
            Size = sizeIndex;
            CtxTestContext = ctx;

        }

        public override int Compare(object callHistory, object interviewIts)
        {
            int? x = ((CallHistory)callHistory)?.ExtendedStatus;
            Debug.Assert(interviewIts != null, nameof(interviewIts) + " != null");
            int y = Convert.ToInt32(interviewIts);

            Index++;

            if (x != y)
            {
                WriteLine(String.Format(@"No expected ITS from Callhistory: {0}, actual: {1}, expected:{2}", Index, x, y));
                //Console.WriteLine(@"No expected ITS from Callhistory: {0}, actual: {1}, expected:{2}", Index, x, y);
                NotEqual = true;
            }

            if (Index == Size && NotEqual)
                return -1;

            return 0;
        }
    }

    public class CallHistoryComparer : CallsComparer
    {

        public CallHistoryComparer(TestContext ctx, int startIndex, int sizeIndex)
        {
            Index = startIndex;
            Size = sizeIndex;
            CtxTestContext = ctx;
        }

        public override int Compare(object callHistory, object callHistoryWithVars)
        {
            var x = (CallHistory)callHistory;

            var y = (CallHistoryWithVariables)callHistoryWithVars;

            var surveyNameFromTestContext = CtxTestContext.DataRow["surveyName"].ToString();

            Index++;

            if ((x == null || y == null) ||
                (x.InterviewId != y.InterviewId) ||
                (x.ExtendedStatus != y.ExtendedStatus) ||
                (x.InterviewerId != y.InterviewerId) ||
                (x.Duration != y.Duration) ||
                (x.SurveyId != y.SurveyId) ||
                // ReSharper disable once PossibleInvalidOperationException : y.Time could be null, ignore for now
                (x.Time.UtcDateTime.ToString(CultureInfo.InvariantCulture) != y.Time.Value.UtcDateTime.ToString(CultureInfo.InvariantCulture)) ||
                (x.TelephoneNumber != y.TelephoneNumber) ||
                (y.SurveyName != surveyNameFromTestContext) ||
                (x.WaitingTime != y.WaitingTime))
            {
                Debug.Assert(x != null, nameof(x) + " != null");
                Debug.Assert(y != null, nameof(y) + " != null");
                WriteLine(String.Format(@"CallHistories aren't equal! InterviewId: {0}, Times: {1} <> {2}", Index, x.Time, y.Time));

                NotEqual = true;
            }

            if (Index == Size && NotEqual)
                return -1;

            return 0;
        }
    }
    public class InterviewsComparer : CallsComparer
    {
        public InterviewsComparer(TestContext ctx, int startIndex, int sizeIndex, string comparedFieldName)
        {
            FieldName = comparedFieldName;
            NotEqual = false;
            Index = startIndex;
            Size = sizeIndex;
            CtxTestContext = ctx;

        }
        public override int Compare(object x, object y)
        {
            Index++;
            if ((int?)x != (int?)y)
            {
                WriteLine(String.Format(@"Not expected ITS from DB! InterviewId: {0}, {1}: {2}, {3}", Index, FieldName, x, y));
                NotEqual = true;
            }

            if (Index == Size && NotEqual)
                return -1;

            return 0;
        }
    }

}