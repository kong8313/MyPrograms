using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Confirmit.CATI.Core.Schedules2007.Validation;
using Confirmit.CATI.Core.SystemSettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Scheduling
{
    /// <summary>
    /// Unit tests for SchedulingScriptSecurityValidator
    /// Tests the namespace-level security whitelisting with .gitignore-style patterns
    /// </summary>
    [TestClass]
    public class SchedulingScriptSecurityValidatorTest : BaseTest
    {
        private string _testAssemblyPath;
        private ISchedulingScriptSettings _mockSettings;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            _testAssemblyPath = Path.Combine(Path.GetTempPath(), "TestAssembly_" + Guid.NewGuid() + ".dll");
            _mockSettings = new MockSchedulingScriptSettings();
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
            if (File.Exists(_testAssemblyPath))
            {
                File.Delete(_testAssemblyPath);
            }
        }

        #region Whitelisted Namespace Tests

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemNamespace_Allowed()
        {
            // Arrange: Create assembly that calls System.String methods
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.String", "Concat", new[] { typeof(string), typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "System.String.Concat should be allowed");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        [TestMethod] 
        [TestCategory("Security")] 
        [Owner(@"FIRM\DmitryS")]
        public void Validate_MultipleSameSystemNamespace_Allowed()
        {
            // Arrange: Create assembly that calls System.String methods
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.String", "Concat", new[] { typeof(string), typeof(string) }),
                ("System.String", "Concat", new[] { typeof(string), typeof(string) }),
                ("System.String", "Concat", new[] { typeof(string), typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "System.String.Concat should be allowed");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemMath_Allowed()
        {
            // Arrange: Create assembly that calls System.Math methods
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Math", "Max", new[] { typeof(int), typeof(int) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "System.Math.Max should be allowed");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_MicrosoftJScript_Allowed()
        {
            // Arrange: Create assembly that calls Microsoft.JScript methods
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("Microsoft.JScript.Convert", "ToString", new[] { typeof(object), typeof(bool) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "Microsoft.JScript.Convert.ToString should be allowed");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        #endregion

        #region Excluded Namespace Tests

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemIO_Blocked()
        {
            // Arrange: Create assembly that calls System.IO.File methods
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.IO.File", "Delete", new[] { typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "System.IO.File.Delete should be blocked");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("System.IO.File"));
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemNet_Blocked()
        {
            // Arrange: Create assembly that calls System.Net methods
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Net.WebClient", "DownloadString", new[] { typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "System.Net.WebClient.DownloadString should be blocked");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("System.Net.WebClient"));
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemReflection_Blocked()
        {
            // Arrange: Create assembly that calls System.Reflection methods
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Reflection.Assembly", "Load", new[] { typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "System.Reflection.Assembly.Load should be blocked");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("System.Reflection.Assembly"));
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemDiagnosticsProcess_Blocked()
        {
            // Arrange: Create assembly that calls System.Diagnostics.Process methods
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Diagnostics.Process", "Start", new[] { typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "System.Diagnostics.Process.Start should be blocked");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("System.Diagnostics.Process"));
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemData_Blocked()
        {
            // Arrange: Create assembly that calls System.Data methods
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Data.SqlClient.SqlConnection", "Open", new Type[0])
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "System.Data.SqlClient.SqlConnection.Open should be blocked");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("System.Data"));
        }

        #endregion

        #region Explicit Whitelist Tests

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_ExplicitWhitelist_Allowed()
        {
            // Arrange: Add a Confirmit.CATI method to the whitelist
            var mockSettings = new MockSchedulingScriptSettings();
            mockSettings.SecureExternalMethodList.Add("System.Void Confirmit.CATI.Core.TestClass::TestMethod()");

            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("Confirmit.CATI.Core.TestClass", "TestMethod", new Type[0])
            });

            var validator = new SchedulingScriptSecurityValidator(mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "Explicitly whitelisted method should be allowed");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_ConfirmitCATI_BlockedByDefault()
        {
            // Arrange: Create assembly that calls Confirmit.CATI method NOT in whitelist
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("Confirmit.CATI.Core.TestClass", "DangerousMethod", new Type[0])
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "Confirmit.CATI methods should be blocked by default");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("Confirmit.CATI.Core.TestClass"));
        }

        #endregion

        #region Precedence Tests

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_ExclusionTakesPrecedence_OverWhitelist()
        {
            // Arrange: System.IO is excluded even though System is whitelisted
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.IO.File", "ReadAllText", new[] { typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "Exclusion should take precedence over namespace whitelist");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_ExplicitWhitelist_TakesPrecedence_OverExclusion()
        {
            // Arrange: Add a System.IO method to explicit whitelist
            var mockSettings = new MockSchedulingScriptSettings();
            mockSettings.SecureExternalMethodList.Add("System.Void System.IO.File::ReadAllText(System.String)");

            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.IO.File", "ReadAllText", new[] { typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "Explicit whitelist should take precedence over exclusion");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        #endregion

        #region Multiple Calls Tests
        
        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_MultipleSecureCalls_NothingReported()
        {
            // Arrange: Create assembly with both allowed and blocked calls
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.String", "Concat", new[] { typeof(string), typeof(string) }), // Allowed
                ("System.Math", "Max", new[] { typeof(int), typeof(int) }) // Allowed
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "Should be valid if all calls are allowed");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_MultipleCalls_MixedSecurity()
        {
            // Arrange: Create assembly with both allowed and blocked calls
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.String", "Concat", new[] { typeof(string), typeof(string) }), // Allowed
                ("System.IO.File", "Delete", new[] { typeof(string) }), // Blocked
                ("System.Math", "Max", new[] { typeof(int), typeof(int) }) // Allowed
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "Should be invalid if any call is blocked");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("System.IO.File"));
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_MultipleDangerousCalls_AllReported()
        {
            // Arrange: Create assembly with multiple blocked calls
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.IO.File", "Delete", new[] { typeof(string) }),
                ("System.Net.WebClient", "DownloadString", new[] { typeof(string) }),
                ("System.Reflection.Assembly", "Load", new[] { typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure);
            Assert.AreEqual(3, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls.Any(c => c.Contains("System.IO.File")));
            Assert.IsTrue(result.UnsecureCalls.Any(c => c.Contains("System.Net.WebClient")));
            Assert.IsTrue(result.UnsecureCalls.Any(c => c.Contains("System.Reflection.Assembly")));
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_EmptyAssembly_Valid()
        {
            // Arrange: Create assembly with no external calls
            TestUtility.CreateTestAssembly(_testAssemblyPath, new (string, string, Type[])[0]);

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "Empty assembly should be valid");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_UnknownNamespace_Blocked()
        {
            // Arrange: Create assembly that calls methods from unknown namespace
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("UnknownNamespace.UnknownClass", "UnknownMethod", new Type[0])
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "Unknown namespace should be blocked by default");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_DuplicateCalls_ReportedOnce()
        {
            // Arrange: Create assembly with duplicate dangerous calls
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.IO.File", "Delete", new[] { typeof(string) }),
                ("System.IO.File", "Delete", new[] { typeof(string) }),
                ("System.IO.File", "Delete", new[] { typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure);
            Assert.AreEqual(1, result.UnsecureCalls.Length, "Duplicate calls should be reported only once");
        }

        #endregion

        #region Security Bypass Prevention Tests

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemActivator_Blocked()
        {
            // Arrange: System.Activator can be used to create arbitrary types
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Activator", "CreateInstance", new[] { typeof(Type) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "System.Activator.CreateInstance should be blocked");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("System.Activator"));
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemDelegate_Blocked()
        {
            // Arrange: System.Delegate can be used to create delegates to arbitrary methods
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Delegate", "CreateDelegate", new[] { typeof(Type), typeof(object), typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "System.Delegate.CreateDelegate should be blocked");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("System.Delegate"));
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemType_Blocked()
        {
            // Arrange: System.Type can be used for reflection-based attacks
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Type", "GetType", new[] { typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "System.Type.GetType should be blocked");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("System.Type"));
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemEnvironment_Blocked()
        {
            // Arrange: System.Environment can access environment variables and system info
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Environment", "GetEnvironmentVariable", new[] { typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "System.Environment.GetEnvironmentVariable should be blocked");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("System.Environment"));
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemAppDomain_Blocked()
        {
            // Arrange: System.AppDomain can load assemblies and create isolated domains
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.AppDomain", "Load", new[] { typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "System.AppDomain.Load should be blocked");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("System.AppDomain"));
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemRuntimeInteropServices_Blocked()
        {
            // Arrange: System.Runtime.InteropServices can be used for P/Invoke
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Runtime.InteropServices.Marshal", "Copy", new[] { typeof(IntPtr), typeof(byte[]), typeof(int), typeof(int) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "System.Runtime.InteropServices.Marshal.Copy should be blocked");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("System.Runtime.InteropServices"));
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemThreading_Blocked()
        {
            // Arrange: System.Threading can be used for DoS attacks
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Threading.Thread", "Start", new Type[0])
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "System.Threading.Thread.Start should be blocked");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("System.Threading"));
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemSecurity_Blocked()
        {
            // Arrange: System.Security can be used to manipulate security
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Security.Principal.WindowsIdentity", "GetCurrent", new Type[0])
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "System.Security.Principal.WindowsIdentity.GetCurrent should be blocked");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("System.Security"));
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemXml_Blocked()
        {
            // Arrange: System.Xml can be vulnerable to XXE attacks
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Xml.XmlDocument", "Load", new[] { typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "System.Xml.XmlDocument.Load should be blocked");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("System.Xml"));
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemComponentModel_Blocked()
        {
            // Arrange: System.ComponentModel can be used for dynamic invocation
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.ComponentModel.TypeDescriptor", "GetProperties", new[] { typeof(object) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsFalse(result.IsSecure, "System.ComponentModel.TypeDescriptor.GetProperties should be blocked");
            Assert.AreEqual(1, result.UnsecureCalls.Length);
            Assert.IsTrue(result.UnsecureCalls[0].Contains("System.ComponentModel"));
        }

        #endregion

        #region Additional Whitelisted Types Tests

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemDateTime_Allowed()
        {
            // Arrange: System.DateTime should be allowed
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.DateTime", "Parse", new[] { typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "System.DateTime.Parse should be allowed");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemTimeSpan_Allowed()
        {
            // Arrange: System.TimeSpan should be allowed
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.TimeSpan", "FromMinutes", new[] { typeof(double) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "System.TimeSpan.FromMinutes should be allowed");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemGuid_Allowed()
        {
            // Arrange: System.Guid should be allowed
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Guid", "NewGuid", new Type[0])
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "System.Guid.NewGuid should be allowed");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemConvert_Allowed()
        {
            // Arrange: System.Convert should be allowed
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Convert", "ToInt32", new[] { typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "System.Convert.ToInt32 should be allowed");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemCollectionsGenericList_Allowed()
        {
            // Arrange: System.Collections.Generic.List should be allowed
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Collections.Generic.List`1", "Add", new[] { typeof(object) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "System.Collections.Generic.List.Add should be allowed");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemLinq_Allowed()
        {
            // Arrange: System.Linq should be allowed
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Linq.Enumerable", "Count", new[] { typeof(System.Collections.Generic.IEnumerable<object>) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "System.Linq.Enumerable.Count should be allowed");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemTextStringBuilder_Allowed()
        {
            // Arrange: System.Text.StringBuilder should be allowed
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Text.StringBuilder", "Append", new[] { typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "System.Text.StringBuilder.Append should be allowed");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemTextRegularExpressions_Allowed()
        {
            // Arrange: System.Text.RegularExpressions should be allowed
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Text.RegularExpressions.Regex", "IsMatch", new[] { typeof(string), typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "System.Text.RegularExpressions.Regex.IsMatch should be allowed");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        [TestMethod]
        [TestCategory("Security")]
        [Owner(@"FIRM\DmitryS")]
        public void Validate_SystemGlobalization_Allowed()
        {
            // Arrange: System.Globalization should be allowed
            TestUtility.CreateTestAssembly(_testAssemblyPath, new[]
            {
                ("System.Globalization.CultureInfo", "GetCultureInfo", new[] { typeof(string) })
            });

            var validator = new SchedulingScriptSecurityValidator(_mockSettings);

            // Act
            var result = validator.Validate(_testAssemblyPath);

            // Assert
            Assert.IsTrue(result.IsSecure, "System.Globalization.CultureInfo.GetCultureInfo should be allowed");
            Assert.AreEqual(0, result.UnsecureCalls.Length);
        }

        #endregion

        #region Mock Classes

        private class MockSchedulingScriptSettings : ISchedulingScriptSettings
        {
            public List<string> SecureExternalMethodList { get; set; } = new List<string>();
            public bool EnableRestrictedMode { get; set; }
            public int ErrorLogSize { get; set; }
            public int MaxActionsToExecute { get; set; }
            public int MaxParameters { get; set; }
            public string SecureExternalMethods { get; set; }
            public bool UseDirectDbAccess { get; set; }

            List<string> ISchedulingScriptSettings.SecureExternalMethodList => SecureExternalMethodList;
        }

        #endregion
    }
}
