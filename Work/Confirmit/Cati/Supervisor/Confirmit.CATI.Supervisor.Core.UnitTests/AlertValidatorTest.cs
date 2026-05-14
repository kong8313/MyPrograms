using System.Collections.Generic;

using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Supervisor.Core.Activity;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    [TestClass]
    public class AlertValidatorTest
    {
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void Validate_LastSumbissionAmberEqualsToLastSumbissionRed_OK()
        {
            var alert = new SurveyAlertInfo(0, 25, 25, (int)BvThresholdType.LastSubmissionAlert);
            new AlertValidator(new List<SurveyAlertInfo>()).Validate(alert);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void Validate_LastSumbissionAmberLessThenLastSumbissionRed_OK()
        {
            var alert = new SurveyAlertInfo(0, 15, 25, (int)BvThresholdType.LastSubmissionAlert);
            new AlertValidator(new List<SurveyAlertInfo>()).Validate(alert);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void Validate_QuickAnswerSubmissionAmberEqualsToQuickAnswerSubmissionRed_OK()
        {
            var alert = new SurveyAlertInfo(0, 5, 5, (int)BvThresholdType.QuickAnswerSubmissionAlert);
            new AlertValidator(new List<SurveyAlertInfo>()).Validate(alert);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void Validate_QuickAnswerSubmissionAmberMoreThenQuickAnswerSubmissionRed_OK()
        {
            var alert = new SurveyAlertInfo(0, 5, 3, (int)BvThresholdType.QuickAnswerSubmissionAlert);
            new AlertValidator(new List<SurveyAlertInfo>()).Validate(alert);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), ExpectedException(typeof(UserMessageException))]
        public void Validate_LastSumbissionAmberMoreThenLastSumbissionRed_Error()
        {
            var alert = new SurveyAlertInfo(0, 25, 15, (int)BvThresholdType.LastSubmissionAlert);
            new AlertValidator(new List<SurveyAlertInfo>()).Validate(alert);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), ExpectedException(typeof(UserMessageException))]
        public void Validate_QuickAnswerSubmissionAmberLessThenQuickAnswerSubmissionRed_Error()
        {
            var alert = new SurveyAlertInfo(0, 3, 5, (int)BvThresholdType.QuickAnswerSubmissionAlert);
            new AlertValidator(new List<SurveyAlertInfo>()).Validate(alert);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void Validate_QuickAnswerSubmissionAmberLessThenLastSumbissionAmber_OK()
        {
            var alert1 = new SurveyAlertInfo(0, 5, 3, (int)BvThresholdType.QuickAnswerSubmissionAlert);
            var alert2 = new SurveyAlertInfo(0, 25, 50, (int)BvThresholdType.LastSubmissionAlert);

            new AlertValidator(new List<SurveyAlertInfo> { alert2 }).Validate(alert1);
            new AlertValidator(new List<SurveyAlertInfo> { alert1 }).Validate(alert2);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), ExpectedException(typeof(UserMessageException))]
        public void Validate_QuickAnswerSubmissionAmberMoreThenLastSumbissionAmber_ValidateAlert1_Error()
        {
            var alert1 = new SurveyAlertInfo(0, 30, 5, (int)BvThresholdType.QuickAnswerSubmissionAlert);
            var alert2 = new SurveyAlertInfo(0, 25, 50, (int)BvThresholdType.LastSubmissionAlert);

            new AlertValidator(new List<SurveyAlertInfo> { alert2 }).Validate(alert1);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), ExpectedException(typeof(UserMessageException))]
        public void Validate_QuickAnswerSubmissionAmberMoreThenLastSumbissionAmber_ValidateAlert2_Error()
        {
            var alert1 = new SurveyAlertInfo(0, 30, 5, (int)BvThresholdType.QuickAnswerSubmissionAlert);
            var alert2 = new SurveyAlertInfo(0, 25, 50, (int)BvThresholdType.LastSubmissionAlert);

            new AlertValidator(new List<SurveyAlertInfo> { alert1 }).Validate(alert2);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), ExpectedException(typeof(UserMessageException))]
        public void Validate_QuickAnswerSubmissionAmberEqualsToLastSumbissionAmber_ValidateAlert1_Error()
        {
            var alert1 = new SurveyAlertInfo(0, 25, 5, (int)BvThresholdType.QuickAnswerSubmissionAlert);
            var alert2 = new SurveyAlertInfo(0, 25, 50, (int)BvThresholdType.LastSubmissionAlert);

            new AlertValidator(new List<SurveyAlertInfo> { alert2 }).Validate(alert1);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), ExpectedException(typeof(UserMessageException))]
        public void Validate_QuickAnswerSubmissionAmberEqualsToLastSumbissionAmber_ValidateAlert2_Error()
        {
            var alert1 = new SurveyAlertInfo(0, 25, 5, (int)BvThresholdType.QuickAnswerSubmissionAlert);
            var alert2 = new SurveyAlertInfo(0, 25, 50, (int)BvThresholdType.LastSubmissionAlert);

            new AlertValidator(new List<SurveyAlertInfo> { alert1 }).Validate(alert2);
        }
    }
}
