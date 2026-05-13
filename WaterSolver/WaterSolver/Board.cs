using System.Text;

namespace WaterSolver
{
    internal class Board
    {
        public List<Position> Solution { get; set; }
        private SolutionExplorer _solutionExplorer { get; set; }

        private List<sbyte[]> _flasks;
        private List<FlaskCell> _flaskCells;
        private int _selectedCell;

        private const int CellStartLeftPoint = 10;
        private const int CellTopPoint = 5;
        private const int CellHeaderHeight = 20;
        private const int CellWidth = 55;

        private const int MaxFlasks = 18;
        private const int FlaskTopPoint = 15;

        public Board()
        {
            _flasks = new List<sbyte[]> { new sbyte[4], new sbyte[4], new sbyte[4], new sbyte[4], new sbyte[4] };

            _selectedCell = 0;
            InitializeFlaskCells();
        }

        private void InitializeFlaskCells()
        {
            var cellsPath = Path.Combine(Application.StartupPath, "Cells");
            if (!Directory.Exists(cellsPath))
            {
                throw new Exception("No directory with cell images");
            }

            var unknownCellImagePath = Path.Combine(cellsPath, "_Black.png");
            if (!File.Exists(unknownCellImagePath))
            {
                throw new Exception("No cell image for unknown cell _Black.png");
            }

            var emptyCellImagePath = Path.Combine(cellsPath, "_Empty.png");
            if (!File.Exists(emptyCellImagePath))
            {
                throw new Exception("No cell image for empty cell _Empty.png");
            }

            var directoryInfo = new DirectoryInfo(cellsPath);
            _flaskCells = new List<FlaskCell> { new(), new() };
            _flaskCells[0].Load(-1, unknownCellImagePath);
            _flaskCells[1].Load(0, emptyCellImagePath);

            int number = 1;
            foreach (var fileInfo in directoryInfo.EnumerateFiles())
            {
                if (fileInfo.Name is "_Black.png" or "_Empty.png")
                {
                    continue;
                }

                var flaskCell = new FlaskCell();
                flaskCell.Load(number, fileInfo.FullName);
                _flaskCells.Add(flaskCell);
                number++;
            }

            if (_flaskCells.Count > MaxFlasks)
            {
                throw new Exception($"Maximum supported cell images is {MaxFlasks}. You have loaded {_flaskCells.Count}");
            }

            int width = 0;
            int height = 0;
            foreach (var flaskCell in _flaskCells)
            {
                if (width == 0)
                {
                    width = flaskCell.CellPicture.Width;
                    height = flaskCell.CellPicture.Height;
                    continue;
                }

                if (width != flaskCell.CellPicture.Width ||
                    height != flaskCell.CellPicture.Height)
                {
                    throw new Exception($"All cells must have the same width ({width}) and height ({height}). Looks like {flaskCell.Name} has a different value.");
                }
            }
        }

        public Image GetImageField()
        {
            var drawFont = new Font("Microsoft Sans Serif", (float)8.25, FontStyle.Regular);

            Bitmap pict = new Bitmap(1000, 150, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(pict);
            Pen pen = new Pen(Color.Black);
            Brush brush = new SolidBrush(Color.White);

            g.FillRectangle(brush, 0, 0, pict.Width - 1, pict.Height - 1);
            g.DrawRectangle(pen, 0, 0, pict.Width - 1, pict.Height - 1);

            brush = new SolidBrush(Color.Black);
            for (int i = 0; i < _flasks.Count; i++)
            {
                int leftPoint = CellWidth * i + CellStartLeftPoint;
                g.DrawString((i + 1).ToString(), drawFont, brush, leftPoint + 5, FlaskTopPoint);

                for (int j = 3; j >= 0; j--)
                {
                    int cellNumber = _flasks[i][j];
                    var cellImageHeight = _flaskCells[cellNumber + 1].CellPicture.Height;
                    int topPoint = FlaskTopPoint + CellHeaderHeight + (3 - j) * cellImageHeight;
                    
                    g.DrawImage(_flaskCells[cellNumber + 1].CellPicture, new Point(leftPoint, topPoint));
                    g.DrawRectangle(pen, leftPoint, topPoint,
                        _flaskCells[cellNumber + 1].CellPicture.Width, cellImageHeight);
                }
            }

            brush.Dispose();
            pen.Dispose();
            return pict;
        }

        public Image GetCellsField()
        {
            var drawFont = new Font("Microsoft Sans Serif", (float)8.25, FontStyle.Regular);

            Bitmap pict = new Bitmap(1000, 60, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(pict);
            Pen pen = new Pen(Color.Black);
            Brush brush = new SolidBrush(Color.White);
            g.FillRectangle(brush, 0, 0, pict.Width - 1, pict.Height - 1);
            g.DrawRectangle(pen, 0, 0, pict.Width - 1, pict.Height - 1);

            brush = new SolidBrush(Color.Black);
            for (int i = 0; i < _flaskCells.Count; i++)
            {
                int leftPoint = CellWidth * i + CellStartLeftPoint;
                g.DrawString(_flaskCells[i].Name, drawFont, brush, leftPoint - 5, CellTopPoint);
                g.DrawImage(_flaskCells[i].CellPicture, new Point(leftPoint, CellTopPoint + CellHeaderHeight));
                g.DrawRectangle(pen, leftPoint, CellTopPoint + CellHeaderHeight, 
                     _flaskCells[i].CellPicture.Width, _flaskCells[i].CellPicture.Height);
                DrawColorCount(g, i, leftPoint);
            }

            int selectedLeftPoint = CellWidth * _selectedCell + CellStartLeftPoint;
            pen = new Pen(Color.GreenYellow, 2);
            g.DrawRectangle(pen, selectedLeftPoint, CellTopPoint + CellHeaderHeight,
                _flaskCells[_selectedCell].CellPicture.Width, _flaskCells[_selectedCell].CellPicture.Height);

            brush.Dispose();
            pen.Dispose();
            return pict;
        }

        private void DrawColorCount(Graphics g, int i, int leftPoint)
        {
            int cnt = GetDrawnColorCount(_flaskCells[i].Number);
            Font drawFont;
            Brush brush;
            if (i < 2 || cnt < 4)
            {
                drawFont = new Font("Microsoft Sans Serif", (float)8.25, FontStyle.Regular);
                brush = new SolidBrush(Color.Black);
            }
            else if (cnt == 4)
            {
                drawFont = new Font("Microsoft Sans Serif", (float)8.25, FontStyle.Bold);
                brush = new SolidBrush(Color.Black);
            }
            else
            {
                drawFont = new Font("Microsoft Sans Serif", (float)8.25, FontStyle.Bold);
                brush = new SolidBrush(Color.Red);
            }

            g.DrawString(cnt.ToString(), drawFont, brush, 
                leftPoint + 5, CellTopPoint + CellHeaderHeight + _flaskCells[i].CellPicture.Height + 1);
        }

        private int GetDrawnColorCount(int colorNumber)
        {
            int cnt = 0;
            foreach (var flask in _flasks)
            {
                for (int j = 3; j >= 0; j--)
                {
                    if (flask[j] == colorNumber)
                    {
                        cnt++;
                    }
                }
            }

            return cnt;
        }

        public bool SetCurrentCell(int x, int y)
        {
            if (y < CellTopPoint + CellHeaderHeight ||
                y > CellTopPoint + CellHeaderHeight + _flaskCells[0].CellPicture.Height)
            {
                return false;
            }

            var newSelectedCell = (x - CellStartLeftPoint) / CellWidth;
            if (newSelectedCell >= _flaskCells.Count)
            {
                return false;
            }

            _selectedCell = newSelectedCell;
            return true;
        }

        public void AddFlask()
        {
            if (_flasks.Count + 1 > MaxFlasks)
            {
                throw new Exception($"You can't add a new flask because the maximum supported flasks is {MaxFlasks}.");
            }

            _flasks.Add(new sbyte[4]);
        }

        public void RemoveFlask()
        {
            if (_flasks.Count == 0)
            {
                throw new Exception($"You can't remove a flask because you have no one.");
            }

            _flasks.RemoveAt(_flasks.Count - 1);
        }

        public bool PutCurrentCellInFlaskCell(int x, int y)
        {
            if (y <= FlaskTopPoint + CellHeaderHeight ||
                y >= FlaskTopPoint + CellHeaderHeight + 4 *_flaskCells[0].CellPicture.Height)
            {
                return false;
            }

            var selectedFlask = (x - CellStartLeftPoint) / CellWidth;
            if (selectedFlask >= _flasks.Count)
            {
                return false;
            }

            var selectedCell = 3 - (y - FlaskTopPoint - CellHeaderHeight) / _flaskCells[0].CellPicture.Height;

            _flasks[selectedFlask][selectedCell] = (sbyte)(_selectedCell - 1);

            return true;
        }

        public void FindSolution(WaterSolverForm waterSolverForm)
        {
            _solutionExplorer = new SolutionExplorer(_flasks, _flaskCells);
            Solution = _solutionExplorer.Find();
            waterSolverForm.ShowSolution();
        }

        public int GetCurrentFirstPositionsProgress()
        {
            return _solutionExplorer.CurrentFirstPositionsNumber;
        }

        public int GetFirstPositionsCount()
        {
            return _solutionExplorer.FirstPositionsCount;
        }

        public int GetMaxDepth()
        {
            return _solutionExplorer.MaxDepth;
        }

        public int GetFinishPositionsCount()
        {
            return _solutionExplorer.FinishPositionsCount;
        }

        public int GetHashesCount()
        {
            return _solutionExplorer.HashesCount;
        }

        public void SetSolutionPosition(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= Solution.Count)
            {
                throw new Exception($"Wrong step number {stepIndex}");
            }

            _flasks = Solution[stepIndex].Flasks;
        }

        public void SavePosition(string fileName)
        {
            var sb = new StringBuilder();
            foreach (var flask in _flasks)
            {
                sb.Append(string.Join(',', flask) + "^");
            }

            string result = sb.ToString().TrimEnd('^');
            File.WriteAllText(fileName, result);
        }

        public void LoadPosition(string fileName)
        {
            var content = File.ReadAllText(fileName);

            _flasks = new List<sbyte[]>();
            var flasks = content.Split('^');
            for (var i = 0; i < flasks.Length; i++)
            {
                var cells = flasks[i].Split(',');
                _flasks.Add(new sbyte[cells.Length]);
                for (var j = 0; j < cells.Length; j++)
                {
                    _flasks[i][j] = Convert.ToSByte(cells[j]);
                }
            }
        }
    }
}
