using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacToe;

namespace TicTacToeTests.InvestigateFieldClassTests
{
    [TestClass]
    public class AttackVerticalTests : BaseTest
    {
        [TestMethod]
        public void Line_o___xx___o_4AttackSteps()
        {
            const string position = "0_107_20_79_10_19_10_79_20_112_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(4, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
            Assert.AreEqual(11, steps[2][0]);
            Assert.AreEqual(7, steps[2][1]);
            Assert.AreEqual(12, steps[3][0]);
            Assert.AreEqual(7, steps[3][1]);
        }

        [TestMethod]
        public void Line_o__xx___o_3AttackSteps()
        {
            const string position = "0_127_20_59_10_19_10_79_20_112_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(8, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(11, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
            Assert.AreEqual(12, steps[2][0]);
            Assert.AreEqual(7, steps[2][1]);
        }

        [TestMethod]
        public void Line_o___xx__o_3AttackSteps()
        {
            const string position = "0_107_20_79_10_19_10_59_20_132_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
            Assert.AreEqual(11, steps[2][0]);
            Assert.AreEqual(7, steps[2][1]);
        }

        [TestMethod]
        public void Line_o_xx___o_2AttackSteps()
        {
            const string position = "0_147_20_39_10_19_10_79_20_112_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(11, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(12, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_o___xx_o_2AttackSteps()
        {
            const string position = "0_107_20_79_10_19_10_39_20_152_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__xx__o_2AttackSteps()
        {
            const string position = "0_107_20_59_10_19_10_59_20_152_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(10, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }


        [TestMethod]
        public void Line_b_xx___o_2AttackSteps()
        {
            const string position = "0_20_10_19_10_79_20_279_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(3, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(0, steps[1][1]);
        }

        [TestMethod]
        public void Line_o___xx_b_2AttackSteps()
        {
            const string position = "0_260_20_79_10_19_10_39_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(16, steps[1][0]);
            Assert.AreEqual(0, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_x_x__o_2AttackSteps()
        {
            const string position = "0_107_20_39_10_39_10_59_20_152_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(8, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(10, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__x_x_o_2AttackSteps()
        {
            const string position = "0_87_20_59_10_39_10_39_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__x_x__o_3AttackSteps()
        {
            const string position = "0_87_20_59_10_39_10_59_20_152_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
            Assert.AreEqual(10, steps[2][0]);
            Assert.AreEqual(7, steps[2][1]);
        }

        [TestMethod]
        public void Line_b_x_x__o_2AttackSteps()
        {
            const string position = "0_20_10_39_10_59_20_279_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(2, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(0, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__x_x_b_2AttackSteps()
        {
            const string position = "0_260_20_59_10_39_10_39_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(17, steps[1][0]);
            Assert.AreEqual(0, steps[1][1]);
        }

        [TestMethod]
        public void Line_oxxx__o_2AttackSteps()
        {
            const string position = "0_107_20_19_10_19_10_19_10_59_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(9, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(10, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_bxxx__o_2AttackSteps()
        {
            const string position = "10_19_10_19_10_59_20_299_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(3, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(0, steps[1][1]);
        }

        [TestMethod]
        public void Line_oxx_x_o_2AttackSteps()
        {
            const string position = "0_107_20_19_10_19_10_39_10_39_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(8, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(10, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_bxx_x_o_2AttackSteps()
        {
            const string position = "10_19_10_39_10_39_20_299_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(2, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(0, steps[1][1]);
        }

        [TestMethod]
        public void Line_oxx__xo_2AttackSteps()
        {
            const string position = "0_107_20_19_10_19_10_59_10_19_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(8, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(9, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_bxx__xo_2AttackSteps()
        {
            const string position = "10_19_10_59_10_19_20_299_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(2, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(3, steps[1][0]);
            Assert.AreEqual(0, steps[1][1]);
        }

        [TestMethod]
        public void Line_ox_xx_o_2AttackSteps()
        {
            const string position = "0_107_20_19_10_39_10_19_10_39_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(10, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_bx_xx_o_2AttackSteps()
        {
            const string position = "10_39_10_19_10_39_20_299_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(1, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(0, steps[1][1]);
        }

        [TestMethod]
        public void Line_ox_x_xo_2AttackSteps()
        {
            const string position = "0_107_20_19_10_39_10_39_10_19_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(9, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_bx_x_xo_2AttackSteps()
        {
            const string position = "10_39_10_39_10_19_20_299_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(1, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(3, steps[1][0]);
            Assert.AreEqual(0, steps[1][1]);
        }

        [TestMethod]
        public void Line_ox__xxo_2AttackSteps()
        {
            const string position = "0_107_20_19_10_59_10_19_10_19_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_bx__xxo_2AttackSteps()
        {
            const string position = "10_59_10_19_10_19_20_299_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(1, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(2, steps[1][0]);
            Assert.AreEqual(0, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_xxx_o_2AttackSteps()
        {
            const string position = "0_107_20_39_10_19_10_19_10_39_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(10, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_b_xxx_o_2AttackSteps()
        {
            const string position = "0_20_10_19_10_19_10_39_20_299_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(4, steps[1][0]);
            Assert.AreEqual(0, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_xxx_b_2AttackSteps()
        {
            const string position = "0_280_20_39_10_19_10_19_10_39_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(19, steps[1][0]);
            Assert.AreEqual(0, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_xx_xo_2AttackSteps()
        {
            const string position = "0_107_20_39_10_19_10_39_10_19_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(9, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_xx_xb_2AttackSteps()
        {
            const string position = "0_280_20_39_10_19_10_39_10_19_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(18, steps[1][0]);
            Assert.AreEqual(0, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_x_xxo_2AttackSteps()
        {
            const string position = "0_107_20_39_10_39_10_19_10_19_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(8, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_x_xxb_2AttackSteps()
        {
            const string position = "0_280_20_39_10_39_10_19_10_19_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(17, steps[1][0]);
            Assert.AreEqual(0, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__xxxo_2AttackSteps()
        {
            const string position = "0_107_20_59_10_19_10_19_10_19_20_172_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(6, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__xxxb_2AttackSteps()
        {
            const string position = "0_280_20_59_10_19_10_19_10_19_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(15, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(16, steps[1][0]);
            Assert.AreEqual(0, steps[1][1]);
        }
    }
}
