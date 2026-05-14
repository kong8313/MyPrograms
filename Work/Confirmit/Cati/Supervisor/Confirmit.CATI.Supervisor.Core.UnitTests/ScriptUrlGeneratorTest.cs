using Confirmit.CATI.Supervisor.Core.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    [TestClass]
    public class ScriptUrlGeneratorTest
    {
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetResult_NoParameters_CorrectResult()
        {
            var generator = new ScriptUrlGenerator(@"http:\\localhost\test.aspx");

            string result = generator.GetResult();

            Assert.AreEqual(@"'http:\\localhost\test.aspx'", result);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetResult_StaticParameter_CorrectResult()
        {
            var generator = new ScriptUrlGenerator(@"http:\\localhost\test.aspx");

            generator.AddStaticParameter("p1", "25");

            string result = generator.GetResult();

            Assert.AreEqual(@"'http:\\localhost\test.aspx?p1=25'", result);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetResult_ScriptParameter_CorrectResult()
        {
            var generator = new ScriptUrlGenerator(@"http:\\localhost\test.aspx");

            generator.AddScriptParameter("p1", "GetP1()");

            string result = generator.GetResult();

            Assert.AreEqual(@"'http:\\localhost\test.aspx?p1=' + GetP1()", result);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetResult_ScriptAndStaticParameters_CorrectResult()
        {
            var generator = new ScriptUrlGenerator(@"http:\\localhost\test.aspx");

            generator.AddScriptParameter("p1", "GetP1()");

            generator.AddStaticParameter("p2", "50");

            string result = generator.GetResult();

            Assert.AreEqual(@"'http:\\localhost\test.aspx?p1=' + GetP1() + '&p2=50'", result);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetResult_StaticAndScriptParameters_CorrectResult()
        {
            var generator = new ScriptUrlGenerator(@"http:\\localhost\test.aspx");

            generator.AddStaticParameter("p1", "50");

            generator.AddScriptParameter("p2", "GetP2()");

            string result = generator.GetResult();

            Assert.AreEqual(@"'http:\\localhost\test.aspx?p1=50&p2=' + GetP2()", result);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetResult_2StaticParameters_CorrectResult()
        {
            var generator = new ScriptUrlGenerator(@"http:\\localhost\test.aspx");

            generator.AddStaticParameter("p1", "50");

            generator.AddStaticParameter("p2", "aaa");

            string result = generator.GetResult();

            Assert.AreEqual(@"'http:\\localhost\test.aspx?p1=50&p2=aaa'", result);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetResult_2ScriptParameters_CorrectResult()
        {
            var generator = new ScriptUrlGenerator(@"http:\\localhost\test.aspx");

            generator.AddScriptParameter("p1", "GetP1()");

            generator.AddScriptParameter("p2", "GetP2()");

            string result = generator.GetResult();

            Assert.AreEqual(@"'http:\\localhost\test.aspx?p1=' + GetP1() + '&p2=' + GetP2()", result);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetResult_BaseUrlAlreadyHasParameters_CorrectResult()
        {
            var generator = new ScriptUrlGenerator(@"http:\\localhost\test.aspx?p=1&p0=2");

            generator.AddStaticParameter("p1", "50");

            generator.AddScriptParameter("p2", "GetP2()");

            string result = generator.GetResult();

            Assert.AreEqual(@"'http:\\localhost\test.aspx?p=1&p0=2&p1=50&p2=' + GetP2()", result);
        }
    }
}