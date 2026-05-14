using System;
using System.Diagnostics;
using BootstrapperLibrary;
using BootstrapperLibrary.Interfaces;
using BootstrapperLibrary.Properties;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.Setup.UnitTests.FakeClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Setup.UnitTests
{
    [TestClass]
    public class BootstrapperEngineTests
    {
        const string ParameterName = "Param";
        const string ParameterValue = "Value";

        private IObjectFactory _objectFactory;
        private ILogger _logger;
        private IBootstrapperEngine _bootstrapperEngine;

        [TestInitialize]
        public void TestInitialize()
        {
            _objectFactory = new FakeObjectFactory();
            _logger = new TraceLogger();
            _bootstrapperEngine = new BootstrapperEngine(_objectFactory, _logger);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ShowMessageBox_CallMethodInSilentMode_NoMessageAppears()
        {
            _bootstrapperEngine.IsQuietMode = true;

            _bootstrapperEngine.ShowMessageBox("Test", TraceEventType.Information);

            Assert.AreEqual(0, ((FakeObjectFactory)_objectFactory).CreatedFakeDialogService.ExecutingCount, "Quite mode works wrong");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ShowMessageBox_CallMethodWithDifferentType_OneInformationTwoWarningsAndTreeErrorMessagesWillAppear()
        {
            _bootstrapperEngine.ShowMessageBox("Test", TraceEventType.Information);
            _bootstrapperEngine.ShowMessageBox("Test1", TraceEventType.Warning);
            _bootstrapperEngine.ShowMessageBox("Test2", TraceEventType.Warning);
            _bootstrapperEngine.ShowMessageBox("Test1", TraceEventType.Error);
            _bootstrapperEngine.ShowMessageBox("Test2", TraceEventType.Error);
            _bootstrapperEngine.ShowMessageBox("Test3", TraceEventType.Error);

            Assert.AreEqual(1, ((FakeObjectFactory)_objectFactory).CreatedFakeDialogService.InformationExecutingCount, "ShowMessageBox shows information messages wrong");
            Assert.AreEqual(2, ((FakeObjectFactory)_objectFactory).CreatedFakeDialogService.WarningExecutingCount, "ShowMessageBox shows warning messages wrong");
            Assert.AreEqual(3, ((FakeObjectFactory)_objectFactory).CreatedFakeDialogService.ErrorExecutingCount, "ShowMessageBox shows error messages wrong");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ShowMessageBox_CallMethodWithUnsopportedType_OneErrorMessageWithWillAppear()
        {
            _bootstrapperEngine.ShowMessageBox("Test", TraceEventType.Start);

            Assert.AreEqual(1, ((FakeObjectFactory)_objectFactory).CreatedFakeDialogService.ErrorExecutingCount, "ShowMessageBox shows unknown type messages wrong");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void VerifyParameterValue_CallMethodWithIncorrectValue_CorrectOutputSringReturns()
        {
            var acceptableValues = new[] { "Value1", "Value2", "Value3" };

            var correctOutput = string.Format(Resources.WrongParameterValueAndAcceptableValue,
                    ParameterName,
                    ParameterValue,
                    "Value1 | Value2 | Value3");
            
            string output = _bootstrapperEngine.VerifyParameterValue(ParameterName, ParameterValue, acceptableValues);

            Assert.AreEqual(output, correctOutput, "VerifyParameterValue returns incorrect string");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void VerifyParameterValue_CallMethodWithCorrectValue_EmptyOutputSringReturns()
        {
            var acceptableValues = new[] { "Value", "Value2", "Value3" };

            string output = _bootstrapperEngine.VerifyParameterValue(ParameterName, ParameterValue, acceptableValues);

            Assert.AreEqual(output, string.Empty, "VerifyParameterValue returns incorrect string");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void VerifyParameterValue_CallMethodWithCorrectValueButWithoutAcceptableValues_CorrectOutputSringReturns()
        {
            var acceptableValues = new string[0];
            
            _bootstrapperEngine.VerifyParameterValue(ParameterName, ParameterValue, acceptableValues);
        }
    }
}
