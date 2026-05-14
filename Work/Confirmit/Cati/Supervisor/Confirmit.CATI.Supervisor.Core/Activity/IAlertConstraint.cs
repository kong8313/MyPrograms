using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.Activity
{
    public interface IAlertConstraint
    {
        bool IsConstraintFor(BvThresholdType type);

        void Validate(SurveyAlertInfo alert, IEnumerable<SurveyAlertInfo> alertsInSystem);
    }
}