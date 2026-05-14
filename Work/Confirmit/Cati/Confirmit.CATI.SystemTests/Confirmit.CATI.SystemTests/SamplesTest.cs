using Confirmit.SystemTestFramework.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.SystemTests
{
    [TestClass]
    public class SamplesTest
    {
        [TestMethod, Owner(@"FIRM\KirillV")]
        public void SampleGenerator_Generate_1_TelephoneNumber()
        {
            var generator = new SampleGenerator();
            var expected = "TelephoneNumber\r\n1";

            var actual = generator.Generate(1, ColumnType.TelephoneNumber);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void SampleParser_Parse_1_TelephoneNumber()
        {
            var generator = new SampleGenerator();
            var sample = generator.Generate(1, ColumnType.TelephoneNumber);

            var parser = new SampleParser();
            parser.Parser(sample);
        }
    }
}
