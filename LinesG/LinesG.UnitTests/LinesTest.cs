using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinesG.UnitTests
{
    [TestClass]
    public class LinesTest
    {
        private const int _redLine = 6;
        private readonly Lines _lines;

        public LinesTest()
        {
            _lines = new Lines();
        }

        private void CheckFiled(int checkValueCnt, int checkValue = _redLine + 20)
        {
            int cnt = 0;
            for (int i = 0; i < Consts.FieldSize; i++)
            {
                for (int j = 0; j < Consts.FieldSize; j++)
                {
                    if (_lines.Field[i, j] == checkValue)
                    {
                        cnt++;
                    }
                    else if (_lines.Field[i, j] != 0)
                    {
                        Assert.Fail($"Unexpected value '{_lines.Field[i, j]}' in field ({i}, {j})");
                    }
                }
            }

            Assert.AreEqual(checkValueCnt, cnt, "Extra cells are changed");
        }

        #region Horizontal
        [TestMethod]
        public void CleanLines_5Horizontal_CheckFromLeft_ScoreIs5()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 1; i < 6; i++)
                field[0, i] = _redLine;

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(0, 1));

            Assert.AreEqual(5, score);

            for (int i = 1; i < 6; i++)
                Assert.AreEqual(_redLine + 20, _lines.Field[0, i]);

            CheckFiled(5);
        }

        [TestMethod]
        public void CleanLines_6Horizontal_CheckFromMiddle_ScoreIs12()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 1; i < 7; i++)
                field[0, i] = _redLine;

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(0, 3));

            Assert.AreEqual(12, score);

            for (int i = 1; i < 7; i++)
                Assert.AreEqual(_redLine + 20, _lines.Field[0, i]);

            CheckFiled(6);
        }

        [TestMethod]
        public void CleanLines_7Horizontal_CheckFromRight_ScoreIs21()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 1; i < 8; i++)
                field[0, i] = _redLine;

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(0, 7));

            Assert.AreEqual(21, score);

            for (int i = 1; i < 8; i++)
                Assert.AreEqual(_redLine + 20, _lines.Field[0, i]);

            CheckFiled(7);
        }

        [TestMethod]
        public void CleanLines_4Horizontal_ScoreIs0()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 1; i < 5; i++)
                field[0, i] = _redLine;

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(0, 3));

            Assert.AreEqual(0, score);

            for (int i = 1; i < 5; i++)
                Assert.AreEqual(_redLine, _lines.Field[0, i]);

            CheckFiled(4, _redLine);
        }
        #endregion

        #region Vertical
        [TestMethod]
        public void CleanLines_8Vertical_CheckFromLeft_ScoreIs32()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 1; i < 9; i++)
                field[i, 0] = _redLine;

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(1, 0));

            Assert.AreEqual(32, score);

            for (int i = 1; i < 9; i++)
                Assert.AreEqual(_redLine + 20, _lines.Field[i, 0]);

            CheckFiled(8);
        }

        [TestMethod]
        public void CleanLines_9Vertical_CheckFromMiddle_ScoreIs45()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 0; i < 9; i++)
                field[i, 0] = _redLine;

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(4, 0));

            Assert.AreEqual(45, score);

            for (int i = 0; i < 9; i++)
                Assert.AreEqual(_redLine + 20, _lines.Field[i, 0]);

            CheckFiled(9);
        }

        [TestMethod]
        public void CleanLines_5Vertical_CheckFromRight_ScoreIs5()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 1; i < 6; i++)
                field[i, 0] = _redLine;

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(5, 0));

            Assert.AreEqual(5, score);

            for (int i = 1; i < 6; i++)
                Assert.AreEqual(_redLine + 20, _lines.Field[i, 0]);

            CheckFiled(5);
        }

        [TestMethod]
        public void CleanLines_4Vertical_ScoreIs0()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 1; i < 5; i++)
                field[i, 0] = _redLine;

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(4, 0));

            Assert.AreEqual(0, score);

            for (int i = 1; i < 5; i++)
                Assert.AreEqual(_redLine, _lines.Field[i, 0]);

            CheckFiled(4, _redLine);
        }
        #endregion

        #region LeftTopRightBottomDiagonal
        [TestMethod]
        public void CleanLines_9LTRBDiagonal_CheckFromLeft_ScoreIs45()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 0; i < 9; i++)
                field[i, i] = _redLine;

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(0, 0));

            Assert.AreEqual(45, score);

            for (int i = 0; i < 9; i++)
                Assert.AreEqual(_redLine + 20, _lines.Field[i, i]);

            CheckFiled(9);
        }

        [TestMethod]
        public void CleanLines_9LTRBDiagonal_CheckFromMiddle_ScoreIs45()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 0; i < 9; i++)
                field[i, i] = _redLine;

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(5, 5));

            Assert.AreEqual(45, score);

            for (int i = 0; i < 9; i++)
                Assert.AreEqual(_redLine + 20, _lines.Field[i, i]);

            CheckFiled(9);
        }

        [TestMethod]
        public void CleanLines_9LTRBDiagonal_CheckFromRight_ScoreIs45()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 0; i < 9; i++)
                field[i, i] = _redLine;

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(8, 8));

            Assert.AreEqual(45, score);

            for (int i = 0; i < 9; i++)
                Assert.AreEqual(_redLine + 20, _lines.Field[i, i]);

            CheckFiled(9);
        }

        [TestMethod]
        public void CleanLines_4LTRBDiagonal_ScoreIs0()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 1; i < 5; i++)
                field[i, i] = _redLine;

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(4, 4));

            Assert.AreEqual(0, score);

            for (int i = 1; i < 5; i++)
                Assert.AreEqual(_redLine, _lines.Field[i, i]);

            CheckFiled(4, _redLine);
        }
        #endregion

        #region LeftBottomRightTopDiagonal
        [TestMethod]
        public void CleanLines_9LBRTDiagonal_CheckFromLeft_ScoreIs45()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 0; i < 9; i++)
                field[i, 8 - i] = _redLine;

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(8, 0));

            Assert.AreEqual(45, score);

            for (int i = 0; i < 9; i++)
                Assert.AreEqual(_redLine + 20, _lines.Field[i, 8 - i]);

            CheckFiled(9);
        }

        [TestMethod]
        public void CleanLines_9LBRTDiagonal_CheckFromMiddle_ScoreIs45()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 0; i < 9; i++)
                field[i, 8 - i] = _redLine;

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(4, 4));

            Assert.AreEqual(45, score);

            for (int i = 0; i < 9; i++)
                Assert.AreEqual(_redLine + 20, _lines.Field[i, 8 - i]);

            CheckFiled(9);
        }

        [TestMethod]
        public void CleanLines_9LBRTDiagonal_CheckFromRight_ScoreIs45()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 0; i < 9; i++)
                field[i, 8 - i] = _redLine;

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(0, 8));

            Assert.AreEqual(45, score);

            for (int i = 0; i < 9; i++)
                Assert.AreEqual(_redLine + 20, _lines.Field[i, 8 - i]);

            CheckFiled(9);
        }

        [TestMethod]
        public void CleanLines_9LBRTBDiagonal_ScoreIs0()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 1; i < 5; i++)
                field[i, 8 - i] = _redLine;

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(4, 4));

            Assert.AreEqual(0, score);

            for (int i = 1; i < 5; i++)
                Assert.AreEqual(_redLine, _lines.Field[i, 8 - i]);

            CheckFiled(4, _redLine);
        }
        #endregion

        #region CrossedLines
        [TestMethod]
        public void CleanLines_5HorizontalAnd5Vertical_CheckFromCrossCell_ScoreIs45()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 0; i < 5; i++)
            {
                field[0, i] = _redLine;
                field[i, 0] = _redLine;
            }

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(0, 0));

            Assert.AreEqual(45, score);

            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(_redLine + 20, _lines.Field[0, i]);
                Assert.AreEqual(_redLine + 20, _lines.Field[i, 0]);
            }

            CheckFiled(9);
        }

        [TestMethod]
        public void CleanLines_6HorizontalAnd6Vertical_CheckFromNotCrossedHorizontalCell_ScoreIs77()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 0; i < 6; i++)
            {
                field[0, i] = _redLine;
                field[i, 0] = _redLine;
            }

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(0, 3));

            Assert.AreEqual(77, score);

            for (int i = 0; i < 6; i++)
            {
                Assert.AreEqual(_redLine + 20, _lines.Field[0, i]);
                Assert.AreEqual(_redLine + 20, _lines.Field[i, 0]);
            }

            CheckFiled(11);
        }

        [TestMethod]
        public void CleanLines_7HorizontalAnd7Vertical_CheckFromNotCrossVerticalCell_ScoreIs117()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 0; i < 7; i++)
            {
                field[2, i] = _redLine;
                field[i, 3] = _redLine;
            }

            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(6, 3));

            Assert.AreEqual(117, score);

            for (int i = 0; i < 7; i++)
            {
                Assert.AreEqual(_redLine + 20, _lines.Field[2, i]);
                Assert.AreEqual(_redLine + 20, _lines.Field[i, 3]);
            }

            CheckFiled(13);
        }

        [TestMethod]
        public void CleanLines_CrossedHorizontaVerticalAndDiagonals_ScoreIs252()
        {
            var field = new int[Consts.FieldSize, Consts.FieldSize];

                                                      field[0, 4] = field[0, 5] = field[0, 6] = field[0, 7] = field[0, 8] =
                                                      field[1, 4] =
            field[2, 1] =               field[2, 3] =               field[2, 5] =
                          field[3, 2] =                             field[3, 5] =
            field[4, 1] =               field[4, 3] =               field[4, 5] =
                                                      field[5, 4] = field[5, 5] =
                                                                    field[6, 5] =
                                                                                  field[7, 6] = _redLine;
            _lines.InitTestField(field);

            int score = _lines.CleanLines(new Point(4, 3));

            Assert.AreEqual(252, score);

            field[0, 4] = field[0, 5] = field[0, 6] = field[0, 7] = field[0, 8] =
                                                      field[1, 4] =
            field[2, 1] = field[2, 3] = field[2, 5] =
                          field[3, 2] = field[3, 5] =
            field[4, 1] = field[4, 3] = field[4, 5] =
                                                      field[5, 4] = field[5, 5] =
                                                                    field[6, 5] =
                                                                                  field[7, 6] = _redLine + 20;

            for (int i = 0; i < Consts.FieldSize; i++)
            {
                for (int j = 0; j < Consts.FieldSize; j++)
                {
                    Assert.AreEqual(field[i, j], _lines.Field[i, j]);
                }
            }
        }
        #endregion
    }
}
