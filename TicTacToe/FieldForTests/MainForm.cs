using System;
using System.Drawing;
using System.Windows.Forms;
using TicTacToe;

namespace FieldForTests
{
    public partial class MainForm : Form
    {
        private PaintClass _paintClass;		        // Общий класс для рисования

        private ObjectType[,] _field;		        // Поле с текущей позицией
        private int _rowsCnt;                       // Количество строк на поле
        private int _columnsCnt;                    // Количество столбцов на поле
        private readonly FieldConverter _fieldConverter;     // Объект для конвертации поля в текстовый вид и обратно

        public MainForm()
        {
            InitializeComponent();
            _fieldConverter = new FieldConverter();
        }


        /// <summary>
        /// Загрузка формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            _paintClass = new PaintClass();

            _rowsCnt = PaintClass.BitmapWidth / PaintClass.CellSize;
            _columnsCnt = PaintClass.BitmapHeight / PaintClass.CellSize;
            _field = new ObjectType[_rowsCnt, _columnsCnt];

            PaintBitmap();
        }

        public void PaintBitmap()
        {
            using (Graphics g = panelField.CreateGraphics())
            {
                g.DrawImage(_paintClass.FieldBitmap, 0, 0);
            }
        }

        /// <summary>
        /// Перерисовка изображения на панели
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panelField_Paint(object sender, PaintEventArgs e)
        {
            PaintBitmap();
        }

        /// <summary>
        /// Выставление крестиков и ноликов.
        /// Автоматически чередовать, если не выбрано очищение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panelField_MouseDown(object sender, MouseEventArgs e)
        {
            ObjectType objectToSet;
            if (e.Button == MouseButtons.Left)
            {
                objectToSet = ObjectType.Cross;
                //buttonSetX_Click(null, null);
            }
            else if (e.Button == MouseButtons.Right)
            {
                objectToSet = ObjectType.Nil;
                //buttonSetO_Click(null, null);
            }
            else
            {
                objectToSet = ObjectType.Empty;
                //buttonClearCell_Click(null, null);
            }

            int x = e.Y / PaintClass.CellSize;
            int y = e.X / PaintClass.CellSize;

            _field[x, y] = objectToSet;
            _paintClass.DrawObject(objectToSet, x, y);
            PaintBitmap();
        }

        private void buttonSetX_Click(object sender, EventArgs e)
        {
            buttonSetX.FlatStyle = FlatStyle.Flat;
            buttonSetO.FlatStyle = FlatStyle.Standard;
            buttonClearCell.FlatStyle = FlatStyle.Standard;
            richTextBoxTextField.Focus();
        }

        private void buttonSetO_Click(object sender, EventArgs e)
        {
            buttonSetX.FlatStyle = FlatStyle.Standard;
            buttonSetO.FlatStyle = FlatStyle.Flat;
            buttonClearCell.FlatStyle = FlatStyle.Standard;
            richTextBoxTextField.Focus();
        }

        private void buttonClearCell_Click(object sender, EventArgs e)
        {
            buttonSetX.FlatStyle = FlatStyle.Standard;
            buttonSetO.FlatStyle = FlatStyle.Standard;
            buttonClearCell.FlatStyle = FlatStyle.Flat;
            richTextBoxTextField.Focus();
        }

        private void buttonClearField_Click(object sender, EventArgs e)
        {
            _field = new ObjectType[_rowsCnt, _columnsCnt];
            _paintClass = new PaintClass();
            PaintBitmap();
            richTextBoxTextField.Focus();
        }

        private void buttonTextToPicture_Click(object sender, EventArgs e)
        {
            _paintClass = new PaintClass();

            _field = _fieldConverter.TextToField(richTextBoxTextField.Text, _rowsCnt, _columnsCnt);
            for (int i = 0; i < _rowsCnt; i++)
            {
                for (int j = 0; j < _columnsCnt; j++)
                {
                    _paintClass.DrawObject(_field[i, j], i, j);
                }
            }

            PaintBitmap();
            richTextBoxTextField.Focus();
        }

        private void buttonPictureToText_Click(object sender, EventArgs e)
        {
            richTextBoxTextField.Text = _fieldConverter.FieldToText(_field);
            richTextBoxTextField.Focus();
        }

        private void buttonDoStep_Click(object sender, EventArgs e)
        {
            ObjectType whoStep;
            if (buttonSetX.FlatStyle == FlatStyle.Flat)
                whoStep = ObjectType.Cross;
            else if (buttonSetO.FlatStyle == FlatStyle.Flat)
            {
                whoStep = ObjectType.Nil;
            }
            else
            {
                MessageBox.Show("Select object to do step", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int[] step = ComputerClass.DoStep(_field, whoStep);
            _field[step[0], step[1]] = whoStep;
            labelStep.Text = $"{step[0]}:{step[1]}";

            _paintClass.DrawObject(whoStep, step[0], step[1]);
            PaintBitmap();
        }
    }
}
