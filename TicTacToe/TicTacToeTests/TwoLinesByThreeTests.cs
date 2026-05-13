using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacToe;

namespace TicTacToeTests
{
    [TestClass]
    public class TwoLinesByThreeTests : BaseTest
    {
        [TestMethod]
        public void TwoDiagonals_2InLineAnd1And1InLine_TopIsWinAttack()
        {
            const string position = "0_168_21210_16_12020_21_10_186_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(7, step[0]);
            Assert.AreEqual(10, step[1]);
        }

        [TestMethod]
        public void TwoDiagonals_2InLineAnd1And1InLine_MiddleIsWinAttack()
        {
            const string position = "0_129_10_38_21210_16_12020_208_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(7, step[0]);
            Assert.AreEqual(10, step[1]);
        }
        
    }
}
