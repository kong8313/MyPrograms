using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacToe;

namespace TicTacToeTests.InvestigateFieldClassTests
{
    [TestClass]
    public class AttackLeftTopRightBottomDiagTests : BaseTest
    {
        [TestMethod]
        public void Line_o___xx___o_4AttackSteps()
        {
            const string position = "0_105_20_83_10_20_10_83_20_105_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(4, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
            Assert.AreEqual(11, steps[2][0]);
            Assert.AreEqual(11, steps[2][1]);
            Assert.AreEqual(12, steps[3][0]);
            Assert.AreEqual(12, steps[3][1]);
        }
        
        [TestMethod]
        public void Line_o__xx___o_3AttackSteps()
        {
            const string position = "0_126_20_62_10_20_10_83_20_105_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(8, steps[0][0]);
            Assert.AreEqual(8, steps[0][1]);
            Assert.AreEqual(11, steps[1][0]);
            Assert.AreEqual(11, steps[1][1]);
            Assert.AreEqual(12, steps[2][0]);
            Assert.AreEqual(12, steps[2][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_b__xx___o_3AttackSteps()
        {
            const string position = "0_102_10_20_10_83_20_192_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(4, steps[0][0]);
            Assert.AreEqual(1, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
            Assert.AreEqual(8, steps[2][0]);
            Assert.AreEqual(5, steps[2][1]);
        }

        [TestMethod]
        public void LineFromTopSide_b__xx___o_3AttackSteps()
        {
            const string position = "0_45_10_20_10_83_20_249_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(1, steps[0][0]);
            Assert.AreEqual(4, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
            Assert.AreEqual(5, steps[2][0]);
            Assert.AreEqual(8, steps[2][1]);
        }

        [TestMethod]
        public void Line_o___xx__o_3AttackSteps()
        {
            const string position = "0_105_20_83_10_20_10_62_20_126_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
            Assert.AreEqual(11, steps[2][0]);
            Assert.AreEqual(11, steps[2][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_o___xx__b_3AttackSteps()
        {
            const string position = "0_250_20_83_10_20_10_44_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(14, steps[0][0]);
            Assert.AreEqual(12, steps[0][1]);
            Assert.AreEqual(15, steps[1][0]);
            Assert.AreEqual(13, steps[1][1]);
            Assert.AreEqual(18, steps[2][0]);
            Assert.AreEqual(16, steps[2][1]);
        }

        [TestMethod]
        public void LineFromRightSide_o___xx__b_3AttackSteps()
        {
            const string position = "0_192_20_83_10_20_10_102_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(11, steps[0][0]);
            Assert.AreEqual(14, steps[0][1]);
            Assert.AreEqual(12, steps[1][0]);
            Assert.AreEqual(15, steps[1][1]);
            Assert.AreEqual(15, steps[2][0]);
            Assert.AreEqual(18, steps[2][1]);
        }

        [TestMethod]
        public void Line_o_xx___o_2AttackSteps()
        {
            const string position = "0_147_20_41_10_20_10_83_20_105_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(11, steps[0][0]);
            Assert.AreEqual(11, steps[0][1]);
            Assert.AreEqual(12, steps[1][0]);
            Assert.AreEqual(12, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_b_xx___o_2AttackSteps()
        {
            const string position = "0_81_10_20_10_83_20_213_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(3, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_b_xx___o_2AttackSteps()
        {
            const string position = "0_24_10_20_10_83_20_270_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(3, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_o___xx_o_2AttackSteps()
        {
            const string position = "0_105_20_83_10_20_10_41_20_147_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_o___xx_b_2AttackSteps()
        {
            const string position = "0_271_20_83_10_20_10_23_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(13, steps[0][1]);
            Assert.AreEqual(16, steps[1][0]);
            Assert.AreEqual(14, steps[1][1]);
        }

        [TestMethod]
        public void LineFromRightSide_o___xx_b_2AttackSteps()
        {
            const string position = "0_213_20_83_10_20_10_81_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(12, steps[0][0]);
            Assert.AreEqual(15, steps[0][1]);
            Assert.AreEqual(13, steps[1][0]);
            Assert.AreEqual(16, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__xx__o_2AttackSteps()
        {
            const string position = "0_105_20_62_10_20_10_62_20_147_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(10, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_b__xx__o_2AttackSteps()
        {
            const string position = "0_102_10_20_10_62_20_213_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(4, steps[0][0]);
            Assert.AreEqual(1, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_b__xx__o_2AttackSteps()
        {
            const string position = "0_45_10_20_10_62_20_270_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(1, steps[0][0]);
            Assert.AreEqual(4, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_o__xx__b_2AttackSteps()
        {
            const string position = "0_271_20_62_10_20_10_44_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(13, steps[0][1]);
            Assert.AreEqual(18, steps[1][0]);
            Assert.AreEqual(16, steps[1][1]);
        }

        [TestMethod]
        public void LineFromRightSide_o__xx__b_2AttackSteps()
        {
            const string position = "0_213_20_62_10_20_10_102_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(12, steps[0][0]);
            Assert.AreEqual(15, steps[0][1]);
            Assert.AreEqual(15, steps[1][0]);
            Assert.AreEqual(18, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_x_x__o_2AttackSteps()
        {
            const string position = "0_105_20_41_10_41_10_62_20_147_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(8, steps[0][0]);
            Assert.AreEqual(8, steps[0][1]);
            Assert.AreEqual(10, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_b_x_x__o_2AttackSteps()
        {
            const string position = "0_81_10_41_10_62_20_213_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(5, steps[0][0]);
            Assert.AreEqual(2, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_b_x_x__o_2AttackSteps()
        {
            const string position = "0_24_10_41_10_62_20_270_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(2, steps[0][0]);
            Assert.AreEqual(5, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__x_x_o_2AttackSteps()
        {
            const string position = "0_84_20_62_10_41_10_41_20_168_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_o__x_x_b_2AttackSteps()
        {
            const string position = "0_270_20_62_10_41_10_24_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(12, steps[0][1]);
            Assert.AreEqual(17, steps[1][0]);
            Assert.AreEqual(14, steps[1][1]);
        }

        [TestMethod]
        public void LineFromRightSide_o__x_x_b_2AttackSteps()
        {
            const string position = "0_233_20_62_10_41_10_61_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(13, steps[0][0]);
            Assert.AreEqual(15, steps[0][1]);
            Assert.AreEqual(15, steps[1][0]);
            Assert.AreEqual(17, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__x_x__o_3AttackSteps()
        {
            const string position = "0_84_20_62_10_41_10_62_20_147_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
            Assert.AreEqual(10, steps[2][0]);
            Assert.AreEqual(10, steps[2][1]);
        }

        [TestMethod]
        public void Line_oxxx__o_2AttackSteps()
        {
            const string position = "0_105_20_20_10_20_10_20_10_62_20_168_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(9, steps[0][0]);
            Assert.AreEqual(9, steps[0][1]);
            Assert.AreEqual(10, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_bxxx__o_2AttackSteps()
        {
            const string position = "00010_20_10_20_10_62_20_291_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(3, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_bxxx__o_2AttackSteps()
        {
            const string position = "0_60_10_20_10_20_10_62_20_234_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(3, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
        }

        [TestMethod]
        public void Line_oxx_x_o_2AttackSteps()
        {
            const string position = "0_105_20_20_10_20_10_41_10_41_20_168_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(8, steps[0][0]);
            Assert.AreEqual(8, steps[0][1]);
            Assert.AreEqual(10, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_bxx_x_o_2AttackSteps()
        {
            const string position = "00010_20_10_41_10_41_20_291_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(2, steps[0][0]);
            Assert.AreEqual(5, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_bxx_x_o_2AttackSteps()
        {
            const string position = "0_60_10_20_10_41_10_41_20_234_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(5, steps[0][0]);
            Assert.AreEqual(2, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
        }

        [TestMethod]
        public void Line_oxx__xo_2AttackSteps()
        {
            const string position = "0_105_20_20_10_20_10_62_10_20_20_168_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(8, steps[0][0]);
            Assert.AreEqual(8, steps[0][1]);
            Assert.AreEqual(9, steps[1][0]);
            Assert.AreEqual(9, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_bxx__xo_2AttackSteps()
        {
            const string position = "00010_20_10_62_10_20_20_291_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(2, steps[0][0]);
            Assert.AreEqual(5, steps[0][1]);
            Assert.AreEqual(3, steps[1][0]);
            Assert.AreEqual(6, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_bxx__xo_2AttackSteps()
        {
            const string position = "0_60_10_20_10_62_10_20_20_234_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(5, steps[0][0]);
            Assert.AreEqual(2, steps[0][1]);
            Assert.AreEqual(6, steps[1][0]);
            Assert.AreEqual(3, steps[1][1]);
        }

        [TestMethod]
        public void Line_ox_xx_o_2AttackSteps()
        {
            const string position = "0_105_20_20_10_41_10_20_10_41_20_168_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(10, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_bx_xx_o_2AttackSteps()
        {
            const string position = "00010_41_10_20_10_41_20_291_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(1, steps[0][0]);
            Assert.AreEqual(4, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_bx_xx_o_2AttackSteps()
        {
            const string position = "0_60_10_41_10_20_10_41_20_234_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(4, steps[0][0]);
            Assert.AreEqual(1, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
        }

        [TestMethod]
        public void Line_ox_x_xo_2AttackSteps()
        {
            const string position = "0_105_20_20_10_41_10_41_10_20_20_168_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(9, steps[1][0]);
            Assert.AreEqual(9, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_bx_x_xo_2AttackSteps()
        {
            const string position = "00010_41_10_41_10_20_20_291_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(1, steps[0][0]);
            Assert.AreEqual(4, steps[0][1]);
            Assert.AreEqual(3, steps[1][0]);
            Assert.AreEqual(6, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_bx_x_xo_2AttackSteps()
        {
            const string position = "0_60_10_41_10_41_10_20_20_234_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(4, steps[0][0]);
            Assert.AreEqual(1, steps[0][1]);
            Assert.AreEqual(6, steps[1][0]);
            Assert.AreEqual(3, steps[1][1]);
        }

        [TestMethod]
        public void Line_ox__xxo_2AttackSteps()
        {
            const string position = "0_105_20_20_10_62_10_20_10_20_20_168_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_bx__xxo_2AttackSteps()
        {
            const string position = "00010_62_10_20_10_20_20_291_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(1, steps[0][0]);
            Assert.AreEqual(4, steps[0][1]);
            Assert.AreEqual(2, steps[1][0]);
            Assert.AreEqual(5, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_bx__xxo_2AttackSteps()
        {
            const string position = "0_60_10_62_10_20_10_20_20_234_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(4, steps[0][0]);
            Assert.AreEqual(1, steps[0][1]);
            Assert.AreEqual(5, steps[1][0]);
            Assert.AreEqual(2, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_xxx_o_2AttackSteps()
        {
            const string position = "0_105_20_41_10_20_10_20_10_41_20_168_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(10, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_b_xxx_o_2AttackSteps()
        {
            const string position = "0_24_10_20_10_20_10_41_20_291_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(3, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_b_xxx_o_2AttackSteps()
        {
            const string position = "0_81_10_20_10_20_10_41_20_234_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(3, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_o_xxx_b_2AttackSteps()
        {
            const string position = "0_291_20_41_10_20_10_20_10_24_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(12, steps[0][1]);
            Assert.AreEqual(19, steps[1][0]);
            Assert.AreEqual(16, steps[1][1]);
        }

        [TestMethod]
        public void LineFromRightSide_o_xxx_b_2AttackSteps()
        {
            const string position = "0_234_20_41_10_20_10_20_10_81_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(12, steps[0][0]);
            Assert.AreEqual(15, steps[0][1]);
            Assert.AreEqual(16, steps[1][0]);
            Assert.AreEqual(19, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_xx_xo_2AttackSteps()
        {
            const string position = "0_105_20_41_10_20_10_41_10_20_20_168_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(9, steps[1][0]);
            Assert.AreEqual(9, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_o_xx_xb_2AttackSteps()
        {
            const string position = "0_291_20_41_10_20_10_41_1000";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(12, steps[0][1]);
            Assert.AreEqual(18, steps[1][0]);
            Assert.AreEqual(15, steps[1][1]);
        }

        [TestMethod]
        public void LineFromRightSide_o_xx_xb_2AttackSteps()
        {
            const string position = "0_234_20_41_10_20_10_41_10_60_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(12, steps[0][0]);
            Assert.AreEqual(15, steps[0][1]);
            Assert.AreEqual(15, steps[1][0]);
            Assert.AreEqual(18, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_x_xxo_2AttackSteps()
        {
            const string position = "0_105_20_41_10_41_10_20_10_20_20_168_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_o_x_xxb_2AttackSteps()
        {
            const string position = "0_291_20_41_10_41_10_20_1000";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(12, steps[0][1]);
            Assert.AreEqual(17, steps[1][0]);
            Assert.AreEqual(14, steps[1][1]);
        }

        [TestMethod]
        public void LineFromRightSide_o_x_xxb_2AttackSteps()
        {
            const string position = "0_234_20_41_10_41_10_20_10_60_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(12, steps[0][0]);
            Assert.AreEqual(15, steps[0][1]);
            Assert.AreEqual(14, steps[1][0]);
            Assert.AreEqual(17, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__xxxo_2AttackSteps()
        {
            const string position = "0_105_20_62_10_20_10_20_10_20_20_168_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_o__xxxb_2AttackSteps()
        {
            const string position = "0_291_20_62_10_20_10_20_1000";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(12, steps[0][1]);
            Assert.AreEqual(16, steps[1][0]);
            Assert.AreEqual(13, steps[1][1]);
        }

        [TestMethod]
        public void LineFromRightSide_o__xxxb_2AttackSteps()
        {
            const string position = "0_234_20_62_10_20_10_20_10_60_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(12, steps[0][0]);
            Assert.AreEqual(15, steps[0][1]);
            Assert.AreEqual(13, steps[1][0]);
            Assert.AreEqual(16, steps[1][1]);
        }
    }
}
