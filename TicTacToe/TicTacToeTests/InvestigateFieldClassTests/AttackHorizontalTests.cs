using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacToe;

namespace TicTacToeTests.InvestigateFieldClassTests
{
    [TestClass]
    public class AttackHorizontalTests : BaseTest
    {
        [TestMethod]
        public void Line_o___xx___o_4AttackSteps()
        {
            const string position = "0_145_20001100020_14_20_230_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(4, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
            Assert.AreEqual(7, steps[2][0]);
            Assert.AreEqual(11, steps[2][1]);
            Assert.AreEqual(7, steps[3][0]);
            Assert.AreEqual(12, steps[3][1]);
        }

        [TestMethod]
        public void Line_o__xx___o_3AttackSteps()
        {
            const string position = "0_146_2001100020_14_20_230_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(8, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(11, steps[1][1]);
            Assert.AreEqual(7, steps[2][0]);
            Assert.AreEqual(12, steps[2][1]);
        }

        [TestMethod]
        public void Line_o___xx__o_3AttackSteps()
        {
            const string position = "0_145_2000110020_15_20_230_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
            Assert.AreEqual(7, steps[2][0]);
            Assert.AreEqual(11, steps[2][1]);
        }

        [TestMethod]
        public void Line_o_xx___o_2AttackSteps()
        {
            const string position = "0_147_201100020_14_20_230_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(11, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(12, steps[1][1]);
        }

        [TestMethod]
        public void Line_o___xx_o_2AttackSteps()
        {
            const string position = "0_145_200011020_16_20_230_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__xx__o_2AttackSteps()
        {
            const string position = "0_145_200110020_247_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
        }


        [TestMethod]
        public void Line_b_xx___o_2AttackSteps()
        {
            const string position = "01100020_393_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(3, steps[0][1]);
            Assert.AreEqual(0, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
        }

        [TestMethod]
        public void Line_o___xx_b_2AttackSteps()
        {
            const string position = "0_13_2000110_381_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(15, steps[0][1]);
            Assert.AreEqual(0, steps[1][0]);
            Assert.AreEqual(16, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_x_x__o_2AttackSteps()
        {
            const string position = "0_145_201010020_247_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(8, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__x_x_o_2AttackSteps()
        {
            const string position = "0_144_200101020_248_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__x_x__o_3AttackSteps()
        {
            const string position = "0_144_2001010020_247_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
            Assert.AreEqual(7, steps[2][0]);
            Assert.AreEqual(10, steps[2][1]);
        }

        [TestMethod]
        public void Line_b_x_x__o_2AttackSteps()
        {
            const string position = "01010020_393_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(2, steps[0][1]);
            Assert.AreEqual(0, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__x_x_b_2AttackSteps()
        {
            const string position = "0_13_2001010_381_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(15, steps[0][1]);
            Assert.AreEqual(0, steps[1][0]);
            Assert.AreEqual(17, steps[1][1]);
        }

        [TestMethod]
        public void Line_oxxx__o_2AttackSteps()
        {
            const string position = "0_145_21110020_248_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(9, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
        }

        [TestMethod]
        public void Line_bxxx__o_2AttackSteps()
        {
            const string position = "1110020_394_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(3, steps[0][1]);
            Assert.AreEqual(0, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
        }

        [TestMethod]
        public void Line_oxx_x_o_2AttackSteps()
        {
            const string position = "0_145_21101020_248_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(8, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
        }

        [TestMethod]
        public void Line_bxx_x_o_2AttackSteps()
        {
            const string position = "1101020_394_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(2, steps[0][1]);
            Assert.AreEqual(0, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
        }

        [TestMethod]
        public void Line_oxx__xo_2AttackSteps()
        {
            const string position = "0_145_21100120_248_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(8, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(9, steps[1][1]);
        }

        [TestMethod]
        public void Line_bxx__xo_2AttackSteps()
        {
            const string position = "1100120_394_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(2, steps[0][1]);
            Assert.AreEqual(0, steps[1][0]);
            Assert.AreEqual(3, steps[1][1]);
        }

        [TestMethod]
        public void Line_ox_xx_o_2AttackSteps()
        {
            const string position = "0_145_21011020_248_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
        }

        [TestMethod]
        public void Line_bx_xx_o_2AttackSteps()
        {
            const string position = "1011020_394_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(1, steps[0][1]);
            Assert.AreEqual(0, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
        }

        [TestMethod]
        public void Line_ox_x_xo_2AttackSteps()
        {
            const string position = "0_145_21010120_248_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(9, steps[1][1]);
        }

        [TestMethod]
        public void Line_bx_x_xo_2AttackSteps()
        {
            const string position = "1010120_394_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(1, steps[0][1]);
            Assert.AreEqual(0, steps[1][0]);
            Assert.AreEqual(3, steps[1][1]);
        }

        [TestMethod]
        public void Line_ox__xxo_2AttackSteps()
        {
            const string position = "0_145_21001120_248_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(7, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
        }

        [TestMethod]
        public void Line_bx__xxo_2AttackSteps()
        {
            const string position = "1001120_394_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(1, steps[0][1]);
            Assert.AreEqual(0, steps[1][0]);
            Assert.AreEqual(2, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_xxx_o_2AttackSteps()
        {
            const string position = "0_145_20111020_248_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(10, steps[1][1]);
        }

        [TestMethod]
        public void Line_b_xxx_o_2AttackSteps()
        {
            const string position = "0111020_394_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(0, steps[0][1]);
            Assert.AreEqual(0, steps[1][0]);
            Assert.AreEqual(4, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_xxx_b_2AttackSteps()
        {
            const string position = "0_14_201110_381_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(15, steps[0][1]);
            Assert.AreEqual(0, steps[1][0]);
            Assert.AreEqual(19, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_xx_xo_2AttackSteps()
        {
            const string position = "0_145_20110120_248_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(9, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_xx_xb_2AttackSteps()
        {
            const string position = "0_14_2011010_380_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(15, steps[0][1]);
            Assert.AreEqual(0, steps[1][0]);
            Assert.AreEqual(18, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_x_xxo_2AttackSteps()
        {
            const string position = "0_145_20101120_248_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(8, steps[1][1]);
        }

        [TestMethod]
        public void Line_o_x_xxb_2AttackSteps()
        {
            const string position = "0_14_2010110_380_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(15, steps[0][1]);
            Assert.AreEqual(0, steps[1][0]);
            Assert.AreEqual(17, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__xxxo_2AttackSteps()
        {
            const string position = "0_145_20011120_248_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(7, steps[0][0]);
            Assert.AreEqual(6, steps[0][1]);
            Assert.AreEqual(7, steps[1][0]);
            Assert.AreEqual(7, steps[1][1]);
        }

        [TestMethod]
        public void Line_o__xxxb_2AttackSteps()
        {
            const string position = "0_14_2001110_380_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            List<int[]> steps = InvestigateFieldClassNew.FindThreeInLineAttackSteps(filed, ObjectType.Cross);

            Assert.AreEqual(2, steps.Count);
            Assert.AreEqual(0, steps[0][0]);
            Assert.AreEqual(15, steps[0][1]);
            Assert.AreEqual(0, steps[1][0]);
            Assert.AreEqual(16, steps[1][1]);
        }
    }
}
