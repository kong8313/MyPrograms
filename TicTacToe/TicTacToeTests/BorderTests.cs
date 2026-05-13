using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacToe;

namespace TicTacToeTests
{
    [TestClass]
    public class BorderTests : BaseTest
    {
        [TestMethod]
        public void ObjectsInAllCorners_NoException()
        {
            const string position = "10_18_1020_16_20_322_20_16_2010_18_1";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
        }
        
        [TestMethod]
        public void ObjectsNearAllCorners_NoException()
        {
            const string position = "010_16_10120_16_210_320_120_16_21010_16_10";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
        }
    }
}
