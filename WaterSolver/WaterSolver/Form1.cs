using System.Diagnostics;

namespace WaterSolver
{
    public partial class WaterSolverForm : Form
    {
        private Board _board;
        private Stopwatch _stopwatch;

        public WaterSolverForm()
        {
            try
            {
                InitializeComponent();
                _stopwatch = new Stopwatch();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void WaterSolverForm_Shown(object sender, EventArgs e)
        {
            try
            {
                _board = new Board();

                DrawField();
                //DrawCells();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void DrawCells()
        {
            pictureBoxCells.BackgroundImage = _board.GetCellsField();
        }

        private void DrawField()
        {
            pictureBoxField.BackgroundImage = _board.GetImageField();
            DrawCells();
        }

        private void pictureBoxCells_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (_board.SetCurrentCell(e.X, e.Y))
                {
                    DrawCells();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonAddFlask_Click(object sender, EventArgs e)
        {
            try
            {
                _board.AddFlask();
                DrawField();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonRemoveFlask_Click(object sender, EventArgs e)
        {
            try
            {
                _board.RemoveFlask();
                DrawField();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void pictureBoxField_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (_board.PutCurrentCellInFlaskCell(e.X, e.Y))
                {
                    DrawField();
                    DrawCells();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void buttonFindSolution_Click(object sender, EventArgs e)
        {
            try
            {
                _stopwatch.Reset();
                _stopwatch.Start();
                panelInfo.Visible = true;
                timer.Enabled = true;
                Task task = new TaskFactory().StartNew(() => _board.FindSolution(this));
                await task.WaitAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                _stopwatch.Stop();
                timer_Tick(null, null);
                timer.Enabled = false;
            }
        }

        public void ShowSolution()
        {
            dataGridViewSteps.Invoke(new MethodInvoker(UpdateDataGridView));
            if (!_board.Solution.Last().IsSolution)
            {
                MessageBox.Show("No solution. The deapest finish position is presented.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateDataGridView()
        {
            dataGridViewSteps.Rows.Clear();

            int i = 0;
            foreach (var position in _board.Solution)
            {
                dataGridViewSteps.Rows.Add(i == 0 ? "" : i.ToString(), position.Steps.Last().ToString());
                i++;
            }

            dataGridViewSteps.Select();
        }

        private void buttonSavePosition_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _board.SavePosition(saveFileDialog.FileName);
            }
        }

        private void buttonLoadPosition_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _board.LoadPosition(openFileDialog.FileName);
                DrawField();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            var time = new DateTime(_stopwatch.ElapsedTicks);
            labelInfo2.Text = $"{time.ToString("HH:mm:ss")}\r\n{_board.GetCurrentFirstPositionsProgress()}/{_board.GetFirstPositionsCount()}";
            labelInfo4.Text = $"{_board.GetMaxDepth()}\r\n{_board.GetFinishPositionsCount()}";
            labelInfo6.Text = $"{_board.GetHashesCount()}";
            Application.DoEvents();
        }

        private void dataGridViewSteps_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                _board.SetSolutionPosition(e.RowIndex);
                DrawField();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
