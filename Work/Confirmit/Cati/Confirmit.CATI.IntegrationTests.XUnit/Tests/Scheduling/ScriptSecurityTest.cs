using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Handmade.Cache;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ScriptSecurityTest : BaseMockedIntegrationTest
    {
        [Fact, Owner(@"FIRM\MaximL")]
        public void LaunchWithoutSecurity_ScriptWithSecureCustomFunction_ExceptionIsNotThrown()
        {
            var script = new TestScript(
                new[]
                {
                    new Action(Action.Operation.RunCustomScript, "SetITS")
                },
                new Shift(1, 1, "0.00:00:00", "0.00:00:00"))
            {
                CustomScript = @"
                        function SetITS()
                        {
                            Scheduling.Interview.TransientState = 10;
                        }"
            };
            ServiceLocator.Resolve<ISchedulingScriptSettings>().EnableRestrictedMode = false;

            script.Create("test script");
        }

        [Fact, Owner(@"FIRM\MaximL")]
        public void LaunchWithSecurity_SecureScript_ExceptionIsNotThrown()
        {
            var script = new TestScript(
                new[]
                {
                    new Action(Action.Operation.RunCustomScript, "SetITS")
                },
                new Shift(1, 1, "0.00:00:00", "0.00:00:00"))
            {
                CustomScript = @"
                        function SetITS()
                        {
                            Scheduling.Interview.TransientState = 10;
                        }"
            };
            ServiceLocator.Resolve<ISchedulingScriptSettings>().EnableRestrictedMode = true;

            script.Create("test script");
        }

        [Fact, Owner(@"FIRM\MaximL")]
        public void LaunchWithoutSecurity_UnsecureScriptWithCustomScript_ExceptionIsNotThrown()
        {
            var script = new TestScript(
                new[]
                {
                    new Action(Action.Operation.RunCustomScript, "SetITS")
                },
                new Shift(1, 1, "0.00:00:00", "0.00:00:00"))
            {
                CustomScript = @"
                        function SetITS()
                        {
                            System.IO.File.Open('c:\test.txt');
                        }"
            };
            ServiceLocator.Resolve<ISchedulingScriptSettings>().EnableRestrictedMode = false;

            script.Create("test script");
        }

        [Fact, Owner(@"FIRM\MaximL")]
        public void LaunchWithSecurity_UnsecureScriptWithCustomScript_ExceptionIsThrown()
        {
            var script = new TestScript(
                new[]
                {
                    new Action(Action.Operation.RunCustomScript, "SetITS")
                },
                new Shift(1, 1, "0.00:00:00", "0.00:00:00"))
            {
                CustomScript = @"
                        function SetITS()
                        {
                            System.IO.File.Open('c:\test.txt');
                        }"
            };
            ServiceLocator.Resolve<ISchedulingScriptSettings>().EnableRestrictedMode = true;

            var exception = Xunit.Assert.Throws<UserMessageException>(() => script.Create("test script"));

            Xunit.Assert.True(exception.Message.Contains(
                "System.IO.FileStream System.IO.File::Open(System.String,System.IO.FileMode)"));
        }

        [Fact, Owner(@"FIRM\MaximL")]
        public void LaunchWithoutSecurity_UnsecureScriptWithCustomFilter_ExceptionIsNotThrown()
        {
            var script = new TestScript(
                new[]
                {
                    new Action(Action.Operation.SetNewITS, "10", "System.IO.File.Open('c:\test.txt') == null")
                },
                new Shift(1, 1, "0.00:00:00", "0.00:00:00"));

            ServiceLocator.Resolve<ISchedulingScriptSettings>().EnableRestrictedMode = false;

            script.Create("test script");
        }

        [Fact, Owner(@"FIRM\MaximL")]
        public void LaunchWithSecurity_UnsecureScriptWithCustomFilter_ExceptionIsThrown()
        {
            var script = new TestScript(
                new[]
                {
                    new Action(Action.Operation.SetNewITS, "10", "System.IO.File.Open('c:\test.txt') == null")
                },
                new Shift(1, 1, "0.00:00:00", "0.00:00:00"));

            ServiceLocator.Resolve<ISchedulingScriptSettings>().EnableRestrictedMode = true;

            var exception = Xunit.Assert.Throws<UserMessageException>(() => script.Create("test script"));

            Xunit.Assert.True(exception.Message.Contains(
                "System.IO.FileStream System.IO.File::Open(System.String,System.IO.FileMode)"));
        }

        [Fact, Owner(@"FIRM\MaximL")]
        public void LaunchWithSecurity_UnsecureScriptWithCustomFilterButUnsecureMethodIsAddedToWhiteList_ExceptionIsNotThrown()
        {
            var script = new TestScript(
                new[]
                {
                    new Action(Action.Operation.SetNewITS, "10", "System.IO.File.Open('c:\test.txt') == null")
                },
                new Shift(1, 1, "0.00:00:00", "0.00:00:00"));

            ServiceLocator.Resolve<ISchedulingScriptSettings>().EnableRestrictedMode = true;
            ServiceLocator.Resolve<ISchedulingScriptSettings>().SecureExternalMethods = @"System.IO.FileStream System.IO.File::Open(System.String,System.IO.FileMode);System.IO.FileStream System.IO.File::Create(System.String,System.IO.FileMode)";
            ServiceLocator.Resolve<ISystemSettingCache>().Reset();

            script.Create("test script");
        }
    }
}
