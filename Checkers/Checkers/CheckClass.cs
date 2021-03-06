using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Checkers
{
    /// <summary>
    /// Класс, опысавающий работу с одной шашкой
    /// </summary>
    public class CheckClass
    {
        public int X, Y;								// Номер клетки на поле по горизонтали (слева направо) и по 
        // вертикали (сверху вниз), в которой стоит данная шашка			
        public TypeCheck typeCheck;			// Тип шашки
        public ColorCheck colorCheck;		// Цвет шашки
        public PictureBox pictureBox;		// Контрол для отображения нашей шашки на поле
        public int[,] dest;							// Координаты полей, куда может попасть шашка
        public bool mustGo;							// Признак того, что этой шашкой надо обязательно сходить		
        MainForm mf;

        public CheckClass(int x, int y, MainForm mainForm)
            : this(x, y, ColorCheck.unknown, mainForm)
        {
        }

        public CheckClass(int x, int y, ColorCheck cCheck, MainForm mainForm)
        {
            mf = mainForm;
            X = x;
            Y = y;
            typeCheck = TypeCheck.check;
            colorCheck = cCheck;
            mustGo = false;
            pictureBox = new PictureBox();
            pictureBox.Size = new Size(mf.nSize, mf.nSize);
            if (!mf.changeSide)
                pictureBox.Location = new Point(mf.nSize * y + mf.leftX, mf.nSize * x + mf.leftY);
            else
                pictureBox.Location = new Point(mf.nSize * (7 - y) + mf.leftX, mf.nSize * (7 - x) + mf.leftY);

            if (colorCheck == ColorCheck.black)
            {
                pictureBox.Image = global::Checkers.Properties.Resources.check_black;
                dest = new int[0, 2];
            }
            else if (colorCheck == ColorCheck.white)
            {
                pictureBox.Image = global::Checkers.Properties.Resources.check_white;
                dest = new int[0, 2];
            }
            else if ((x + y) % 2 == 0)
            {
                pictureBox.Image = global::Checkers.Properties.Resources.empty;
            }
            else
            {
                pictureBox.Image = global::Checkers.Properties.Resources.full;
            }

            pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(pictureBox_MouseDown);
            mainForm.Controls.Add(pictureBox);
        }

        /// <summary>
        /// Обработка нажатия мышкой на pictureBox-e
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (mf.placing)
                mf.AddCheck(X, Y);
            else
                mf.action.Press(X, Y);
        }
    }
}
