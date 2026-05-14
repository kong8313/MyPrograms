using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Core.Activity
{
    public class AlertValidator
    {
        private static readonly IAlertConstraint[] Constraints = new IAlertConstraint[]
            {
                new SingleAlertConstraint(BvThresholdType.QuickAnswerSubmissionAlert, alert => alert.Red <= alert.Amber, Strings.WarningThresholdMustBeGreater),
                new SingleAlertConstraint(BvThresholdType.LastSubmissionAlert, alert => alert.Red >= alert.Amber, Strings.RedThresholdMustBeGreater),
                new DoubleAlertConstraint(
                    BvThresholdType.QuickAnswerSubmissionAlert,
                    BvThresholdType.LastSubmissionAlert,
                    (alert1, alert2) => alert1.Amber < alert2.Amber, Strings.QuickAnswerSubmissionAlertMustBeLessThatLastSubmissionAlert),
                new SingleAlertConstraint(BvThresholdType.NoActivityAlert, alert => alert.Red >= alert.Amber, Strings.RedThresholdMustBeGreater),
                new SingleAlertConstraint(BvThresholdType.InterviewDurationAlert, alert => alert.Red >= alert.Amber, Strings.RedThresholdMustBeGreater),
                new SingleAlertConstraint(BvThresholdType.BreakDurationAlert, alert => alert.Red >= alert.Amber, Strings.RedThresholdMustBeGreater),
            };

        private readonly List<SurveyAlertInfo> _alertsInSystem;

        public AlertValidator(List<SurveyAlertInfo> alertsInSystem)
        {
            _alertsInSystem = alertsInSystem;
        }

        public void Validate(SurveyAlertInfo alert)
        {
            foreach (var constraint in Constraints)
            {
                if (constraint.IsConstraintFor(alert.ThresholdType))
                {
                    constraint.Validate(alert, _alertsInSystem);
                }
            }
        }
    }
}