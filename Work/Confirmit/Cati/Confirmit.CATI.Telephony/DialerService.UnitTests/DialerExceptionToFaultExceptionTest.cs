using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using Confirmit.CATI.Telephony.DialerService;
using Confirmit.CATI.Telephony.DialerService.Contract;
using ConfirmitDialerInterface;
using DialerCommon.DialerParameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerService.UnitTests
{
    class TestDialerException : DialerException
    {
        public TestDialerException(DialerErrorCode errorCode, string exceptionMessage)
            : base(errorCode, exceptionMessage)
        {
        }
    }

    [TestClass]
    public class DialerExceptionToFaultExceptionTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod, Owner(@"FIRM\alm")]
        public void DialerExceptionIsConvertedToFaultCorrectly()
        {
            const DialerErrorCode expectedErrorCode = DialerErrorCode.Forbidden;
            const string expectedExceptionMessage = "Test DialerException qdxwcedcwdwe1426658";

            var target = new DialerExceptionToFaultException();

            var exception = target.Convert(new DialerException(expectedErrorCode, expectedExceptionMessage));

            Assert.IsInstanceOfType(exception, typeof(FaultException<DialerExceptionDetail>), "Conversion is wrong.");

            var faultExceptionDetail = ((FaultException<DialerExceptionDetail>) exception).Detail;

            Assert.AreEqual(expectedErrorCode, faultExceptionDetail.ErrorCode, "Error code is wrong.");
            Assert.IsTrue(faultExceptionDetail.ErrorString.Contains(expectedExceptionMessage),
                string.Format("Error message is wrong. Expected to be contained: [{0}]. Actual: [{1}].",
                    expectedExceptionMessage, faultExceptionDetail.ErrorString));
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void UnknownDescendantExceptionIsConvertedToFaultCorrectly()
        {
            const DialerErrorCode expectedErrorCode = DialerErrorCode.InvalidPhoneNumber;
            const string expectedExceptionMessage = "Test DialerException sfffsg34534gwsdg";

            var target = new DialerExceptionToFaultException();

            var exception = target.Convert(new TestDialerException(expectedErrorCode, expectedExceptionMessage));

            Assert.IsInstanceOfType(exception, typeof(FaultException<DialerExceptionDetail>), "Conversion is wrong.");

            var faultExceptionDetail = ((FaultException<DialerExceptionDetail>) exception).Detail;

            Assert.AreEqual(expectedErrorCode, faultExceptionDetail.ErrorCode, "Error code is wrong.");
            Assert.IsTrue(faultExceptionDetail.ErrorString.Contains(expectedExceptionMessage),
                string.Format("Error message is wrong. Expected to be contained: [{0}]. Actual: [{1}].",
                    expectedExceptionMessage, faultExceptionDetail.ErrorString));
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void ParametersExceptionExceptionIsConvertedToFaultCorrectly()
        {
            const string expectedExceptionMessage = "Test DialerException ParametersExceptionExceptionIsConvertedToFaultCorrectly";

            DialerException parametersException = new ParametersException(new Collection<DialerParameterError>
            {
                new DialerParameterError("id1", "param1", expectedExceptionMessage + " /1"),
                new DialerParameterError("id2", "param2", expectedExceptionMessage + " /2"),
                new DialerParameterError("id3", "param3", expectedExceptionMessage + " /3")
            });

            var target = new DialerExceptionToFaultException();

            var exception = target.Convert(parametersException);

            Assert.IsInstanceOfType(exception, typeof(FaultException<DialerParametersExceptionDetails>), "Conversion is wrong.");

            var faultExceptionDetail = ((FaultException<DialerParametersExceptionDetails>) exception).Detail;
            var faultExceptionString = 
                ((DialerParametersException) faultExceptionDetail.ToException()).GetExceptionFormattedString();

            Assert.IsTrue(faultExceptionString.Contains(parametersException.Message),
                string.Format("Error message is wrong. Expected to be contained: [{0}]. Actual: [{1}].",
                    parametersException.Message, faultExceptionString));
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void AllDescendantExceptionsAreConvertedToFaultsCorrectly()
        {
            string[] expectedExceptionMessages =
            {
                "Test DialerIsNotInitializedException [AllDescendantExceptionsAreConvertedToFaultsCorrectly/1]",
                "Test ExtensionIsAlreadyInUseException [AllDescendantExceptionsAreConvertedToFaultsCorrectly/2]",
                "Test ParametersException [AllDescendantExceptionsAreConvertedToFaultsCorrectly/3]"
            };

            // Note: All DialerException derivated types should be presented in the derivedExceptions dictionary below
            DialerException[] descendantExceptions =
            {
                new DialerIsNotInitializedException(expectedExceptionMessages[0]),
                new ExtensionIsAlreadyInUseException(expectedExceptionMessages[1]),
                new ParametersException(new Collection<DialerParameterError>
                {
                    new DialerParameterError("id1", "param1", expectedExceptionMessages[2])
                })
            };

            var typeToException = new Dictionary<Type, DialerException>
                {
                    { typeof (DialerIsNotInitializedException), descendantExceptions[0] },
                    { typeof (ExtensionIsAlreadyInUseException), descendantExceptions[1] },
                    { typeof (ParametersException), descendantExceptions[2] }
                };

            var dialerExceptionType = typeof(DialerException);

            var descendantTypes =
                from type in dialerExceptionType.Assembly.GetTypes()
                where type.IsSubclassOf(dialerExceptionType)
                select type;

            var target = new DialerExceptionToFaultException();

            foreach (var descendantType in descendantTypes)
            {
                var exceptionFromType = typeToException[descendantType];

                if (exceptionFromType is ParametersException)
                {
                    // The ParametersException is processed in special way.
                    // See ParametersExceptionExceptionIsConvertedToFaultCorrectly() test.
                    continue;
                }

                var exception = target.Convert(exceptionFromType);

                Assert.IsInstanceOfType(exception, typeof(FaultException<DialerExceptionDetail>), "Conversion is wrong.");

                var faultExceptionDetail = ((FaultException<DialerExceptionDetail>) exception).Detail;
                Assert.AreEqual(exceptionFromType.ErrorCode, faultExceptionDetail.ErrorCode, "Error code is wrong.");

                Assert.IsTrue(faultExceptionDetail.ErrorString.Contains(exceptionFromType.Message),
                    string.Format("Error message is wrong. Expected to be contained: [{0}]. Actual: [{1}].",
                    exceptionFromType.Message, faultExceptionDetail.ErrorString));
            }
        }
    }
}
