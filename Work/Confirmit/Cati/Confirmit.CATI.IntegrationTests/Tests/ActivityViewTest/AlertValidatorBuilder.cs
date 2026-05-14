using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.Activity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.ActivityViewTest
{
    public class AlertValidatorBuilder
    {
        private readonly IntegrationTestingFramework _testingFramework;
        private readonly int _surveySid;
        private readonly BvThresholdType _thresholdType;

        private AlertStatus _expectedNormalThresholdAlertStatus;
        private AlertStatus _expectedWarningThresholdAlertStatus;
        private AlertStatus _expectedErrorThresholdAlertStatus;
        private readonly IActivityManager _activityManager;

        public AlertValidatorBuilder(IntegrationTestingFramework testingFramework, int surveySid, BvThresholdType thresholdType)
        {
            _testingFramework = testingFramework;
            _surveySid = surveySid;
            _thresholdType = thresholdType;
            _activityManager = ServiceLocator.Resolve<IActivityManager>();
        }

        public AlertValidatorBuilder DefaultSetUp()
        {
            _expectedNormalThresholdAlertStatus = AlertStatus.Ok;
            _expectedWarningThresholdAlertStatus = AlertStatus.Ok;
            _expectedErrorThresholdAlertStatus = AlertStatus.Ok;

            return this;
        }

        public AlertValidatorBuilder SetUp(AlertStatus expectedNormalThresholdAlertStatus, AlertStatus expectedWarningThresholdAlertStatus, AlertStatus expectedErrorThresholdAlertStatus)
        {
            _expectedNormalThresholdAlertStatus = expectedNormalThresholdAlertStatus;
            _expectedWarningThresholdAlertStatus = expectedWarningThresholdAlertStatus;
            _expectedErrorThresholdAlertStatus = expectedErrorThresholdAlertStatus;

            ActivityManager.SetAlert(new SurveyAlertInfo(0, 300, 600, (int)_thresholdType));

            return this;
        }

        public void Validate(Func<TaskActivityInfo, AlertStatus> activityInfoPropertyToValidate, string supervisorName)
        {
            var sqlDateTimeMocker = new DateTimeMocker(_testingFramework);

            IsAlertStatusCorrect(activityInfoPropertyToValidate, _surveySid, _expectedNormalThresholdAlertStatus, supervisorName);

            sqlDateTimeMocker.MockOffset(450);

            IsAlertStatusCorrect(activityInfoPropertyToValidate, _surveySid, _expectedWarningThresholdAlertStatus, supervisorName);

            sqlDateTimeMocker.MockOffset(605);

            IsAlertStatusCorrect(activityInfoPropertyToValidate, _surveySid, _expectedErrorThresholdAlertStatus, supervisorName);
        }

        private void IsAlertStatusCorrect(Func<TaskActivityInfo, AlertStatus> propToValidate, int surveySid, AlertStatus checkedAlertStatus, string supervisorName)
        {
            var dataList = _activityManager.GetTasksActivityData(String.Empty, true, true, new[] { surveySid }, new int[0], supervisorName);

            if (propToValidate(dataList[0]) != checkedAlertStatus)
            {
                Assert.Fail("Wrong activity alert. Current: {0}. Expected: {1}", propToValidate(dataList[0]), checkedAlertStatus);
            }
        }
    }
}