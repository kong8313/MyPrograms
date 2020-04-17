using System;
using System.Collections.Generic;
using System.Drawing;

namespace LinesG
{
    public class LinesLogic
    {
        public int CleanLines(int[,] field, Point position)
        {           
            var ballIndex = field[position.X, position.Y];

            if (!(ballIndex > 0 && ballIndex < 10))
            {
                return 0;
            }

            var removedCells = new List<Point>() { position };
            int removedBallsCount = 0;

            while (removedCells.Count > 0)
            {
                int x = removedCells[0].X;
                int y = removedCells[0].Y;

                List<Point> lineCells = CheckHorizontalLine(field, x, y, ballIndex);
                if (lineCells.Count > 0)
                {
                    removedBallsCount += lineCells.Count;
                    removedCells.AddRange(lineCells);
                }

                lineCells = CheckVerticalLine(field, x, y, ballIndex);
                if (lineCells.Count > 0)
                {
                    removedBallsCount += lineCells.Count;
                    removedCells.AddRange(lineCells);
                }

                lineCells = CheckLeftTopRightBottomLine(field, x, y, ballIndex);
                if (lineCells.Count > 0)
                {
                    removedBallsCount += lineCells.Count;
                    removedCells.AddRange(lineCells);
                }

                lineCells = CheckLeftBottomRightTopLine(field, x, y, ballIndex);
                if (lineCells.Count > 0)
                {
                    removedBallsCount += lineCells.Count;
                    removedCells.AddRange(lineCells);
                }

                removedCells.RemoveAt(0);
            }

            if (removedBallsCount > 0)
            {
                field[position.X, position.Y] += 20;
                removedBallsCount++;
            }

            return ConvertCountToScores(removedBallsCount);
        }

        private int ConvertCountToScores(int n)
        {
            return n * (n - 4);
        }

        private List<Point> CheckHorizontalLine(int[,] field, int x, int y, int ballIndex)
        {
            var removedCells = new List<Point>();

            int j = y - 1;
            while (j >= 0 && field[x, j] == ballIndex)
            {                
                removedCells.Add(new Point(x, j));
                j--;
            }

            j = y + 1;
            while (j < Consts.FieldSize && field[x, j] == ballIndex)
            {
                removedCells.Add(new Point(x, j));
                j++;
            }

            if (removedCells.Count < 4)
            {
                return new List<Point>();
            }

            foreach (var removedCell in removedCells)
            {
                field[removedCell.X, removedCell.Y] = ballIndex + 20;
            }

            return removedCells;
        }

        private List<Point> CheckVerticalLine(int[,] field, int x, int y, int ballIndex)
        {
            var removedCells = new List<Point>();

            int i = x - 1;
            while (i >= 0 && field[i, y] == ballIndex)
            {
                removedCells.Add(new Point(i, y));
                i--;
            }

            i = x + 1;
            while (i < Consts.FieldSize && field[i, y] == ballIndex)
            {
                removedCells.Add(new Point(i, y));
                i++;
            }

            if (removedCells.Count < 4)
            {
                return new List<Point>();
            }

            foreach (var removedCell in removedCells)
            {
                field[removedCell.X, removedCell.Y] = ballIndex + 20;
            }

            return removedCells;
        }

        private List<Point> CheckLeftBottomRightTopLine(int[,] field, int x, int y, int ballIndex)
        {
            var removedCells = new List<Point>();

            int i = x - 1;
            int j = y - 1;
            while (i >= 0 && j >= 0 && field[i, j] == ballIndex)
            {
                removedCells.Add(new Point(i, j));
                i--;
                j--;
            }

            i = x + 1;
            j = y + 1;
            while (i < Consts.FieldSize && j < Consts.FieldSize && field[i, j] == ballIndex)
            {
                removedCells.Add(new Point(i, j));
                i++;
                j++;
            }

            if (removedCells.Count < 4)
            {
                return new List<Point>();
            }

            foreach (var removedCell in removedCells)
            {
                field[removedCell.X, removedCell.Y] = ballIndex + 20;
            }

            return removedCells;
        }

        private List<Point> CheckLeftTopRightBottomLine(int[,] field, int x, int y, int ballIndex)
        {
            var removedCells = new List<Point>();

            int i = x + 1;
            int j = y - 1;
            while (i < Consts.FieldSize && j >= 0 && field[i, j] == ballIndex)
            {
                removedCells.Add(new Point(i, j));
                i++;
                j--;
            }

            i = x - 1;
            j = y + 1;
            while (i >= 0 && j < Consts.FieldSize && field[i, j] == ballIndex)
            {
                removedCells.Add(new Point(i, j));
                i--;
                j++;
            }

            if (removedCells.Count < 4)
            {
                return new List<Point>();
            }

            foreach (var removedCell in removedCells)
            {
                field[removedCell.X, removedCell.Y] = ballIndex + 20;
            }

            return removedCells;
        }

        public Point[] GetPath(int[,] field, Point fromPosition, Point toPosition)
        {
            int[,] tempField = new int[Consts.FieldSize, Consts.FieldSize];

            for (int i = 0; i < Consts.FieldSize; i++)
            {
                for (int j = 0; j < Consts.FieldSize; j++)
                {
                    tempField[i, j] = field[i, j] > 0 ? -1 : 0;
                }
            }

            tempField[fromPosition.X, fromPosition.Y] = 1;

            var newFront = new List<Point>();
            newFront.Add(new Point(fromPosition.X, fromPosition.Y));

            while (newFront.Count > 0)
            {
                var oldFront = new List<Point>(newFront);
                newFront = new List<Point>();

                foreach (Point position in oldFront)
                {
                    int i = position.X;
                    int j = position.Y;

                    if (i > 0 && tempField[i - 1, j] == 0)
                    {
                        tempField[i - 1, j] = tempField[i, j] + 1;
                        newFront.Add(new Point(i - 1, j));
                    }

                    if (i < Consts.FieldSize - 1 && tempField[i + 1, j] == 0)
                    {
                        tempField[i + 1, j] = tempField[i, j] + 1;
                        newFront.Add(new Point(i + 1, j));
                    }

                    if (j > 0 && tempField[i, j - 1] == 0)
                    {
                        tempField[i, j - 1] = tempField[i, j] + 1;
                        newFront.Add(new Point(i, j - 1));
                    }

                    if (j < Consts.FieldSize - 1 && tempField[i, j + 1] == 0)
                    {
                        tempField[i, j + 1] = tempField[i, j] + 1;
                        newFront.Add(new Point(i, j + 1));
                    }
                }

                if (tempField[toPosition.X, toPosition.Y] > 0)
                {
                    break;
                }
            }

            if (tempField[toPosition.X, toPosition.Y] > 0)
            {
                var path = new List<Point>();
                Point currentPosition = new Point(toPosition.X, toPosition.Y);

                while (tempField[currentPosition.X, currentPosition.Y] != 1)
                {
                    var i = currentPosition.X;
                    var j = currentPosition.Y;
                    var nextValue = tempField[i, j] - 1;
                    path.Add(new Point(i, j));

                    if (i > 0 && tempField[i - 1, j] == nextValue)
                    {
                        currentPosition = new Point(i - 1, j);
                    }
                    else if (i < Consts.FieldSize - 1 && tempField[i + 1, j] == nextValue)
                    {
                        currentPosition = new Point(i + 1, j);
                    }
                    else if (j > 0 && tempField[i, j - 1] == nextValue)
                    {
                        currentPosition = new Point(i, j - 1);
                    }
                    else if (j < Consts.FieldSize - 1 && tempField[i, j + 1] == nextValue)
                    {
                        currentPosition = new Point(i, j + 1);
                    }
                    else
                    {
                        throw new Exception("Path finding algorithm exception");
                    }
                }

                path.Add(new Point(fromPosition.X, fromPosition.Y));
                return path.ToArray();
            }

            return null;
        }
    }
}
