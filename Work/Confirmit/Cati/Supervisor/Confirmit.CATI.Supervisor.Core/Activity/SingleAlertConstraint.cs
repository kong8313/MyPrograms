using System;
using System.Collections.Generic;

using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Supervisor.Core.Activity
{
    public class SingleAlertConstraint : IAlertConstraint
    {
        private readonly BvThresholdType _type;

        private readonly Func<SurveyAlertInfo, bool> _validator;
        private readonly string _errorMessage;

        public SingleAlertConstraint(BvThresholdType type, Func<SurveyAlertInfo, bool> validator, string errorMessage)
        {
            _type = type;
            _validator = validator;
            _errorMessage = errorMessage;
        }

        public void Validate(SurveyAlertInfo alert, IEnumerable<SurveyAlertInfo> alertsInSystem)
        {
            if (!_validator(alert))
            {
                throw new UserMessageException(_errorMessage);
            }
        }

        public bool IsConstraintFor(BvThresholdType type)
        {
            return _type == type;
        }
    }
}