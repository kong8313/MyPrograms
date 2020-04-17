using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LinesG
{
    public class Lines
    {
        private readonly LinesLogic _linesLogic;

        private int[,] _savedField = new int[Consts.FieldSize, Consts.FieldSize];
        private int _prevSavedValue = 0;

        private Point? _jumpedPosition;

        private readonly Random _random;

        public int[,] Field { get; } = new int[Consts.FieldSize, Consts.FieldSize];

        public Lines()
        {
            _linesLogic = new LinesLogic();

            _random = new Random();
            _jumpedPosition = null;

            InitNewField();
        }

        public void InitNewField()
        {
            for (int i = 0; i < Consts.FieldSize; i++)
            {
                for (int j = 0; j < Consts.FieldSize; j++)
                {
                    Field[i, j] = 0;
                }
            }

            for (int i = 0; i < 5; i++)
            {
                var position = GetRandomPosition();
                var ballIndex = GetRandomBallIndex();

                Field[position.Value.X, position.Value.Y] = ballIndex;
            }

            AddFutureBalls();
        }

        public Point[] ProceedClick(Point position)
        {
            _prevSavedValue = 0;

            var filedValue = Field[position.X, position.Y];

            if (filedValue <= 0 && _jumpedPosition.HasValue)
            {
                return _linesLogic.GetPath(Field, _jumpedPosition.Value, position);
            }
            
            if (filedValue > 10)
            {
                Field[position.X, position.Y] = filedValue - 10;
                _jumpedPosition = null;
            }
            else if (filedValue > 0 && filedValue < 10)
            {
                StartJumping(position);
            }

            return null;
        }

        public void DoMove(Point fromPosition, Point toPosition)
        {
            if (Field[fromPosition.X, fromPosition.Y] > 10)
            {
                Field[fromPosition.X, fromPosition.Y] -= 10;
            }

            int newSavedValue = Field[toPosition.X, toPosition.Y];
            Field[toPosition.X, toPosition.Y] = Field[fromPosition.X, fromPosition.Y];
            Field[fromPosition.X, fromPosition.Y] = _prevSavedValue;
            _prevSavedValue = newSavedValue;
        }

        public void StopJumping()
        {
            if (_jumpedPosition.HasValue && Field[_jumpedPosition.Value.X, _jumpedPosition.Value.Y] > 10)
            {
                Field[_jumpedPosition.Value.X, _jumpedPosition.Value.Y] -= 10;
            }

            _jumpedPosition = null;
        }

        public void StartJumping(Point position)
        {
            StopJumping();

            Field[position.X, position.Y] += 10;
            _jumpedPosition = position;
        }

        public void InitTestField(int[,] testField)
        {
            for (int i = 0; i < Consts.FieldSize; i++)
            {
                for (int j = 0; j < Consts.FieldSize; j++)
                {
                    Field[i, j] = testField[i,j];
                }
            }
        }

        public void SavePosition()
        {
            Array.Copy(Field, _savedField, Consts.FieldSize * Consts.FieldSize);
        }

        public void LoadPosition()
        {
            Array.Copy(_savedField, Field, Consts.FieldSize * Consts.FieldSize);

            for (int i = 0; i < Consts.FieldSize; i++)
            {
                for (int j = 0; j < Consts.FieldSize; j++)
                {
                    if (Field[i, j] > 10 && Field[i, j] < 20)
                    {
                        Field[i, j] -= 10;
                    }
                }
            }
        }

        public List<Point> ShowNewBalls()
        {
            AddFutureBalls();

            _prevSavedValue = 0;

            var showedCells = new List<Point>();

            for (int i = 0; i < Consts.FieldSize; i++)
            {
                for (int j = 0; j < Consts.FieldSize; j++)
                {
                    if (Field[i, j] < 0)
                    {
                        Field[i, j] -= 10;
                        showedCells.Add(new Point(i, j));
                    }                    
                }
            }

            return showedCells;
        }

        public bool AddFutureBalls()
        {
            var emptyCellsCount = 0;
            var futureBallsCount = 0;
            for (int i = 0; i < Consts.FieldSize; i++)
            {
                for (int j = 0; j < Consts.FieldSize; j++)
                {
                    if (Field[i, j] < 0)
                    {
                        futureBallsCount++;
                    }
                    else if (Field[i, j] == 0)
                    {
                        emptyCellsCount++;
                    }
                }
            }

            if (emptyCellsCount == 0)
            {
                return false;
            }
            
            if (futureBallsCount == 2)
            {
                var position = GetRandomPosition();

                if (!position.HasValue)
                {
                    return true;
                }

                Field[position.Value.X, position.Value.Y] = _prevSavedValue;
            }
            else if (futureBallsCount < 3)
            {
                for (int i = futureBallsCount; i < 3; i++)
                {
                    var position = GetRandomPosition();
                    var ballIndex = GetRandomBallIndex() * -1;

                    if (!position.HasValue)
                    {
                        return true;
                    }

                    Field[position.Value.X, position.Value.Y] = ballIndex;
                }
            }

            return true;
        }

        public List<Image> GetFutureImages()
        {
            var images = new List<Image>();

            for (int i = 0; i < Consts.FieldSize; i++)
            {
                for (int j = 0; j < Consts.FieldSize; j++)
                {
                    if (Field[i, j] < 0 && Field[i, j] > -10)
                    {
                        images.Add(GetFutureImageByBall(i, j));
                    }
                }
            }

            return images;
        }

        public int CleanLines(Point position)
        {
            return _linesLogic.CleanLines(Field, position);
        }

        public void FixDynamicBalls(Point? position = null)
        {
            for (int i = 0; i < Consts.FieldSize; i++)
            {
                for (int j = 0; j < Consts.FieldSize; j++)
                {
                    if (Field[i, j] > 20)
                    {
                        Field[i, j] = 0;
                    }
                    else if (Field[i, j] < -10)
                    {
                        Field[i, j] = (Field[i, j] + 10) * -1;
                    }
                }
            }

            if (position.HasValue && _prevSavedValue < 0)
            {
                Field[position.Value.X, position.Value.Y] = _prevSavedValue;
            }
        }

        public void SaveGame(string fileName, int score, int timeInSec, int undoStepCnt)
        {
            var field = new StringBuilder();

            for (int i = 0; i < Consts.FieldSize; i++)
            {
                for (int j = 0; j < Consts.FieldSize; j++)
                {
                    field.Append(Field[i, j] + " ");                   
                }
            }

            string content = $"score={score};timeInSec={timeInSec};undoStepCnt={undoStepCnt};field={field.ToString()}";

            Packer.SaveData(content, fileName);
        }

        public void LoadGame(string fileName, out int score, out int timeInSec, out int undoStepCnt)
        {
            try
            {
                var content = Packer.LoadData(fileName);
                string[] datArr = content.Split(';');

                string[] scoreData = datArr[0].Split('=');
                int tempScore = int.Parse(scoreData[1]);

                string[] timeInSecData = datArr[1].Split('=');
                int tempTimeInSec = int.Parse(timeInSecData[1]);

                string[] undoStepCntData = datArr[2].Split('=');
                int tempUndoStepCnt = int.Parse(undoStepCntData[1]);

                string[] allFieldData = datArr[3].Split('=');
                string[] fieldData = allFieldData[1].Split(' ');
                int n = 0;

                var tempField = new int[Consts.FieldSize, Consts.FieldSize];
                for (int i = 0; i < Consts.FieldSize; i++)
                {
                    for (int j = 0; j < Consts.FieldSize; j++)
                    {
                        tempField[i, j] = int.Parse(fieldData[n]);
                        n++;
                    }
                }

                score = tempScore;
                timeInSec = tempTimeInSec;
                undoStepCnt = tempUndoStepCnt;
                Array.Copy(tempField, Field, Consts.FieldSize * Consts.FieldSize);
            }
            catch(Exception ex)
            {
                throw new Exception("Неправильный формат файла", ex);
            }
        }

        private Point? GetRandomPosition()
        {
            var emptyCells = new List<Point>();

            for (int i = 0; i < Consts.FieldSize; i++)
            {
                for (int j = 0; j < Consts.FieldSize; j++)
                {
                    if (Field[i, j] == 0)
                    {
                        emptyCells.Add(new Point(i, j));
                    }
                }
            }

            if (emptyCells.Count == 0)
            {
                return null;
            }

            int index = _random.Next(0, emptyCells.Count);

            return emptyCells[index];
        }

        private int GetRandomBallIndex()
        {
            return _random.Next(0, 7) + 1;
        }

        private Image GetFutureImageByBall(int rowIndex, int columnIndex)
        {
            switch (Field[rowIndex, columnIndex])
            {
                case -1:
                    return Properties.Resources.Aqua_black;
                case -2:
                    return Properties.Resources.Blue_black;
                case -3:
                    return Properties.Resources.Brown_black;
                case -4:
                    return Properties.Resources.Green_black;
                case -5:
                    return Properties.Resources.Pink_black;
                case -6:
                    return Properties.Resources.Red_black;
                case -7:
                    return Properties.Resources.Yellow_black;

                default:
                    throw new NotSupportedException($"Wrong future ball value {Field[rowIndex, columnIndex]}");
            }
        }

        public Image GetImage(int rowIndex, int columnIndex)
        {
            switch (Field[rowIndex, columnIndex])
            {
                case 0:
                    return Properties.Resources.Empty;
                case 1:
                    return Properties.Resources.Aqua_big;
                case 2:
                    return Properties.Resources.Blue_big;
                case 3:
                    return Properties.Resources.Brown_big;
                case 4:
                    return Properties.Resources.Green_big;
                case 5:
                    return Properties.Resources.Pink_big;
                case 6:
                    return Properties.Resources.Red_big;
                case 7:
                    return Properties.Resources.Yellow_big;  
                    
                case 11:
                    return Properties.Resources.Aqua_jump;
                case 12:
                    return Properties.Resources.Blue_jump;
                case 13:
                    return Properties.Resources.Brown_jump;
                case 14:
                    return Properties.Resources.Green_jump;
                case 15:
                    return Properties.Resources.Pink_jump;
                case 16:
                    return Properties.Resources.Red_jump;
                case 17:
                    return Properties.Resources.Yellow_jump;

                case 21:
                    return Properties.Resources.Aqua_blowup;
                case 22:
                    return Properties.Resources.Blue_blowup;
                case 23:
                    return Properties.Resources.Brown_blowup;
                case 24:
                    return Properties.Resources.Green_blowup;
                case 25:
                    return Properties.Resources.Pink_blowup;
                case 26:
                    return Properties.Resources.Red_blowup;
                case 27:
                    return Properties.Resources.Yellow_blowup;

                case -1:
                    return Properties.Resources.Aqua_small;
                case -2:
                    return Properties.Resources.Blue_small;
                case -3:
                    return Properties.Resources.Brown_small;
                case -4:
                    return Properties.Resources.Green_small;
                case -5:
                    return Properties.Resources.Pink_small;
                case -6:
                    return Properties.Resources.Red_small;
                case -7:
                    return Properties.Resources.Yellow_small;

                case -11:
                    return Properties.Resources.Aqua_grow;
                case -12:
                    return Properties.Resources.Blue_grow;
                case -13:
                    return Properties.Resources.Brown_grow;
                case -14:
                    return Properties.Resources.Green_grow;
                case -15:
                    return Properties.Resources.Pink_grow;
                case -16:
                    return Properties.Resources.Red_grow;
                case -17:
                    return Properties.Resources.Yellow_grow;

                default:
                    throw new NotSupportedException($"Not supported field value {Field[rowIndex, columnIndex]}");
            }
        }
    }
}
