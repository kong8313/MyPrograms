using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinesG
{
    public partial class MainForm : Form
    {
        private Leaders _leaders;

        private Lines _lines;
        private int _score;
        private int _prevSavedScore;
        private int _undoStepCnt;

        private int _timeInSec;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            panelField.Visible = false;

            panelField.Size = new Size(Consts.FieldSize * Consts.CellSize + 1, Consts.FieldSize * Consts.CellSize + 1);

            Size = new Size(Consts.FieldSize * Consts.CellSize + 17, Consts.FieldSize * Consts.CellSize + 117);

            for (int rowIndex = 0; rowIndex < Consts.FieldSize; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < Consts.FieldSize; columnIndex++)
                {
                    AddPictureBox(rowIndex, columnIndex);
                }
            }

            openFileDialog.InitialDirectory = Application.StartupPath;
            saveFileDialog.InitialDirectory = Application.StartupPath;

            panelField.Visible = true;

            RunNewGame();
        }

        private void RunNewGame()
        {
            _score = 0;
            _timeInSec = 0;
            _undoStepCnt = 0;

            _lines = new Lines();
            _leaders = new Leaders();

            labelScore.Text = _score.ToString("D5");
            labelMaxScore.Text = _leaders.MaxScore.ToString("D5");
            timerGame.Enabled = true;

            ShowFutureBalls();
        }

        private void AddPictureBox(int rowIndex, int columnIndex)
        {
            var pictureBox = new PictureBox
            {
                BackgroundImageLayout = ImageLayout.None,
                Location = new Point(columnIndex * Consts.CellSize, rowIndex * Consts.CellSize),
                Name = GetPictureBoxName(rowIndex, columnIndex),
                Size = new Size(Consts.CellSize, Consts.CellSize),
                TabStop = true,
            };

            pictureBox.MouseClick += new MouseEventHandler(pictureBox_MouseClick);

            panelField.Controls.Add(pictureBox);
        }

        private void panelField_Paint(object sender, PaintEventArgs e)
        {
            using (var g = panelField.CreateGraphics())
            {
                g.DrawLine(new Pen(Color.Black), panelField.Width - 1, 0, panelField.Width - 1, panelField.Height - 1);
                g.DrawLine(new Pen(Color.Black), 0, panelField.Height - 1, panelField.Width - 1, panelField.Height - 1);
            }

            for (int rowIndex = 0; rowIndex < Consts.FieldSize; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < Consts.FieldSize; columnIndex++)
                {
                    var pictureBox = (PictureBox)panelField.Controls[GetPictureBoxName(rowIndex, columnIndex)];
                    if (IsBitmapsDifferent((Bitmap)_lines.GetImage(rowIndex, columnIndex), (Bitmap)pictureBox.Image))
                    {
                        pictureBox.Image = _lines.GetImage(rowIndex, columnIndex);
                    }
                }
            }

            Application.DoEvents();
        }

        private bool IsBitmapsDifferent(Bitmap image1, Bitmap image2)
        {
            if ((image1 == null && image2 != null) || (image1 != null && image2 == null))
            {
                return true;
            }

            byte[] image1Bytes;
            byte[] image2Bytes;

            using (var mstream = new MemoryStream())
            {
                image1.Save(mstream, image1.RawFormat);
                image1Bytes = mstream.ToArray();
            }

            using (var mstream = new MemoryStream())
            {
                image2.Save(mstream, image2.RawFormat);
                image2Bytes = mstream.ToArray();
            }

            var image164 = Convert.ToBase64String(image1Bytes);
            var image264 = Convert.ToBase64String(image2Bytes);

            return !string.Equals(image164, image264);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private async void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                var pictureBox = (PictureBox)sender;

                Point position = GetPictureBoxPosition(pictureBox.Name);

                Point[] revertedPath = _lines.ProceedClick(position);
                if (revertedPath == null)
                {
                    panelField_Paint(null, null);
                    return;
                }

                _lines.SavePosition();
                _lines.StopJumping();
                _lines.SaveLastMovedCellPosition(revertedPath.Last());
                stepBackToolStripMenuItem.Enabled = true;

                for (int i = revertedPath.Length - 1; i > 0; i--)
                {
                    _lines.DoMove(revertedPath[i], revertedPath[i - 1]);
                    panelField_Paint(null, null);
                }

                int scores = _lines.CleanLines(position);

                if (scores == 0)
                {
                    var showedCells = _lines.ShowNewBalls();
                    await ShowAction();

                    foreach (var showedCell in showedCells)
                    {
                        scores += _lines.CleanLines(showedCell);
                    }

                    if (scores > 0)
                    {
                        await ShowAction();
                    }

                    if (!_lines.AddFutureBalls())
                    {
                        finishGameToolStripMenuItem_Click(null, null);
                        return;
                    }
                    else
                    {
                        ShowFutureBalls();
                    }
                }
                else
                {
                    await ShowAction(position);
                }

                ShowScore(scores);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла непредвиденная ошибка. Отправьте её разработичку, если сможете :)\r\n" + ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowFutureBalls()
        {
            var futureImages = _lines.GetFutureImages();

            pictureBoxFuture1.Visible = futureImages.Count > 0;
            if (pictureBoxFuture1.Visible)
            {
                pictureBoxFuture1.Image = futureImages[0];
            }

            pictureBoxFuture2.Visible = futureImages.Count > 1;
            if (pictureBoxFuture2.Visible)
            {
                pictureBoxFuture2.Image = futureImages[1];
            }

            pictureBoxFuture3.Visible = futureImages.Count > 2;
            if (pictureBoxFuture3.Visible)
            {
                pictureBoxFuture3.Image = futureImages[2];
            }

            panelField_Paint(null, null);
        }

        private async Task ShowAction(Point? position = null)
        {
            panelField_Paint(null, null);
            await Task.Delay(500);

            _lines.FixDynamicBalls(position);
            panelField_Paint(null, null);
        }

        private void ShowScore(int newPoints)
        {
            _prevSavedScore = _score;
            _score += newPoints;
            labelScore.Text = _score.ToString("D5");
        }

        private string GetPictureBoxName(int rowIndex, int columnIndex)
        {
            return $"pictureBox{rowIndex}_{columnIndex}";
        }

        private Point GetPictureBoxPosition(string name)
        {
            var subname = name.Substring("pictureBox".Length);
            var indexes = subname.Split('_');

            return new Point(int.Parse(indexes[0]), int.Parse(indexes[1]));
        }

        private void stepBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_undoStepCnt == 3)
                {
                    if (DialogResult.No == MessageBox.Show("Вы уверены, что хотите сделать шаг назад? Вы не попадёте в таблицу лидеров!", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        return;
                    }
                }

                _undoStepCnt++;

                _score = _prevSavedScore;
                ShowScore(0);

                _lines.LoadPosition();
                stepBackToolStripMenuItem.Enabled = false;
                panelField_Paint(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла непредвиденная ошибка. Отправьте её разработичку, если сможете :)\r\n" + ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lidersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var leadersForm = new LeadersForm(_leaders);
            leadersForm.ShowDialog();
        }

        private void timerGame_Tick(object sender, EventArgs e)
        {
            _timeInSec++;

            labelTime.Text = TimeConverter.Convert(_timeInSec);
        }

        private void finishGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender != null && DialogResult.No == MessageBox.Show("Вы действительно хотите окончить текукщую игру?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    return;
                }

                timerGame.Enabled = false;

                if (_undoStepCnt > 3 || _score <= _leaders.MinScore)
                {
                    if (DialogResult.Yes == MessageBox.Show("Игра окончена. Ваш результат не попал в десятку лучших. Хотите сыграть ещё раз?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        RunNewGame();
                    }
                    else
                    {
                        exitToolStripMenuItem_Click(null, null);
                    }

                    return;
                }

                var newLeaderNameForm = new NewLeaderNameForm();
                newLeaderNameForm.ShowDialog();

                _leaders.AddNewLeader(newLeaderNameForm.UserName, _score, _timeInSec);

                lidersToolStripMenuItem_Click(null, null);

                RunNewGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла непредвиденная ошибка. Отправьте её разработичку, если сможете :)\r\n" + ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Вы хотите начать новую игру? Ваш текущий результат будет анулирован.", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                RunNewGame();
            }
        }

        private void howToPlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new HowToPlayForm().ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == saveFileDialog.ShowDialog())
            {
                _lines.SaveGame(saveFileDialog.FileName, _score, _timeInSec, _undoStepCnt);
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.OK == openFileDialog.ShowDialog())
                {
                    _lines.LoadGame(openFileDialog.FileName, out _score, out _timeInSec, out _undoStepCnt);

                    stepBackToolStripMenuItem.Enabled = false;
                    ShowScore(0);
                    panelField_Paint(null, null);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
