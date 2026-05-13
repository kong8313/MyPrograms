using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacToe;

namespace TicTacToeTests.InvestigateFieldClassTests
{
    [TestClass]
    public class AttackLeftBottomRightTopDiagTests : BaseTest
    {
        [TestMethod]
        public void Line_o___xx___o_4AttackSteps()
        {
            const string position = "0_113_20_75_10_18_10_75_20_115_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(4, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(11, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
            Assert.AreEqual(11, steps[2][0]);
            Assert.AreEqual(7, steps[2][1]);
            Assert.AreEqual(12, steps[3][0]);
            Assert.AreEqual(6, steps[3][1]);
        }

        [TestMethod]
        public void Line_o__xx___o_3AttackSteps()
        {
            const string position = "0_113_20_75_10_18_10_56_20_134_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(11, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
            Assert.AreEqual(11, steps[2][0]);
            Assert.AreEqual(7, steps[2][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_b__xx___o_3AttackSteps()
        {
            const string position = "0_207_20_75_10_18_10_97_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(12, steps[0][0]);
            Assert.AreEqual(5, steps[0][1]);
            Assert.AreEqual(13, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
            Assert.AreEqual(16, steps[2][0]);
            Assert.AreEqual(1, steps[2][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_b__xx___o_3AttackSteps()
        {
            const string position = "0_249_20_75_10_18_10_55_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(14, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(15, steps[1][0]);
            Assert.AreEqual(6, steps[1][1]);
            Assert.AreEqual(18, steps[2][0]);
            Assert.AreEqual(3, steps[2][1]);
        }

        [TestMethod]
        public void Line_o___xx__o_3AttackSteps()
        {
            const string position = "0_132_20_56_10_18_10_75_20_115_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(8, steps[0][0]);
            Assert.AreEqual(10, steps[0][1]);
            Assert.AreEqual(11, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
            Assert.AreEqual(12, steps[2][0]);
            Assert.AreEqual(6, steps[2][1]);
        }

        [TestMethod]
        public void LineFromTopSide_o___xx__b_3AttackSteps()
        {
            const string position = "0_55_10_18_10_75_20_249_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(1, steps[0][0]);
            Assert.AreEqual(16, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(13, steps[1][1]);
            Assert.AreEqual(5, steps[2][0]);
            Assert.AreEqual(12, steps[2][1]);
        }

        [TestMethod]
        public void LineFromRightSide_o___xx__b_3AttackSteps()
        {
            const string position = "0_97_10_18_10_75_20_207_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(3, steps[0][0]);
            Assert.AreEqual(18, steps[0][1]);
            Assert.AreEqual(6, steps[1][0]);
            Assert.AreEqual(15, steps[1][1]);
            Assert.AreEqual(7, steps[2][0]);
            Assert.AreEqual(14, steps[2][1]);
        }

        [TestMethod]
        public void Line_o_xx___o_2AttackSteps()
        {
            const string position = "0_113_20_75_10_18_10_37_20_153_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(11, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_b_xx___o_2AttackSteps()
        {
            const string position = "0_226_20_75_10_18_10_78_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(13, steps[0][0]);
            Assert.AreEqual(4, steps[0][1]);
            Assert.AreEqual(14, steps[1][0]);
            Assert.AreEqual(3, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_b_xx___o_2AttackSteps()
        {
            const string position = "0_268_20_75_10_18_10_36_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(16, steps[1][0]);
            Assert.AreEqual(5, steps[1][1]);
        }

        [TestMethod]
        public void Line_o___xx_o_2AttackSteps()
        {
            const string position = "0_151_20_37_10_18_10_75_20_115_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(11, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(12, steps[1][0]);
            Assert.AreEqual(6, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_o___xx_b_2AttackSteps()
        {
            const string position = "0_36_10_18_10_75_20_268_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(3, steps[0][0]);
            Assert.AreEqual(14, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(13, steps[1][1]);
        }

        [TestMethod]
        public void LineFromRightSide_o___xx_b_2AttackSteps()
        {
            const string position = "0_78_10_18_10_75_20_226_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(5, steps[0][0]);
            Assert.AreEqual(16, steps[0][1]);
            Assert.AreEqual(6, steps[1][0]);
            Assert.AreEqual(15, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__xx__o_2AttackSteps()
        {
            const string position = "0_132_20_56_10_18_10_56_20_134_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(8, steps[0][0]);
            Assert.AreEqual(10, steps[0][1]);
            Assert.AreEqual(11, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_b__xx__o_2AttackSteps()
        {
            const string position = "0_226_20_56_10_18_10_97_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(13, steps[0][0]);
            Assert.AreEqual(4, steps[0][1]);
            Assert.AreEqual(16, steps[1][0]);
            Assert.AreEqual(1, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_b__xx__o_2AttackSteps()
        {
            const string position = "0_268_20_56_10_18_10_55_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(18, steps[1][0]);
            Assert.AreEqual(3, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_o__xx__b_2AttackSteps()
        {
            const string position = "0_55_10_18_10_56_20_268_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(1, steps[0][0]);
            Assert.AreEqual(16, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(13, steps[1][1]);
        }

        [TestMethod]
        public void LineFromRightSide_o__xx__b_2AttackSteps()
        {
            const string position = "0_97_10_18_10_56_20_226_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(3, steps[0][0]);
            Assert.AreEqual(18, steps[0][1]);
            Assert.AreEqual(6, steps[1][0]);
            Assert.AreEqual(15, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_x_x__o_2AttackSteps()
        {
            const string position = "0_113_20_56_10_37_10_37_20_153_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(11, steps[0][1]);
            Assert.AreEqual(9, steps[1][0]);
            Assert.AreEqual(9, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_b_x_x__o_2AttackSteps()
        {
            const string position = "0_226_20_56_10_37_10_78_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(13, steps[0][0]);
            Assert.AreEqual(4, steps[0][1]);
            Assert.AreEqual(15, steps[1][0]);
            Assert.AreEqual(2, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_b_x_x__o_2AttackSteps()
        {
            const string position = "0_268_20_56_10_37_10_36_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(17, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__x_x_o_2AttackSteps()
        {
            const string position = "0_132_20_37_10_37_10_56_20_134_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(9, steps[0][0]);
            Assert.AreEqual(9, steps[0][1]);
            Assert.AreEqual(11, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_o__x_x_b_2AttackSteps()
        {
            const string position = "0_36_10_37_10_56_20_268_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(2, steps[0][0]);
            Assert.AreEqual(15, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(13, steps[1][1]);
        }

        [TestMethod]
        public void LineFromRightSide_o__x_x_b_2AttackSteps()
        {
            const string position = "0_78_10_37_10_56_20_226_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(4, steps[0][0]);
            Assert.AreEqual(17, steps[0][1]);
            Assert.AreEqual(6, steps[1][0]);
            Assert.AreEqual(15, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__x_x__o_3AttackSteps()
        {
            const string position = "0_113_20_56_10_37_10_56_20_134_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(11, steps[0][1]);
            Assert.AreEqual(9, steps[1][0]);
            Assert.AreEqual(9, steps[1][1]);
            Assert.AreEqual(11, steps[2][0]);
            Assert.AreEqual(7, steps[2][1]);
        }

        [TestMethod]
        public void Line_oxxx__o_2AttackSteps()
        {
            const string position = "0_113_20_56_10_18_10_18_10_18_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(12, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(11, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_bxxx__o_2AttackSteps()
        {
            const string position = "0_245_20_56_10_18_10_18_10_59_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(13, steps[0][0]);
            Assert.AreEqual(4, steps[0][1]);
            Assert.AreEqual(14, steps[1][0]);
            Assert.AreEqual(3, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_bxxx__o_2AttackSteps()
        {
            const string position = "0_287_20_56_10_18_10_18_10_17_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(16, steps[1][0]);
            Assert.AreEqual(5, steps[1][1]);
        }

        [TestMethod]
        public void Line_oxx_x_o_2AttackSteps()
        {
            const string position = "0_113_20_37_10_37_10_18_10_18_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(12, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_bxx_x_o_2AttackSteps()
        {
            const string position = "0_245_20_37_10_37_10_18_10_59_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(13, steps[0][0]);
            Assert.AreEqual(4, steps[0][1]);
            Assert.AreEqual(15, steps[1][0]);
            Assert.AreEqual(2, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_bxx_x_o_2AttackSteps()
        {
            const string position = "0_287_20_37_10_37_10_18_10_17_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(17, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
        }

        [TestMethod]
        public void Line_oxx__xo_2AttackSteps()
        {
            const string position = "0_113_20_18_10_56_10_18_10_18_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(11, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_bxx__xo_2AttackSteps()
        {
            const string position = "0_245_20_18_10_56_10_18_10_59_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(14, steps[0][0]);
            Assert.AreEqual(3, steps[0][1]);
            Assert.AreEqual(15, steps[1][0]);
            Assert.AreEqual(2, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_bxx__xo_2AttackSteps()
        {
            const string position = "0_287_20_18_10_56_10_18_10_17_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(16, steps[0][0]);
            Assert.AreEqual(5, steps[0][1]);
            Assert.AreEqual(17, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
        }

        [TestMethod]
        public void Line_ox_xx_o_2AttackSteps()
        {
            const string position = "0_113_20_37_10_18_10_37_10_18_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(12, steps[0][1]);
            Assert.AreEqual(9, steps[1][0]);
            Assert.AreEqual(9, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_bx_xx_o_2AttackSteps()
        {
            const string position = "0_245_20_37_10_18_10_37_10_59_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(13, steps[0][0]);
            Assert.AreEqual(4, steps[0][1]);
            Assert.AreEqual(16, steps[1][0]);
            Assert.AreEqual(1, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_bx_xx_o_2AttackSteps()
        {
            const string position = "0_287_20_37_10_18_10_37_10_17_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(18, steps[1][0]);
            Assert.AreEqual(3, steps[1][1]);
        }

        [TestMethod]
        public void Line_ox_x_xo_2AttackSteps()
        {
            const string position = "0_113_20_18_10_37_10_37_10_18_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(11, steps[0][1]);
            Assert.AreEqual(9, steps[1][0]);
            Assert.AreEqual(9, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_bx_x_xo_2AttackSteps()
        {
            const string position = "0_245_20_18_10_37_10_37_10_59_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(14, steps[0][0]);
            Assert.AreEqual(3, steps[0][1]);
            Assert.AreEqual(16, steps[1][0]);
            Assert.AreEqual(1, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_bx_x_xo_2AttackSteps()
        {
            const string position = "0_287_20_18_10_37_10_37_10_17_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(16, steps[0][0]);
            Assert.AreEqual(5, steps[0][1]);
            Assert.AreEqual(18, steps[1][0]);
            Assert.AreEqual(3, steps[1][1]);
        }

        [TestMethod]
        public void Line_ox__xxo_2AttackSteps()
        {
            const string position = "0_113_20_18_10_18_10_56_10_18_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(8, steps[0][0]);
            Assert.AreEqual(10, steps[0][1]);
            Assert.AreEqual(9, steps[1][0]);
            Assert.AreEqual(9, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_bx__xxo_2AttackSteps()
        {
            const string position = "0_245_20_18_10_18_10_56_10_59_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(2, steps[0][1]);
            Assert.AreEqual(16, steps[1][0]);
            Assert.AreEqual(1, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_bx__xxo_2AttackSteps()
        {
            const string position = "0_287_20_18_10_18_10_56_10_17_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(17, steps[0][0]);
            Assert.AreEqual(4, steps[0][1]);
            Assert.AreEqual(18, steps[1][0]);
            Assert.AreEqual(3, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_xxx_o_2AttackSteps()
        {
            const string position = "0_113_20_37_10_18_10_18_10_37_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(12, steps[0][1]);
            Assert.AreEqual(10, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
        }

        [TestMethod]
        public void LineFromLeftSide_b_xxx_o_2AttackSteps()
        {
            const string position = "0_245_20_37_10_18_10_18_10_78_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(13, steps[0][0]);
            Assert.AreEqual(4, steps[0][1]);
            Assert.AreEqual(17, steps[1][0]);
            Assert.AreEqual(0, steps[1][1]);
        }

        [TestMethod]
        public void LineFromBottomSide_b_xxx_o_2AttackSteps()
        {
            const string position = "0_287_20_37_10_18_10_18_10_36_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(19, steps[1][0]);
            Assert.AreEqual(2, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_o_xxx_b_2AttackSteps()
        {
            const string position = "0_36_10_18_10_18_10_37_20_287_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(17, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(13, steps[1][1]);
        }

        [TestMethod]
        public void LineFromRightSide_o_xxx_b_2AttackSteps()
        {
            const string position = "0_78_10_18_10_18_10_37_20_245_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(2, steps[0][0]);
            Assert.AreEqual(19, steps[0][1]);
            Assert.AreEqual(6, steps[1][0]);
            Assert.AreEqual(15, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_xx_xo_2AttackSteps()
        {
            const string position = "0_113_20_18_10_37_10_18_10_37_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(11, steps[0][1]);
            Assert.AreEqual(10, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_o_xx_xb_2AttackSteps()
        {
            const string position = "0_17_10_37_10_18_10_37_20_287_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(1, steps[0][0]);
            Assert.AreEqual(16, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(13, steps[1][1]);
        }

        [TestMethod]
        public void LineFromRightSide_o_xx_xb_2AttackSteps()
        {
            const string position = "0_59_10_37_10_18_10_37_20_245_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(3, steps[0][0]);
            Assert.AreEqual(18, steps[0][1]);
            Assert.AreEqual(6, steps[1][0]);
            Assert.AreEqual(15, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_x_xxo_2AttackSteps()
        {
            const string position = "0_113_20_18_10_18_10_37_10_37_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(8, steps[0][0]);
            Assert.AreEqual(10, steps[0][1]);
            Assert.AreEqual(10, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_o_x_xxb_2AttackSteps()
        {
            const string position = "0_17_10_18_10_37_10_37_20_287_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(2, steps[0][0]);
            Assert.AreEqual(15, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(13, steps[1][1]);
        }

        [TestMethod]
        public void LineFromRightSide_o_x_xxb_2AttackSteps()
        {
            const string position = "0_59_10_18_10_37_10_37_20_245_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(4, steps[0][0]);
            Assert.AreEqual(17, steps[0][1]);
            Assert.AreEqual(6, steps[1][0]);
            Assert.AreEqual(15, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__xxxo_2AttackSteps()
        {
            const string position = "0_113_20_18_10_18_10_18_10_56_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(9, steps[0][0]);
            Assert.AreEqual(9, steps[0][1]);
            Assert.AreEqual(10, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
        }

        [TestMethod]
        public void LineFromTopSide_o__xxxb_2AttackSteps()
        {
            const string position = "0_17_10_18_10_18_10_56_20_287_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(3, steps[0][0]);
            Assert.AreEqual(14, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(13, steps[1][1]);
        }

        [TestMethod]
        public void LineFromRightSide_o__xxxb_2AttackSteps()
        {
            const string position = "0_59_10_18_10_18_10_56_20_245_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(5, steps[0][0]);
            Assert.AreEqual(16, steps[0][1]);
            Assert.AreEqual(6, steps[1][0]);
            Assert.AreEqual(15, steps[1][1]);
        }
    }
}
