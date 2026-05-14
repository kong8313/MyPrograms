using System;
using Confirmit.CATI.Supervisor.Core.Activity;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.Activity.Fakes
{
    public class StubIAlertConstraint : IAlertConstraint 
    {
        private IAlertConstraint _inner;

        public StubIAlertConstraint()
        {
            _inner = null;
        }

        public IAlertConstraint Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool IsConstraintForBvThresholdTypeDelegate(BvThresholdType type);
        public IsConstraintForBvThresholdTypeDelegate IsConstraintForBvThresholdType;

        bool IAlertConstraint.IsConstraintFor(BvThresholdType type)
        {


            if (IsConstraintForBvThresholdType != null)
            {
                return IsConstraintForBvThresholdType(type);
            } else if (_inner != null)
            {
                return ((IAlertConstraint)_inner).IsConstraintFor(type);
            }

            return default(bool);
        }

        public delegate void ValidateSurveyAlertInfoIEnumerableOfSurveyAlertInfoDelegate(SurveyAlertInfo alert, IEnumerable<SurveyAlertInfo> alertsInSystem);
        public ValidateSurveyAlertInfoIEnumerableOfSurveyAlertInfoDelegate ValidateSurveyAlertInfoIEnumerableOfSurveyAlertInfo;

        void IAlertConstraint.Validate(SurveyAlertInfo alert, IEnumerable<SurveyAlertInfo> alertsInSystem)
        {

            if (ValidateSurveyAlertInfoIEnumerableOfSurveyAlertInfo != null)
            {
                ValidateSurveyAlertInfoIEnumerableOfSurveyAlertInfo(alert, alertsInSystem);
            } else if (_inner != null)
            {
                ((IAlertConstraint)_inner).Validate(alert, alertsInSystem);
            }
        }

    }
}