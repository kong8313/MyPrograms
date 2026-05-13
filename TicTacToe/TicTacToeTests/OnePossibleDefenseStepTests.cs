using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacToe;

namespace TicTacToeTests
{
    [TestClass]
    public class OnePossibleDefenseStepTests : BaseTest
    {
        [TestMethod]
        public void FourInLine_LeftBottomIsDefense()
        {
            const string position = "0_130_10_19_210_16_2120_17_1210_16_10_17_210_19_20_17_20_135_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(12, step[0]);
            Assert.AreEqual(5, step[1]);
        }

        [TestMethod]
        public void FourInLine_RightTopIsDefense()
        {
            const string position = "0_148_20_19_2120_17_1210_16_10_18_10_18_20_154_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(7, step[0]);
            Assert.AreEqual(10, step[1]);
        }

        [TestMethod]
        public void FourInLine_LeftTopIsDefense()
        {
            const string position = "0_64_20_41_10_20_10_20_10_19_2120_19_20_209_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(4, step[0]);
            Assert.AreEqual(5, step[1]);
        }

        [TestMethod]
        public void FourInLine_RightBottomIsDefense()
        {
            const string position = "0_85_20_20_120_19_10_20_10_19_2120_229_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(9, step[0]);
            Assert.AreEqual(10, step[1]);
        }

        [TestMethod]
        public void FourInLine_LeftIsDefense()
        {
            const string position = "0_125_1_4_20_15_2220_252_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(6, step[0]);
            Assert.AreEqual(4, step[1]);
        }

        [TestMethod]
        public void FourInLine_RightIsDefense()
        {
            const string position = "0_124_21_4_0_17_2220_251_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(6, step[0]);
            Assert.AreEqual(9, step[1]);
        }

        [TestMethod]
        public void FourInLine_TopIsDefense()
        {
            const string position = "0_129_10_19_10_19_120_18_120_17_220_190_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(5, step[0]);
            Assert.AreEqual(9, step[1]);
        }

        [TestMethod]
        public void FourInLine_BottomIsDefense()
        {
            const string position = "0_109_20_19_10_19_10_19_120_18_120_17_20_191_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(10, step[0]);
            Assert.AreEqual(9, step[1]);
        }

        [TestMethod]
        public void Vertical_ThreeAndOneInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_109_20_19_10_39_120_18_120_17_210_19_20_170_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(7, step[0]);
            Assert.AreEqual(9, step[1]);
        }

        [TestMethod]
        public void Vertical_TwoAndTwoInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_109_20_19_10_19_10_20_20_18_120_17_210_19_20_170_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(8, step[0]);
            Assert.AreEqual(9, step[1]);
        }

        [TestMethod]
        public void Vertical_FourAndFourInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_89_20_19_10_19_120_18_120_18_120_38_10_19_120_18_120_18_120_18_20_110_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(9, step[0]);
            Assert.AreEqual(9, step[1]);
        }

        [TestMethod]
        public void Horizontal_ThreeAndOneInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_145_21011120_16_220_230_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(7, step[0]);
            Assert.AreEqual(7, step[1]);
        }

        [TestMethod]
        public void Horizontal_TwoAndTwoInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_145_21101120_16_220_230_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(7, step[0]);
            Assert.AreEqual(8, step[1]);
        }

        [TestMethod]
        public void Horizontal_FourAndFourInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_126_20_20_21_4_01_4_20_10_22200220_225_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(7, step[0]);
            Assert.AreEqual(12, step[1]);
        }


        [TestMethod]
        public void LeftBottomRightTopDiagonal_ThreeAndOneInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_131_20_18_10_17_210_18_120_36_10_18_20_154_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(10, step[0]);
            Assert.AreEqual(7, step[1]);
        }

        [TestMethod]
        public void LeftBottomRightTopDiagonal_TwoAndTwoInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_131_20_18_10_17_210_19_20_17_10_18_10_18_20_154_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(9, step[0]);
            Assert.AreEqual(8, step[1]);
        }

        [TestMethod]
        public void LeftBottomRightTopDiagonal_FourAndFourInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_112_20_18_120_17_1220_16_120_17_10_37_120_17_120_17_10_18_10_18_20_97_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(10, step[0]);
            Assert.AreEqual(7, step[1]);
        }

        [TestMethod]
        public void LeftTopRightBottomDiagonal_ThreeAndOneInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_106_20_20_10_19_210_20_10_18_20_22_10_20_20_167_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(9, step[0]);
            Assert.AreEqual(10, step[1]);
        }

        [TestMethod]
        public void LeftTopRightBottomDiagonal_TwoAndTwoInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_106_20_20_10_19_210_39_2010_20_10_20_20_167_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(8, step[0]);
            Assert.AreEqual(9, step[1]);
        }

        [TestMethod]
        public void LeftTopRightBottomDiagonal_FourAndFourInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_107_2020_18_1020_18_1020_18_10_20_10_22_2220_16_10_20_10_20_10_20_10_20_20_82_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(10, step[0]);
            Assert.AreEqual(12, step[1]);
        }

        [TestMethod]
        public void DiagonalFromLeftTopCorner_FourInLine_RightBottomIsDefense()
        {
            const string position = "120_18_210_19_210_20_10_41_20_294_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(4, step[0]);
            Assert.AreEqual(4, step[1]);
        }

        [TestMethod]
        public void DiagonalFromLeftBottomCorner_TwoAndWtoInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_285_20_18_10_18_120_15_20_20_10_18_120_18_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(17, step[0]);
            Assert.AreEqual(2, step[1]);
        }

        [TestMethod]
        public void DiagonalFromRightBottomCorner_ThreeAndOneInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_19_10_17_2120_17_10_19_20_17_10_18_20_285_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(3, step[0]);
            Assert.AreEqual(16, step[1]);
        }

        [TestMethod]
        public void DiagonalFromRightTopCorner_ThreeAndOneInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_294_20_20_120_19_10_20_10_21_20_18_21";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(18, step[0]);
            Assert.AreEqual(18, step[1]);
        }

        [TestMethod]
        public void HorizontalFromLeftTopCorner_FourInLine_RightIsDefense()
        {
            const string position = "1_4_0_16_2220020_374_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(0, step[0]);
            Assert.AreEqual(4, step[1]);
        }

        [TestMethod]
        public void HorizontalFromRightTopCorner_ThreeAndOneInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_14_2111010_16_20220_360_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(0, step[0]);
            Assert.AreEqual(18, step[1]);
        }

        [TestMethod]
        public void HorizontalFromLeftBottomCorner_TwoAndTwoInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_361_20220_15_1101120_14_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(19, step[0]);
            Assert.AreEqual(2, step[1]);
        }

        [TestMethod]
        public void HorizontalFromRightBottomCorner_TwoAndTwoInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_376_22020_14_211011";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(19, step[0]);
            Assert.AreEqual(17, step[1]);
        }

        [TestMethod]
        public void VerticalFromLeftTopCorner_TwoAndTwoInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_376_22020_14_211011";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(19, step[0]);
            Assert.AreEqual(17, step[1]);
        }

        [TestMethod]
        public void VerticalFromLeftBottomCorner_ThreeAndOneInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_280_20_19_10_20_20_18_1020_17_120_18_10_19_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(16, step[0]);
            Assert.AreEqual(0, step[1]);
        }

        [TestMethod]
        public void VerticalFromRightTopCorner_FourInLine_BottomIsDefense()
        {
            const string position = "0_19_10_18_210_17_2210_19_10_39_20_280_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(4, step[0]);
            Assert.AreEqual(19, step[1]);
        }

        [TestMethod]
        public void VerticalFromRightBottomCorner_ThreeAndOneInLine_EmptyMiddleIsDefense()
        {
            const string position = "0_318_120_17_10_20_120_19_20_18_12";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(16, step[0]);
            Assert.AreEqual(19, step[1]);
        }
    }
}