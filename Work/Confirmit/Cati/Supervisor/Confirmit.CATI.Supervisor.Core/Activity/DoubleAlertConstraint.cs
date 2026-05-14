using System;
using System.Collections.Generic;
using System.Linq;

using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Supervisor.Core.Activity
{
    public class DoubleAlertConstraint : IAlertConstraint
    {
        private readonly BvThresholdType _type1;

        private readonly BvThresholdType _type2;

        private readonly Func<SurveyAlertInfo, SurveyAlertInfo, bool> _validator;
        private readonly string _errorMessage;

        public DoubleAlertConstraint(BvThresholdType type1, BvThresholdType type2, Func<SurveyAlertInfo, SurveyAlertInfo, bool> validator, string errorMessage)
        {
            _type1 = type1;
            _type2 = type2;
            _validator = validator;
            _errorMessage = errorMessage;
        }

        public bool IsConstraintFor(BvThresholdType type)
        {
            return _type1 == type || _type2 == type;
        }

        public void Validate(SurveyAlertInfo alert, IEnumerable<SurveyAlertInfo> alertsInSystem)
        {
            SurveyAlertInfo alert1 = null;
            SurveyAlertInfo alert2 = null;
            if (alert.ThresholdType == _type1)
            {
                alert1 = alert;
                alert2 = alertsInSystem.FirstOrDefault(x => x.ThresholdType == _type2);
            }
            else if (alert.ThresholdType == _type2)
            {
                alert1 = alertsInSystem.FirstOrDefault(x => x.ThresholdType == _type1);
                alert2 = alert;
            }

            if (alert1 != null && alert2 != null)
            {
                Validate(alert1, alert2);
            }
        }

        private void Validate(SurveyAlertInfo alert1, SurveyAlertInfo alert2)
        {
            if (!_validator(alert1, alert2))
            {
                throw new UserMessageException(_errorMessage);
            }
        }
    }
}