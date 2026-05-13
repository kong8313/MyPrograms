using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacToe;

namespace TicTacToeTests
{
    [TestClass]
    public class AttackTests : BaseTest
    {
        [TestMethod]
        public void TreeDiagonals_WinIn2Step_MiddleIsWinAttack_Ignore2CrossDefence()
        {
            const string position = "0_154_10_13_21210120_13_120200020_204_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(9, step[0]);
            Assert.AreEqual(12, step[1]);
        }

        [TestMethod]
        public void TreeDiagonals_WinIn2Step_MiddleIsWinAttack()
        {
            const string position = "0_154_10_13_212101220_12_12020_208_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Cross);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(9, step[0]);
            Assert.AreEqual(12, step[1]);
        }
        

        [TestMethod]
        public void TreeDiagonals_WinIn2Step_BottomMiddleIsWinAttack()
        {
            const string position = "0_132_12020_15_20010_12_2212121220_12_10_211_";
            var filed = Converter.TextToField(position, RowsCnt, ColumnsCnt);
            int[] step = ComputerClass.DoStep(filed, ObjectType.Nil);

            Assert.AreEqual(2, step.Length);
            Assert.AreEqual(6, step[0]);
            Assert.AreEqual(13, step[1]);
        }
    }
}
