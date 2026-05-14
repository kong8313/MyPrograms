using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.UnitTests.SurveyPermissionVerifier
{
    [TestClass]
    public class SurveyPermissionVerifierTest
    {
        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        [ExpectedException(typeof(ArgumentException))]
        public void VerifySurveyPermissison_ParameterIsRequiedButNotSupplied_Success()
        {
            new Classes.SurveyPermissionVerifier(new StubPageParamererIsRequiedButNotSupplied(), String.Empty).Verify();            
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void VerifySurveyPermissison_ParameterIsNotRequiedAndNotSupplied_Success()
        {
            new Classes.SurveyPermissionVerifier(new StubPageParamererIsNotRequiedAndNotSupplied(), String.Empty).Verify();
        }        
    }
}
