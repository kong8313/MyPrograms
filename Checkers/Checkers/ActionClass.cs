using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Checkers
{
    /// <summary>
    /// Действия над шашками
    /// </summary>
    public class ActionClass
    {
        /// <summary>
        /// Указатель на главную форму
        /// </summary>
        private MainForm mf;

        /// <summary>
        /// Поле с шашками
        /// </summary>
        public CheckClass[,] board;

        /// <summary>
        /// Номер выделенной клетки
        /// </summary>
        private int[] selectCell = new int[2] { -1, -1 };

        /// <summary>
        /// Массив для поиска возможных ходов для шашки
        /// </summary>
        private ArrayList destArray;

        /// <summary>
        /// Кто ходит
        /// </summary>
        public ColorCheck whoMove;

        /// <summary>
        /// Запрет на выделение (когда шашка должна продолжить есть)
        /// </summary>
        private bool forbidSelect = false;

        /// <summary>
        /// Замораживание игры
        /// </summary>
        public bool freezeGame = false;

        public ActionClass(MainForm mainForm)
        {
            mf = mainForm;
            whoMove = mf.whoFirst;
            board = new CheckClass[8, 8];

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 8; j++)
                    if ((i + j) % 2 != 0)
                        board[i, j] = new CheckClass(i, j, ColorCheck.black, mainForm);
                    else
                        board[i, j] = new CheckClass(i, j, mainForm);

            for (int i = 3; i < 5; i++)
                for (int j = 0; j < 8; j++)
                    board[i, j] = new CheckClass(i, j, mainForm);

            for (int i = 5; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if ((i + j) % 2 != 0)
                        board[i, j] = new CheckClass(i, j, ColorCheck.white, mainForm);
                    else
                        board[i, j] = new CheckClass(i, j, mainForm);

            mf.logClass.WriteLine("Инициализация ActionClass");
            //ReDestination(selectCell, -1, -1);
        }

        /// <summary>
        /// При остановке игры надо провести данные обнуления для корректного начала следующей
        /// </summary>
        public void SetEnd()
        {
            mf.logClass.WriteLine("SetEnd");
            HideAvalable(selectCell[0], selectCell[1]);
            HideMustGo(selectCell);
            forbidSelect = false;
            selectCell = new int[2] { -1, -1 };
        }

        #region Поиск доступных полей для всех шашек
        /// <summary>
        /// Поиск полей для дамки (так как два одинаковых блока)
        /// coorse = 1 - не проверять вправо вниз
        /// coorse = 2 - не проверять влево вниз
        /// coorse = 3 - не проверять вправо вверх
        /// coorse = 4 - не проверять влево вверх
        /// </summary>
        /// <param name="colCheck"></param>
        /// <param name="ch"></param>
        /// <param name="coorse"></param>
        /// <returns></returns>
        private int CheckKing(ColorCheck colCheck, CheckClass ch, int coorse)
        {
            int[] xy;
            int flag = 0,
                    flagEat = 0;
            int saveX,
                    saveY,
                    x1,
                    y1,
                    saveFlag;
            // Проверка вправо вниз
            if (coorse != 1)
            {
                saveX = ch.X + 1;
                saveY = ch.Y + 1;
                while (saveX < 8 && saveY < 8 && board[saveX, saveY].dest == null)
                {
                    xy = new int[4] { 1, saveX, saveY, 0 };
                    destArray.Add(xy);
                    saveX += 1;
                    saveY += 1;
                }
                if (saveX < 7 && saveY < 7 && board[saveX, saveY].colorCheck == colCheck && board[saveX + 1, saveY + 1].dest == null)
                {
                    saveX += 1;
                    saveY += 1;
                    flagEat = 1;

                    // Проверка на то, можно ли съесть ещё одну шашку на одной прямой со съеденной
                    x1 = saveX;
                    y1 = saveY;
                    saveFlag = 1;
                    while (x1 < 7 && y1 < 7 && board[x1, y1].dest == null)
                    {
                        x1++;
                        y1++;
                    }
                    if (x1 < 7 && y1 < 7 && board[x1, y1].colorCheck == colCheck && board[x1 + 1, y1 + 1].dest == null)
                        saveFlag = 2;

                    while (saveX < 8 && saveY < 8 && board[saveX, saveY].dest == null)
                    {
                        flag = saveFlag;

                        // Поиск вправо вверх и влево вниз шашки, которую можно съесть
                        x1 = saveX - 1;
                        y1 = saveY + 1;
                        while (x1 > 1 && y1 < 6 && board[x1, y1].dest == null)
                        {
                            x1 -= 1;
                            y1 += 1;
                        }
                        if (x1 > 0 && y1 < 7 && board[x1, y1].colorCheck == colCheck && board[x1 - 1, y1 + 1].dest == null)
                            flag = 2;
                        else
                        {
                            // влево вниз, если вправо вверх ничего не нашли
                            x1 = saveX + 1;
                            y1 = saveY - 1;
                            while (x1 < 6 && y1 > 1 && board[x1, y1].dest == null)
                            {
                                x1 += 1;
                                y1 -= 1;
                            }
                            if (x1 < 7 && y1 > 0 && board[x1, y1].colorCheck == colCheck && board[x1 + 1, y1 - 1].dest == null)
                                flag = 2;
                        }

                        xy = new int[4] { 1, saveX, saveY, flag };
                        destArray.Add(xy);
                        saveX += 1;
                        saveY += 1;
                    }
                }
            }
            // Проверка влево вниз
            if (coorse != 2)
            {
                saveX = ch.X + 1;
                saveY = ch.Y - 1;
                while (saveX < 8 && saveY > -1 && board[saveX, saveY].dest == null)
                {
                    xy = new int[4] { 2, saveX, saveY, 0 };
                    destArray.Add(xy);
                    saveX += 1;
                    saveY -= 1;
                }
                if (saveX < 7 && saveY > 0 && board[saveX, saveY].colorCheck == colCheck && board[saveX + 1, saveY - 1].dest == null)
                {
                    saveX += 1;
                    saveY -= 1;
                    flagEat = 1;

                    // Проверка на то, можно ли съесть ещё одну шашку на одной прямой со съеденной
                    x1 = saveX;
                    y1 = saveY;
                    saveFlag = 1;
                    while (x1 < 7 && y1 > 0 && board[x1, y1].dest == null)
                    {
                        x1++;
                        y1--;
                    }
                    if (x1 < 7 && y1 > 0 && board[x1, y1].colorCheck == colCheck && board[x1 + 1, y1 - 1].dest == null)
                        saveFlag = 2;

                    while (saveX < 8 && saveY > -1 && board[saveX, saveY].dest == null)
                    {
                        flag = saveFlag;

                        // Поиск поиск влево вверх и вправо вниз шашки, которую можно съесть
                        x1 = saveX - 1;
                        y1 = saveY - 1;
                        while (x1 > 1 && y1 > 1 && board[x1, y1].dest == null)
                        {
                            x1 -= 1;
                            y1 -= 1;
                        }
                        if (x1 > 0 && y1 > 0 && board[x1, y1].colorCheck == colCheck && board[x1 - 1, y1 - 1].dest == null)
                            flag = 2;
                        else
                        {
                            // влево вниз, если вправо вверх ничего не нашли
                            x1 = saveX + 1;
                            y1 = saveY + 1;
                            while (x1 < 6 && y1 < 6 && board[x1, y1].dest == null)
                            {
                                x1 += 1;
                                y1 += 1;
                            }
                            if (x1 < 7 && y1 < 7 && board[x1, y1].colorCheck == colCheck && board[x1 + 1, y1 + 1].dest == null)
                                flag = 2;
                        }

                        xy = new int[4] { 2, saveX, saveY, flag };
                        destArray.Add(xy);
                        saveX += 1;
                        saveY -= 1;
                    }
                }
            }
            // Проверка вправо вверх
            if (coorse != 3)
            {
                saveX = ch.X - 1;
                saveY = ch.Y + 1;
                while (saveX > -1 && saveY < 8 && board[saveX, saveY].dest == null)
                {
                    xy = new int[4] { 3, saveX, saveY, 0 };
                    destArray.Add(xy);
                    saveX -= 1;
                    saveY += 1;
                }
                if (saveX > 0 && saveY < 7 && board[saveX, saveY].colorCheck == colCheck && board[saveX - 1, saveY + 1].dest == null)
                {
                    saveX -= 1;
                    saveY += 1;
                    flagEat = 1;

                    // Проверка на то, можно ли съесть ещё одну шашку на одной прямой со съеденной
                    x1 = saveX;
                    y1 = saveY;
                    saveFlag = 1;
                    while (x1 > 0 && y1 < 7 && board[x1, y1].dest == null)
                    {
                        x1--;
                        y1++;
                    }
                    if (x1 > 0 && y1 < 7 && board[x1, y1].colorCheck == colCheck && board[x1 - 1, y1 + 1].dest == null)
                        saveFlag = 2;

                    while (saveX > -1 && saveY < 8 && board[saveX, saveY].dest == null)
                    {
                        flag = saveFlag;

                        // Поиск поиск вправо вниз и влево вверх шашки, которую можно съесть
                        x1 = saveX - 1;
                        y1 = saveY - 1;
                        while (x1 > 1 && y1 > 1 && board[x1, y1].dest == null)
                        {
                            x1 -= 1;
                            y1 -= 1;
                        }
                        if (x1 > 0 && y1 > 0 && board[x1, y1].colorCheck == colCheck && board[x1 - 1, y1 - 1].dest == null)
                            flag = 2;
                        else
                        {
                            // вправо вниз, если влево вверх ничего не нашли
                            x1 = saveX + 1;
                            y1 = saveY + 1;
                            while (x1 < 6 && y1 < 6 && board[x1, y1].dest == null)
                            {
                                x1 += 1;
                                y1 += 1;
                            }
                            if (x1 < 7 && y1 < 7 && board[x1, y1].colorCheck == colCheck && board[x1 + 1, y1 + 1].dest == null)
                                flag = 2;
                        }

                        xy = new int[4] { 3, saveX, saveY, flag };
                        destArray.Add(xy);
                        saveX -= 1;
                        saveY += 1;
                    }
                }
            }
            // Проверка влево вверх	
            if (coorse != 4)
            {
                saveX = ch.X - 1;
                saveY = ch.Y - 1;
                while (saveX > -1 && saveY > -1 && board[saveX, saveY].dest == null)
                {
                    xy = new int[4] { 4, saveX, saveY, 0 };
                    destArray.Add(xy);
                    saveX -= 1;
                    saveY -= 1;
                }
                if (saveX > 0 && saveY > 0 && board[saveX, saveY].colorCheck == colCheck && board[saveX - 1, saveY - 1].dest == null)
                {
                    saveX -= 1;
                    saveY -= 1;
                    flagEat = 1;

                    // Проверка на то, можно ли съесть ещё одну шашку на одной прямой со съеденной
                    x1 = saveX;
                    y1 = saveY;
                    saveFlag = 1;
                    while (x1 > 0 && y1 > 0 && board[x1, y1].dest == null)
                    {
                        x1--;
                        y1--;
                    }
                    if (x1 > 0 && y1 > 0 && board[x1, y1].colorCheck == colCheck && board[x1 - 1, y1 - 1].dest == null)
                        saveFlag = 2;

                    while (saveX > -1 && saveY > -1 && board[saveX, saveY].dest == null)
                    {
                        flag = saveFlag;

                        // Поиск поиск вправо вверх и влево вниз шашки, которую можно съесть
                        x1 = saveX - 1;
                        y1 = saveY + 1;
                        while (x1 > 1 && y1 < 6 && board[x1, y1].dest == null)
                        {
                            x1 -= 1;
                            y1 += 1;
                        }
                        if (x1 > 0 && y1 < 7 && board[x1, y1].colorCheck == colCheck && board[x1 - 1, y1 + 1].dest == null)
                            flag = 2;
                        else
                        {
                            // влево вниз, если вправо вверх ничего не нашли
                            x1 = saveX + 1;
                            y1 = saveY - 1;
                            while (x1 < 6 && y1 > 1 && board[x1, y1].dest == null)
                            {
                                x1 += 1;
                                y1 -= 1;
                            }
                            if (x1 < 7 && y1 > 0 && board[x1, y1].colorCheck == colCheck && board[x1 + 1, y1 - 1].dest == null)
                                flag = 2;
                        }

                        xy = new int[4] { 4, saveX, saveY, flag };
                        destArray.Add(xy);
                        saveX -= 1;
                        saveY -= 1;
                    }
                }
            }
            return flagEat;
        }

        /// <summary>
        /// Поиск полей для шашки (так как два одинаковых блока)
        /// </summary>
        /// <param name="colCheck"></param>
        /// <param name="ch"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private int CheckCheck(ColorCheck colCheck, CheckClass ch, int i, int j)
        {
            int[] xy;
            int flag = 0;
            // Проверка на наличие шашек другого цвета рядом с шашкой (и чтобы за ними ничего не было)
            if (ch.Y < 6 && ch.X < 6 && board[i + 1, j + 1].dest != null && board[i + 1, j + 1].colorCheck == colCheck && board[i + 2, j + 2].dest == null)
            {
                xy = new int[4] { 0, i + 2, j + 2, 1 };
                destArray.Add(xy);
                flag = 1;
            }
            if (ch.Y > 1 && ch.X < 6 && board[i + 1, j - 1].dest != null && board[i + 1, j - 1].colorCheck == colCheck && board[i + 2, j - 2].dest == null)
            {
                xy = new int[4] { 0, i + 2, j - 2, 1 };
                destArray.Add(xy);
                flag = 1;
            }
            if (ch.Y < 6 && ch.X > 1 && board[i - 1, j + 1].dest != null && board[i - 1, j + 1].colorCheck == colCheck && board[i - 2, j + 2].dest == null)
            {
                xy = new int[4] { 0, i - 2, j + 2, 1 };
                destArray.Add(xy);
                flag = 1;
            }
            if (ch.Y > 1 && ch.X > 1 && board[i - 1, j - 1].dest != null && board[i - 1, j - 1].colorCheck == colCheck && board[i - 2, j - 2].dest == null)
            {
                xy = new int[4] { 0, i - 2, j - 2, 1 };
                destArray.Add(xy);
                flag = 1;
            }
            return flag;
        }

        /// <summary>
        /// Поиск полей, в которые может сходить каждая конкретная шашка
        /// </summary>
        /// <param name="fromCell">Поле, откуда ходила шашка</param>
        /// <param name="x">X-координата поля, куда пошла шашка</param>
        /// <param name="y">Y-координата поля, куда пошла шашка</param>
        public void ReDestination(int x, int y)
        {
            int[] xy;
            int flag;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (board[i, j].dest != null)
                    {
                        CheckClass ch = board[i, j];
                        destArray = new ArrayList();
                        if (ch.colorCheck == ColorCheck.black)						// Проверка для чёрных шашек
                        {
                            if (ch.typeCheck == TypeCheck.check)			// Проверка для шашки
                            {
                                flag = CheckCheck(ColorCheck.white, ch, i, j);

                                if (flag == 0)
                                {
                                    // Проверка на пустые поля перед шашкой справа и слева
                                    if (ch.Y < 7 && board[i + 1, j + 1].dest == null)
                                    {
                                        xy = new int[4] { 0, i + 1, j + 1, 0 };
                                        destArray.Add(xy);
                                    }
                                    if (ch.Y > 0 && board[i + 1, j - 1].dest == null)
                                    {
                                        xy = new int[4] { 0, i + 1, j - 1, 0 };
                                        destArray.Add(xy);
                                    }
                                }
                            }
                            else																			// Проверка для дамки							
                            {
                                int coorse = 0;
                                if (x == i && y == j)
                                {
                                    if (selectCell[0] < x && selectCell[1] < y)
                                        coorse = 4;
                                    else if (selectCell[0] < x && selectCell[1] > y)
                                        coorse = 3;
                                    else if (selectCell[0] > x && selectCell[1] < y)
                                        coorse = 2;
                                    else
                                        coorse = 1;
                                }
                                flag = CheckKing(ColorCheck.white, ch, coorse);
                            }
                        }
                        else																						// Проверка для белых 
                        {
                            if (board[i, j].typeCheck == TypeCheck.check)			// Проверка для шашки
                            {
                                flag = CheckCheck(ColorCheck.black, ch, i, j);

                                if (flag == 0)
                                {
                                    // Проверка на пустые поля перед шашкой справа и слева
                                    if (ch.Y < 7 && board[i - 1, j + 1].dest == null)
                                    {
                                        xy = new int[4] { 0, i - 1, j + 1, 0 };
                                        destArray.Add(xy);
                                    }
                                    if (ch.Y > 0 && board[i - 1, j - 1].dest == null)
                                    {
                                        xy = new int[4] { 0, i - 1, j - 1, 0 };
                                        destArray.Add(xy);
                                    }
                                }

                            }
                            else																							// Проверка для дамки							
                            {
                                int coorse = 0;
                                if (x == i && y == j)
                                {
                                    if (selectCell[0] < x && selectCell[1] < y)
                                        coorse = 4;
                                    else if (selectCell[0] < x && selectCell[1] > y)
                                        coorse = 3;
                                    else if (selectCell[0] > x && selectCell[1] < y)
                                        coorse = 2;
                                    else
                                        coorse = 1;
                                }
                                flag = CheckKing(ColorCheck.black, ch, coorse);
                            }
                        }

                        int[,] mas = new int[destArray.Count, 2];
                        int cnt = 0;

                        if (flag > 0)
                            board[i, j].mustGo = true;
                        else
                            board[i, j].mustGo = false;

                        // Создание массива соответствий для направлений и флагов
                        int[] flags = new int[5];
                        for (int l = 0; l < destArray.Count; l++)
                        {
                            xy = (int[])destArray[l];
                            if (xy[3] > flags[xy[0]])
                                flags[xy[0]] = xy[3];
                        }

                        for (int l = 0; l < destArray.Count; l++)
                        {
                            xy = (int[])destArray[l];
                            if (flags[xy[0]] == xy[3] && xy[3] >= flag)
                            {
                                mas[cnt, 0] = xy[1];
                                mas[cnt, 1] = xy[2];
                                cnt++;
                            }
                        }

                        board[i, j].dest = new int[cnt, 2];
                        for (int l = 0; l < cnt; l++)
                        {
                            board[i, j].dest[l, 0] = mas[l, 0];
                            board[i, j].dest[l, 1] = mas[l, 1];
                        }
                    }
        }
        #endregion

        #region Выделение полей и шашек
        /// <summary>
        /// Показывает доступные для хода клетки для указанной шашки
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void ShowAvailable(int x, int y)
        {
            if (x != -1)
            {
                CheckClass ch = board[x, y];
                for (int i = 0; i < ch.dest.Length / 2; i++)
                    Select(ch.dest[i, 0], ch.dest[i, 1]);
            }
        }

        /// <summary>
        /// Удаляет все выделенные клетки для указанной шашки
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void HideAvalable(int x, int y)
        {
            if (x != -1)
            {
                CheckClass ch = board[x, y];
                for (int i = 0; i < board[x, y].dest.Length / 2; i++)
                    DeSelect(ch.dest[i, 0], ch.dest[i, 1]);
            }
        }
        // 
        /// <summary>
        /// Показывает доступные для хода клетки для всех фигур
        /// </summary>
        private void ShowMustGo()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (board[i, j].mustGo == true && board[i, j].colorCheck == whoMove)
                        SelectMustGo(i, j);
        }

        /// <summary>
        /// Удаляет все выделенные клетки для указанной шашки
        /// </summary>
        /// <param name="xy"></param>
        private void HideMustGo(int[] xy)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (board[i, j].mustGo == true && board[i, j].colorCheck == whoMove)
                        if (xy[0] == i && xy[1] == j)
                            Select(i, j);
                        else
                            DeSelect(i, j);
        }

        /// <summary>
        /// Выделение поля при щелчке мыши
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void Select(int x, int y)
        {
            if (x < 0 || x > 7 || y < 0 || y > 7)
                return;

            if (board[x, y].colorCheck == ColorCheck.black)
                if (board[x, y].typeCheck == TypeCheck.check)
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_black_select;
                else
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_dam_black_select;
            else if (board[x, y].colorCheck == ColorCheck.white)
                if (board[x, y].typeCheck == TypeCheck.check)
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_white_select;
                else
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_dam_white_select;
            else
                board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.full_select;
        }

        /// <summary>
        /// Выделение шашки, которой надо сходить
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SelectMustGo(int x, int y)
        {
            if (board[x, y].colorCheck == ColorCheck.black)
                if (board[x, y].typeCheck == TypeCheck.check)
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_black_show;
                else
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_dam_black_show;
            else
                if (board[x, y].typeCheck == TypeCheck.check)
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_white_show;
                else
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_dam_white_show;
        }

        /// <summary>
        /// Снятие выделения с поля
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void DeSelect(int x, int y)
        {
            if (x < 0 || x > 7 || y < 0 || y > 7)
                return;

            if (board[x, y].colorCheck == ColorCheck.black)
                if (board[x, y].typeCheck == TypeCheck.check)
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_black;
                else
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_dam_black;
            else if (board[x, y].colorCheck == ColorCheck.white)
                if (board[x, y].typeCheck == TypeCheck.check)
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_white;
                else
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_dam_white;
            else
                board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.full;
        }
        #endregion

        /// <summary>
        ///	Проверка на конец игры 
        /// </summary>
        /// <returns></returns>
        public bool IsEnd()
        {
            int cnt = 0;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (board[i, j].colorCheck == whoMove)
                        cnt += board[i, j].dest.Length / 2;
            if (cnt == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Получение типа шашки в данной клетке
        /// </summary>
        /// <param name="cell">Фигура для рассмотрения</param>
        /// <returns></returns>
        private ObjectCheck GetObjectCheck(CheckClass cell)
        {
            if (cell.colorCheck == ColorCheck.black)
                if (cell.typeCheck == TypeCheck.check)
                    return ObjectCheck.check_black;
                else if (cell.typeCheck == TypeCheck.king)
                    return ObjectCheck.check_black_dam;
                else
                    return ObjectCheck.empty;
            else if (cell.colorCheck == ColorCheck.white)
                if (cell.typeCheck == TypeCheck.check)
                    return ObjectCheck.check_white;
                else if (cell.typeCheck == TypeCheck.king)
                    return ObjectCheck.check_white_dam;
                else
                    return ObjectCheck.empty;
            else
                return ObjectCheck.empty;
        }

        /// <summary>
        /// Произведение хода и изменение информации о шашках 
        /// </summary>
        /// <param name="fromCell">Координаты фигуры, которой ходим</param>
        /// <param name="x">Х-координата поля, куда ходим</param>
        /// <param name="y">Y-координата поля, куда ходим</param>
        public void MoveCheck(int[] fromCell, int x, int y)
        {
            mf.logClass.WriteLine("MoveCheck fromCell={" + fromCell[0] + fromCell[1] + "} x=" + x.ToString() + " y=" + y.ToString());
            DeSelect(fromCell[0], fromCell[1]);

            ObjectCheck moveCheck = GetObjectCheck(board[fromCell[0], fromCell[1]]);

            TypeCheck tc = board[x, y].typeCheck;
            board[x, y].typeCheck = board[fromCell[0], fromCell[1]].typeCheck;
            board[fromCell[0], fromCell[1]].typeCheck = tc;

            ColorCheck cc = board[x, y].colorCheck;
            board[x, y].colorCheck = board[fromCell[0], fromCell[1]].colorCheck;
            board[fromCell[0], fromCell[1]].colorCheck = cc;

            Image img = board[x, y].pictureBox.Image;
            board[x, y].pictureBox.Image = board[fromCell[0], fromCell[1]].pictureBox.Image;
            board[fromCell[0], fromCell[1]].pictureBox.Image = img;

            board[x, y].dest = new int[0, 2];
            board[fromCell[0], fromCell[1]].dest = null;

            if ((board[x, y].colorCheck == ColorCheck.black && x == 7) ||
                    (board[x, y].colorCheck == ColorCheck.white && x == 0))
            {
                board[x, y].typeCheck = TypeCheck.king;
                if (board[x, y].colorCheck == ColorCheck.white)
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_dam_white;
                else
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_dam_black;
            }
            int x1, y1, x2, y2;				// Удаляем шашку, которую съели, если она есть

            if (fromCell[0] > x)
            {
                x1 = x;
                y1 = y;
                x2 = fromCell[0];
                y2 = fromCell[1];
            }
            else
            {
                x2 = x;
                y2 = y;
                x1 = fromCell[0];
                y1 = fromCell[1];
            }

            bool flag = false;
            int w = y1;
            int delX = -1, delY = -1;
            ObjectCheck delCheck = ObjectCheck.empty;
            //ObjectCheck moveCheckDest = GetObjectCheck(board[x, y]);		
            for (int q = x1 + 1; q < x2; q++)
            {
                if (y1 > y2)
                    w--;
                else
                    w++;
                if (board[q, w].dest != null)
                {
                    delX = q;
                    delY = w;
                    delCheck = GetObjectCheck(board[q, w]);
                    board[q, w].dest = null;
                    board[q, w].mustGo = false;
                    board[q, w].pictureBox.Image = global::Checkers.Properties.Resources.full;
                    board[q, w].typeCheck = TypeCheck.empty;
                    board[q, w].colorCheck = ColorCheck.unknown;
                    flag = true;
                    break;
                }
            }

            // Поиск новых доступных полей для шашек
            if (flag)
                ReDestination(x, y);
            else
                ReDestination(-1, -1);

            if (flag && board[x, y].mustGo)
            {
                mf.AfterStep(whoMove, fromCell[0], fromCell[1], x, y, moveCheck, /*moveCheckDest,*/ delX, delY, delCheck, true);
                Select(x, y);
                selectCell = new int[2] { x, y };
                forbidSelect = true;
            }
            else
            {
                mf.AfterStep(whoMove, fromCell[0], fromCell[1], x, y, moveCheck, /*moveCheckDest,*/ delX, delY, delCheck, false);
                selectCell = new int[2] { -1, -1 };
                if (whoMove == ColorCheck.white)
                {
                    whoMove = ColorCheck.black;
                    mf.pictureBoxWhite.Visible = false;
                    mf.pictureBoxBlack.Visible = true;
                }
                else
                {
                    whoMove = ColorCheck.white;
                    mf.pictureBoxWhite.Visible = true;
                    mf.pictureBoxBlack.Visible = false;
                }
                forbidSelect = false;


                // Проверка на конец игры (если у кого-то цвета больше нет возможностей сходить - 
                // а следующий ход его - то он проиграл)
                if (IsEnd())
                    mf.SetEnd(whoMove);
            }
        }

        public void Press(int x, int y)
        {
            Press(x, y, false);
        }

        public delegate void PressDelegate(int x, int y, bool autoCall);
        /// <summary>
        /// Обработка нажатия на клетку в поле
        /// </summary>
        /// <param name="x">Х-координата поля</param>
        /// <param name="y">Y-координата поля</param>
        public void Press(int x, int y, bool autoCall)
        {
            if (mf.isEnd)
            {
                if (!autoCall)
                {
                    MessageBox.Show("Для начала игры нажмите на кнопку 'Старт игры'", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }

            if (freezeGame)
                return;

            if (!mf.serverReady || !mf.clientReady)
                return;

            if ((mf.typePlayer == TypePlayer.client && whoMove == ColorCheck.white) ||
                    (mf.typePlayer == TypePlayer.server && whoMove == ColorCheck.black))
                return;

            if ((x + y) % 2 == 0 || (board[x, y].colorCheck != ColorCheck.unknown && whoMove != board[x, y].colorCheck))
                return;

            if (mf.typePlayer == TypePlayer.local && mf.typeOrder != OrderGame.UserUser)
            {
                if (!autoCall)
                {
                    if (mf.typeOrder == OrderGame.CompComp)
                        return;
                    if (mf.typeOrder == OrderGame.UserComp && whoMove == ColorCheck.black)
                        return;
                    if (mf.typeOrder == OrderGame.CompUser && whoMove == ColorCheck.white)
                        return;
                }
            }

            mf.logClass.WriteLine("Press x=" + x.ToString() + " y=" + y.ToString() + " autoCall=" + autoCall.ToString());
            HideAvalable(selectCell[0], selectCell[1]);
            HideMustGo(selectCell);
            if (board[x, y].dest != null)			// Выделение шашки
            {
                if (forbidSelect)
                {
                    ShowAvailable(selectCell[0], selectCell[1]);
                    return;
                }

                destArray = new ArrayList();
                int[] xy;
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                        if (board[i, j].mustGo == true && board[i, j].colorCheck == whoMove)
                        {
                            xy = new int[2] { i, j };
                            destArray.Add(xy);
                        }

                if (destArray.Count > 0)
                {
                    if (board[x, y].dest != null)	// Проверяем, какую шашку хочет
                    {													// выделить пользователь. Если не ту - то не даём выделять
                        bool flag = false;
                        for (int i = 0; i < destArray.Count; i++)
                        {
                            xy = (int[])destArray[i];
                            if (xy[0] == x && xy[1] == y)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            selectCell = new int[2] { -1, -1 };
                            ShowMustGo();
                            return;
                        }
                        else
                        {
                            DeSelect(selectCell[0], selectCell[1]);
                            Select(x, y);
                            selectCell = new int[2] { x, y };
                        }
                    }
                }
                else
                {
                    DeSelect(selectCell[0], selectCell[1]);
                    Select(x, y);
                    selectCell = new int[2] { x, y };
                }
            }
            else																// Ходим шашкой
            {
                if (selectCell[0] == -1)
                    return;

                CheckClass ch = board[selectCell[0], selectCell[1]];
                for (int i = 0; i < ch.dest.Length / 2; i++)
                    if (ch.dest[i, 0] == x && ch.dest[i, 1] == y)		// Делаем ход
                    {
                        // Отсылка сообщения о сделанном ходе по сети
                        try
                        {
                            if (mf.typePlayer == TypePlayer.client)
                                mf.fromClient.ToServerStep(selectCell, x, y, mf.timeBlack);
                            else if (mf.typePlayer == TypePlayer.server)
                                mf.fromServer.ToClientStep(selectCell, x, y, mf.timeWhite);
                        }
                        catch
                        {
                            MessageBox.Show("Соединение разорвано. Партия завершена.");
                            mf.button7_Click(new object(), new EventArgs());
                            return;
                        }
                        if (mf.typePlayer != TypePlayer.local)
                            mf.UpdateLastLinesBoxNetInfo("Ожидание хода противника.");

                        MoveCheck(selectCell, x, y);

                        return;
                    }

                // Показ доступных полей для хода
                ShowAvailable(selectCell[0], selectCell[1]);
            }
            Application.DoEvents();
        }

        /// <summary>
        ///	Перерисовка шашек в правильной ориентации 
        /// </summary>
        public void ReWriteCells()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!mf.changeSide)
                        board[i, j].pictureBox.Location = new Point(mf.nSize * j + mf.leftX, mf.nSize * i + mf.leftY);
                    else
                        board[i, j].pictureBox.Location = new Point(mf.nSize * (7 - j) + mf.leftX, mf.nSize * (7 - i) + mf.leftY);
                }
            }
        }

        /// <summary>
        /// Перевод цифры из массива с историей в тип шашки
        /// </summary>
        /// <param name="n">Цифры</param>
        /// <returns></returns>
        private ObjectCheck DigitToObjectCheck(string n)
        {
            switch (n)
            {
                case "1":
                    return ObjectCheck.check_black;
                case "2":
                    return ObjectCheck.check_black_dam;
                case "3":
                    return ObjectCheck.check_white;
                case "4":
                    return ObjectCheck.check_white_dam;
                default:
                    return ObjectCheck.empty;
            }
        }

        /// <summary>
        ///	Заполнение клетки нужной информацией в зависимости от типа поля 
        /// </summary>
        /// <param name="check">Тип поля</param>
        /// <param name="x">X-координата</param>
        /// <param name="y">Y-координата</param>
        public void SetDataToBoard(ObjectCheck check, int x, int y)
        {
            board[x, y].mustGo = false;
            if (check == ObjectCheck.check_black && x == 7)
                check = ObjectCheck.check_black_dam;
            else if (check == ObjectCheck.check_white && x == 0)
                check = ObjectCheck.check_white_dam;

            switch (check)
            {
                case ObjectCheck.check_black:
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_black;
                    board[x, y].typeCheck = TypeCheck.check;
                    board[x, y].colorCheck = ColorCheck.black;
                    board[x, y].dest = new int[0, 2];
                    break;
                case ObjectCheck.check_black_dam:
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_dam_black;
                    board[x, y].typeCheck = TypeCheck.king;
                    board[x, y].colorCheck = ColorCheck.black;
                    board[x, y].dest = new int[0, 2];
                    break;
                case ObjectCheck.check_white:
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_white;
                    board[x, y].typeCheck = TypeCheck.check;
                    board[x, y].colorCheck = ColorCheck.white;
                    board[x, y].dest = new int[0, 2];
                    break;
                case ObjectCheck.check_white_dam:
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.check_dam_white;
                    board[x, y].typeCheck = TypeCheck.king;
                    board[x, y].colorCheck = ColorCheck.white;
                    board[x, y].dest = new int[0, 2];
                    break;
                case ObjectCheck.full:
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.full;
                    board[x, y].typeCheck = TypeCheck.empty;
                    board[x, y].colorCheck = ColorCheck.unknown;
                    board[x, y].dest = null;
                    break;
                default:
                    board[x, y].pictureBox.Image = global::Checkers.Properties.Resources.empty;
                    board[x, y].typeCheck = TypeCheck.empty;
                    board[x, y].colorCheck = ColorCheck.unknown;
                    board[x, y].dest = null;
                    break;
            }
        }

        /// <summary>
        ///	Отмена хода  
        /// </summary>
        public bool BackStep()
        {
            if (forbidSelect)
                return false;

            mf.logClass.WriteLine("BackStep");
            DeSelect(selectCell[0], selectCell[1]);
            selectCell = new int[2] { -1, -1 };
            string[] strSave = (string[])mf.historyMas[mf.historyMas.Count - 1];
            string[] str = new string[4] { strSave[0], strSave[2], strSave[3], strSave[5] };

            int x1, y1, x2, y2;
            ObjectCheck moveCheck = ObjectCheck.empty;
            if (str[2] != "")
            {
                moveCheck = DigitToObjectCheck(str[2].Substring(0, 1));
                x1 = Convert.ToInt32(str[2].Substring(2, 1));
                y1 = Convert.ToInt32(str[2].Substring(4, 1));
                x2 = Convert.ToInt32(str[2].Substring(str[2].Length - 3, 1));
                y2 = Convert.ToInt32(str[2].Substring(str[2].Length - 1, 1));

                while (str[3].Length > 0)
                {
                    int x3, y3;
                    ObjectCheck delCheck = ObjectCheck.empty;
                    x3 = Convert.ToInt32(str[3].Substring(0, 1));
                    y3 = Convert.ToInt32(str[3].Substring(2, 1));
                    delCheck = DigitToObjectCheck(str[3].Substring(4, 1));
                    SetDataToBoard(delCheck, x3, y3);
                    if (str[3].Length > 5)
                        str[3] = str[3].Substring(6);
                    else
                        str[3] = str[3].Substring(5);
                }
            }
            else
            {
                moveCheck = DigitToObjectCheck(str[0].Substring(0, 1));
                x1 = Convert.ToInt32(str[0].Substring(2, 1));
                y1 = Convert.ToInt32(str[0].Substring(4, 1));
                x2 = Convert.ToInt32(str[0].Substring(str[0].Length - 3, 1));
                y2 = Convert.ToInt32(str[0].Substring(str[0].Length - 1, 1));
                while (str[1].Length > 0)
                {
                    int x3, y3;
                    ObjectCheck delCheck = ObjectCheck.empty;
                    x3 = Convert.ToInt32(str[1].Substring(0, 1));
                    y3 = Convert.ToInt32(str[1].Substring(2, 1));
                    delCheck = DigitToObjectCheck(str[1].Substring(4, 1));
                    SetDataToBoard(delCheck, x3, y3);
                    if (str[1].Length > 5)
                        str[1] = str[1].Substring(6);
                    else
                        str[1] = str[1].Substring(5);
                }
            }
            SetDataToBoard(ObjectCheck.full, x2, y2);
            SetDataToBoard(moveCheck, x1, y1);

            if (whoMove == ColorCheck.white)
                whoMove = ColorCheck.black;
            else
                whoMove = ColorCheck.white;
            ReDestination(-1, -1);
            return true;
        }
    }
}
