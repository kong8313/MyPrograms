using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacToe;

namespace TicTacToeTests
{
    [TestClass]
    public class DefenseTests : BaseTest
    {
        [TestMethod]
        public void TwoDiagonals_2InLineAnd1And1InLine_MiddleIsCorrectDefense()
        {
            const string position = "0_129_10_38_21210_16_12020_208_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(7, step[0]);
            Assert.AreEqual(10, step[1]);
        }
        
        [TestMethod]
        public void TwoDiagonals_2InLineAnd3InLine_BottomMiddleIsCorrectDefense()
        {
            const string position = "0_132_1020_13_1200120_15_12120_227_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(9, step[0]);
            Assert.AreEqual(11, step[1]);
        }
        
        [TestMethod]
        public void TwoDiagonals_3InLineAnd2InLine_TopMiddleIsCorrectDefense()
        {
            const string position = "0_151_20010_13_21210120_13_120200020_204_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(6, step[0]);
            Assert.AreEqual(12, step[1]);
        }

        [TestMethod]
        public void ThreeDiagonals_WinIn2Step_BottomMiddleIsCorrectDefense()
        {
            const string position = "0_133_20_20_10_12_2212101220_12_10_211_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(6, step[0]);
            Assert.AreEqual(15, step[1]);
        }
    }
}
