using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacToe;

namespace TicTacToeTests
{
    [TestClass]
    public class WinFourInLineTests : BaseTest
    {
        [TestMethod]
        public void LeftBottomRightTopDiagonal_ThreeInLine_RightTopIsWinFour()
        {
            const string position = "0_111_10_17_210_18_120_36_20_212_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(4, step[0]);
            Assert.AreEqual(12, step[1]);
        }

        [TestMethod]
        public void LeftBottomRightTopDiagonal_TwoAndOneInLine_EmptyMiddleIsWinFour()
        {
            const string position = "0_92_10_36_210_18_120_36_20_212_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(5, step[0]);
            Assert.AreEqual(11, step[1]);
        }

        [TestMethod]
        public void LeftTopRightBottomDiagonal_ThreeInLine_RightBottomIsWinFour()
        {
            const string position = "0_87_20_41_120_18_210_20_10_228_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(9, step[0]);
            Assert.AreEqual(12, step[1]);
        }

        [TestMethod]
        public void LeftTopRightBottomDiagonal_TwoAndOneInLine_EmptyMiddleIsWinFour()
        {
            const string position = "0_87_20_41_120_18_210_41_10_207_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(8, step[0]);
            Assert.AreEqual(11, step[1]);
        }

        [TestMethod]
        public void Horizontal_ThreeInLine_LeftIsWinFour()
        {
            const string position = "0_132_20_17_111020_15_20_229_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(7, step[0]);
            Assert.AreEqual(9, step[1]);
        }

        [TestMethod]
        public void Horizontal_TwoAndOneInLine_EmptyMiddleIsWinFour()
        {
            const string position = "0_132_20_17_11010_16_200020_225_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(7, step[0]);
            Assert.AreEqual(12, step[1]);
        }

        [TestMethod]
        public void Vertical_ThreeInLine_TopIsWinFour()
        {
            const string position = "0_71_20_37_210_19_10_19_120_248_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(4, step[0]);
            Assert.AreEqual(10, step[1]);
        }

        [TestMethod]
        public void Vertical_TwoAndOneInLine_EmptyMiddleIsWinFour()
        {
            const string position = "0_71_20_18_10_18_20_20_10_19_120_248_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(5, step[0]);
            Assert.AreEqual(10, step[1]);
        }

        [TestMethod]
        public void Vertical_ThreeInLine_FirstLineIsWrongBecauseOfTopBorder_TopForSecondLineIsWinFour()
        {
            const string position = "10020_16_120_18_120_127_120_18_1020_17_120_189_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(7, step[0]);
            Assert.AreEqual(9, step[1]);
        }

        [TestMethod]
        public void LeftTopRightBottomDiagonal_ThreeInLine_FirstLineIsWrongBecauseOfTopLeftBorder_TopForSecondLineIsWinFour()
        {
            const string position = "120_19_120_19_10_19_20_106_120_18_1020_17_120_189_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(7, step[0]);
            Assert.AreEqual(9, step[1]);
        }

        [TestMethod]
        public void RightTopLeftBottomDiagonal_ThreeInLine_FirstLineIsWrongBecauseOfTopRightBorder_TopForSecondLineIsWinFour()
        {
            const string position = "0_18_210_17_210_17_210_111_120_18_1020_17_120_189_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(7, step[0]);
            Assert.AreEqual(9, step[1]);
        }

        [TestMethod]
        public void Horizontal_ThreeInLine_FirstLineIsWrongBecauseOfRightBorder_TopForSecondLineIsWinFour()
        {
            const string position = "0_17_1110_18_220_17_20_111_120_18_1020_17_120_189_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(7, step[0]);
            Assert.AreEqual(9, step[1]);
        }
    }
}
