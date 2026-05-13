using System;
using System.Collections.Generic;

namespace TicTacToe
{
    public static class InvestigateFieldClassNew
    {
        /// <summary>
        /// Поиск одного выигрывающего хода
        /// </summary>
        /// <param name="field">Поле с объектами</param>
        /// <param name="whoStep">За кого искать выигрыш</param>
        /// <returns></returns>
        public static int[] FindOneWinStep(ObjectType[,] field, ObjectType whoStep)
        {
            var rowsCnt = field.GetLength(0);
            var columnsCnt = field.GetLength(1);

            int emptyCnt = 0;
            // Поиск линии из 5 одинаковых объектов в ряд, предполагая что мы ставим наш объект в пустую клетку
            // Возвращаем эту клетку как результат
            for (int i = 0; i < rowsCnt; i++)
            {
                for (int j = 0; j < columnsCnt; j++)
                {
                    if (field[i, j] != ObjectType.Empty)
                    {
                        continue;
                    }

                    emptyCnt++;

                    // Ищем, сколько подряд по вертикали
                    int len = 1;
                    int ci1 = i - 1;
                    int cj1 = j;
                    while (ci1 >= 0 && field[ci1, cj1] == whoStep)
                    {
                        ci1--;
                        len++;
                    }

                    int ci2 = i + 1;
                    int cj2 = j;
                    while (ci2 < rowsCnt && field[ci2, cj2] == whoStep)
                    {
                        ci2++;
                        len++;
                    }

                    if (len > 4)
                    {
                        return new[] { i, j };
                    }

                    // Ищем, сколько подряд по горизонтали
                    len = 1;
                    ci1 = i;
                    cj1 = j - 1;
                    while (cj1 >= 0 && field[ci1, cj1] == whoStep)
                    {
                        cj1--;
                        len++;
                    }

                    ci2 = i;
                    cj2 = j + 1;
                    while (cj2 < columnsCnt && field[ci2, cj2] == whoStep)
                    {
                        cj2++;
                        len++;
                    }

                    if (len > 4)
                    {
                        return new[] { i, j };
                    }

                    // Ищем, сколько подряд наискосок с левого верха до правого низа
                    len = 1;
                    ci1 = i - 1;
                    cj1 = j - 1;
                    while (ci1 >= 0 && cj1 >= 0 && field[ci1, cj1] == whoStep)
                    {
                        ci1--;
                        cj1--;
                        len++;
                    }

                    ci2 = i + 1;
                    cj2 = j + 1;
                    while (ci2 < rowsCnt && cj2 < columnsCnt && field[ci2, cj2] == whoStep)
                    {
                        ci2++;
                        cj2++;
                        len++;
                    }

                    if (len > 4)
                    {
                        return new[] { i, j };
                    }

                    // Ищем, сколько подряд наискосок с левого низа до правого верха
                    len = 1;
                    ci1 = i - 1;
                    cj1 = j + 1;
                    while (ci1 >= 0 && cj1 < columnsCnt && field[ci1, cj1] == whoStep)
                    {
                        ci1--;
                        cj1++;
                        len++;
                    }

                    ci2 = i + 1;
                    cj2 = j - 1;
                    while (ci2 < rowsCnt && cj2 >= 0 && field[ci2, cj2] == whoStep)
                    {
                        ci2++;
                        cj2--;
                        len++;
                    }

                    if (len > 4)
                    {
                        return new[] { i, j };
                    }
                }
            }

            if (emptyCnt == 0)
            {
                throw new Exception("Клетки закончились.");
            }

            return new int[0];
        }

        /// <summary>
        /// Поиск хода выстраивающего линию 4 в ряд с пустыми краями
        /// </summary>
        /// <param name="field">Поле с объектами</param>
        /// <param name="whoStep">За кого искать выигрыш</param>
        /// <returns></returns>
        public static List<int[]> FindFourInLineWinStep(ObjectType[,] field, ObjectType whoStep)
        {
            var result = new List<int[]>();

            var rowsCnt = field.GetLength(0);
            var columnsCnt = field.GetLength(1);

            // Поиск линии из 4 одинаковых объектов в ряд с пустыми клетками по краям,
            // предполагая что мы ставим наш объект в пустую клетку
            // Возвращаем эту клетку как результат
            for (int i = 0; i < rowsCnt; i++)
            {
                for (int j = 0; j < columnsCnt; j++)
                {
                    if (field[i, j] != ObjectType.Empty)
                    {
                        continue;
                    }

                    // Ищем, сколько подряд по вертикали
                    int len = 1;
                    int ci1 = i - 1;
                    int cj1 = j;
                    while (ci1 >= 0 && field[ci1, cj1] == whoStep)
                    {
                        ci1--;
                        len++;
                    }

                    int ci2 = i + 1;
                    int cj2 = j;
                    while (ci2 < rowsCnt && field[ci2, cj2] == whoStep)
                    {
                        ci2++;
                        len++;
                    }

                    if (len == 4 && ci1 >= 0 && ci2 < rowsCnt && field[ci1, cj1] == ObjectType.Empty && field[ci2, cj2] == ObjectType.Empty)
                    {
                        result.Add( new[] { i, j });
                    }

                    // Ищем, сколько подряд по горизонтали
                    len = 1;
                    ci1 = i;
                    cj1 = j - 1;
                    while (cj1 >= 0 && field[ci1, cj1] == whoStep)
                    {
                        cj1--;
                        len++;
                    }

                    ci2 = i;
                    cj2 = j + 1;
                    while (cj2 < columnsCnt && field[ci2, cj2] == whoStep)
                    {
                        cj2++;
                        len++;
                    }

                    if (len == 4 && cj1 >= 0 && cj2 < columnsCnt && field[ci1, cj1] == ObjectType.Empty && field[ci2, cj2] == ObjectType.Empty)
                    {
                        result.Add(new[] { i, j });
                    }

                    // Ищем, сколько подряд наискосок с левого верха до правого низа
                    len = 1;
                    ci1 = i - 1;
                    cj1 = j - 1;
                    while (ci1 >= 0 && cj1 >= 0 && field[ci1, cj1] == whoStep)
                    {
                        ci1--;
                        cj1--;
                        len++;
                    }

                    ci2 = i + 1;
                    cj2 = j + 1;
                    while (ci2 < rowsCnt && cj2 < columnsCnt && field[ci2, cj2] == whoStep)
                    {
                        ci2++;
                        cj2++;
                        len++;
                    }

                    if (len == 4 && ci1 >= 0 && cj1 >= 0 && ci2 < rowsCnt && cj2 < columnsCnt &&
                        field[ci1, cj1] == ObjectType.Empty && field[ci2, cj2] == ObjectType.Empty)
                    {
                        result.Add(new[] { i, j });
                    }

                    // Ищем, сколько подряд наискосок с левого низа до правого верха
                    len = 1;
                    ci1 = i - 1;
                    cj1 = j + 1;
                    while (ci1 >= 0 && cj1 < columnsCnt && field[ci1, cj1] == whoStep)
                    {
                        ci1--;
                        cj1++;
                        len++;
                    }

                    ci2 = i + 1;
                    cj2 = j - 1;
                    while (ci2 < rowsCnt && cj2 >= 0 && field[ci2, cj2] == whoStep)
                    {
                        ci2++;
                        cj2--;
                        len++;
                    }

                    if (len == 4 && ci1 >= 0 && cj1 < columnsCnt && ci2 < rowsCnt && cj2 >= 0 &&
                        field[ci1, cj1] == ObjectType.Empty && field[ci2, cj2] == ObjectType.Empty)
                    {
                        result.Add(new[] { i, j });
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Найти повторяющиеся атакующие ходы в позиции для разных линий. Это будут победные ходы на пересечении линий
        /// </summary>
        /// <param name="attackSteps">Атакующие ходы в позиции</param>
        /// <returns></returns>
        public static List<int[]> FindCrossThreeInLineWinStep(List<int[]> attackSteps)
        {
            var result = new List<int[]>();

            for (int i = 0; i < attackSteps.Count - 1; i++)
            {
                for (int j = i + 1; j < attackSteps.Count; j++)
                {
                    if (attackSteps[i][0] == attackSteps[j][0] && attackSteps[i][1] == attackSteps[j][1])
                    {
                        result.Add(attackSteps[i]);
                    }
                }
            }

            return result;
        }

        public static List<int[]> FindThreeInLineAttackSteps(ObjectType[,] field, ObjectType whoStep)
        {
            var rowsCnt = field.GetLength(0);
            var columnsCnt = field.GetLength(1);
            var steps = new List<int[]>();

            // Поиск всех ходов создающих атакующую линию из 3 или 4 одинаковых объектов (но не победную)
            for (int i = 0; i < rowsCnt; i++)
            {
                for (int j = 0; j < columnsCnt; j++)
                {
                    if (field[i, j] != whoStep)
                    {
                        continue;
                    }

                    #region Horizontal checks
                    // Ищем атакующие ходы для двух одинаковых рядом лежащих объектов с пустыми клетками по боками
                    if (j - 1 >= 0 && j + 2 < columnsCnt && field[i, j + 1] == whoStep && 
                        field[i, j - 1] == ObjectType.Empty && field[i, j + 2] == ObjectType.Empty)
                    {
                        if (j - 3 >= 0 && field[i, j - 2] == ObjectType.Empty && field[i, j - 3] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i, j - 2 });
                            steps.Add(new[] { i, j - 1 });
                        }
                        else if (j - 2 >= 0 && j + 3 < columnsCnt && field[i, j - 2] == ObjectType.Empty && field[i, j + 3] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i, j - 1 });
                        }

                        if (j + 4 < columnsCnt && field[i, j + 3] == ObjectType.Empty && field[i, j + 4] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i, j + 2 });
                            steps.Add(new[] { i, j + 3 });
                        }
                        else if (j - 2 >= 0 && j + 3 < columnsCnt && field[i, j - 2] == ObjectType.Empty && field[i, j + 3] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i, j + 2 });
                        }
                    }

                    // Ищем атакующие ходы для двух одинаковых объектов через пустую клетку с пустыми клетками по боками
                    if (j - 1 >= 0 && j + 3 < columnsCnt && field[i, j + 1] == ObjectType.Empty && field[i, j + 2] == whoStep && 
                        field[i, j + 3] == ObjectType.Empty && field[i, j - 1] == ObjectType.Empty)
                    {
                        if (j - 2 >= 0 && field[i, j - 2] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i, j - 1 });
                        }
                        if (j - 2 >= 0 && field[i, j - 2] == ObjectType.Empty ||
                            j + 4 < columnsCnt && field[i, j + 4] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i, j + 1 });
                        }
                        if (j + 4 < columnsCnt && field[i, j + 4] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i, j + 3 });
                        }
                    }

                    // Ищем атакующие ходы для трех одинаковых объектов после вражеского или начала поля
                    if (j + 4 < columnsCnt && field[i, j + 1] == whoStep && field[i, j + 2] == whoStep &&
                             field[i, j + 3] == ObjectType.Empty && field[i, j + 4] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i, j + 3 });
                        steps.Add(new[] { i, j + 4 });
                    }
                    else if (j - 1 >= 0 && j + 3 < columnsCnt && field[i, j + 1] == whoStep && field[i, j + 2] == whoStep &&
                             field[i, j - 1] == ObjectType.Empty && field[i, j + 3] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i, j - 1 });
                        steps.Add(new[] { i, j + 3 });
                    }
                    else if (j - 2 >= 0 && j + 2 < columnsCnt && field[i, j + 1] == whoStep && field[i, j + 2] == whoStep &&
                             field[i, j - 1] == ObjectType.Empty && field[i, j - 2] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i, j - 2 });
                        steps.Add(new[] { i, j - 1 });
                    }
                    else if (j + 4 < columnsCnt && field[i, j + 1] == ObjectType.Empty && field[i, j + 2] == whoStep &&
                             field[i, j + 3] == ObjectType.Empty && field[i, j + 4] == whoStep)
                    {
                        steps.Add(new[] { i, j + 1 });
                        steps.Add(new[] { i, j + 3 });
                    }
                    else if (j + 4 < columnsCnt && field[i, j + 1] == whoStep && field[i, j + 2] == ObjectType.Empty &&
                             field[i, j + 3] == ObjectType.Empty && field[i, j + 4] == whoStep)
                    {
                        steps.Add(new[] { i, j + 2 });
                        steps.Add(new[] { i, j + 3 });
                    }
                    else if (j + 4 < columnsCnt && field[i, j + 1] == whoStep && field[i, j + 2] == ObjectType.Empty &&
                             field[i, j + 3] == whoStep && field[i, j + 4] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i, j + 2 });
                        steps.Add(new[] { i, j + 4 });
                    }
                    else if (j + 4 < columnsCnt && field[i, j + 1] == ObjectType.Empty && field[i, j + 2] == whoStep &&
                             field[i, j + 3] == whoStep && field[i, j + 4] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i, j + 1 });
                        steps.Add(new[] { i, j + 4 });
                    }
                    else if (j + 4 < columnsCnt && field[i, j + 1] == ObjectType.Empty && field[i, j + 2] == ObjectType.Empty &&
                             field[i, j + 3] == whoStep && field[i, j + 4] == whoStep)
                    {
                        steps.Add(new[] { i, j + 1 });
                        steps.Add(new[] { i, j + 2 });
                    }
                    else if (j - 1 >= 0 && j + 3 < columnsCnt && field[i, j + 1] == ObjectType.Empty && field[i, j + 2] == whoStep &&
                             field[i, j - 1] == ObjectType.Empty && field[i, j + 3] == whoStep)
                    {
                        steps.Add(new[] { i, j - 1 });
                        steps.Add(new[] { i, j + 1 });
                    }
                    else if (j - 1 >= 0 && j + 3 < columnsCnt && field[i, j + 1] == whoStep && field[i, j + 2] == ObjectType.Empty &&
                             field[i, j - 1] == ObjectType.Empty && field[i, j + 3] == whoStep)
                    {
                        steps.Add(new[] { i, j - 1 });
                        steps.Add(new[] { i, j + 2 });
                    }
                    #endregion

                    #region Vertical checks
                    // Ищем атакующие ходы для двух одинаковых рядом лежащих объектов с пустыми клетками по боками
                    if (i - 1 >= 0 && i + 2 < rowsCnt && field[i + 1, j] == whoStep &&
                        field[i - 1, j] == ObjectType.Empty && field[i + 2, j] == ObjectType.Empty)
                    {
                        if (i - 3 >= 0 && field[i - 2, j] == ObjectType.Empty && field[i - 3, j] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i - 2, j });
                            steps.Add(new[] { i - 1, j });
                        }
                        else if (i - 2 >= 0 && i + 3 < rowsCnt && field[i - 2, j] == ObjectType.Empty && field[i + 3, j] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i - 1, j });
                        }

                        if (i + 4 < rowsCnt && field[i + 3, j] == ObjectType.Empty && field[i + 4, j] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i + 2, j });
                            steps.Add(new[] { i + 3, j });
                        }
                        else if (i - 2 >= 0 && i + 3 < rowsCnt && field[i - 2, j] == ObjectType.Empty && field[i + 3, j] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i + 2, j });
                        }
                    }

                    // Ищем атакующие ходы для двух одинаковых объектов через пустую клетку с пустыми клетками по боками
                    if (i - 1 >= 0 && i + 3 < rowsCnt && field[i + 1, j] == ObjectType.Empty && field[i + 2, j] == whoStep &&
                        field[i + 3, j] == ObjectType.Empty && field[i - 1, j] == ObjectType.Empty)
                    {
                        if (i - 2 >= 0 && field[i - 2, j] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i - 1, j });
                        }
                        if (i - 2 >= 0 && field[i - 2, j] == ObjectType.Empty ||
                            i + 4 < rowsCnt && field[i + 4, j] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i + 1, j });
                        }
                        if (i + 4 < rowsCnt && field[i + 4, j] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i + 3, j });
                        }
                    }

                    // Ищем атакующие ходы для трех одинаковых объектов после вражеского или начала поля
                    if (i + 4 < rowsCnt && field[i + 1, j] == whoStep && field[i + 2, j] == whoStep &&
                             field[i + 3, j] == ObjectType.Empty && field[i + 4, j] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i + 3, j });
                        steps.Add(new[] { i + 4, j });
                    }
                    else if (i - 1 >= 0 && i + 3 < rowsCnt && field[i + 1, j] == whoStep && field[i + 2, j] == whoStep &&
                             field[i - 1, j] == ObjectType.Empty && field[i + 3, j] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i - 1, j });
                        steps.Add(new[] { i + 3, j });
                    }
                    else if (i - 2 >= 0 && i + 2 < rowsCnt && field[i + 1, j] == whoStep && field[i + 2, j] == whoStep &&
                             field[i - 1, j] == ObjectType.Empty && field[i - 2, j] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i - 2, j });
                        steps.Add(new[] { i - 1, j });
                    }
                    else if (i + 4 < rowsCnt && field[i + 1, j] == ObjectType.Empty && field[i + 2, j] == whoStep &&
                             field[i + 3, j] == ObjectType.Empty && field[i + 4, j] == whoStep)
                    {
                        steps.Add(new[] { i + 1, j });
                        steps.Add(new[] { i + 3, j });
                    }
                    else if (i + 4 < rowsCnt && field[i + 1, j] == whoStep && field[i + 2, j] == ObjectType.Empty &&
                             field[i + 3, j] == ObjectType.Empty && field[i + 4, j] == whoStep)
                    {
                        steps.Add(new[] { i + 2, j });
                        steps.Add(new[] { i + 3, j });
                    }
                    else if (i + 4 < rowsCnt && field[i + 1, j] == whoStep && field[i + 2, j] == ObjectType.Empty &&
                             field[i + 3, j] == whoStep && field[i + 4, j] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i + 2, j });
                        steps.Add(new[] { i + 4, j });
                    }
                    else if (i + 4 < rowsCnt && field[i + 1, j] == ObjectType.Empty && field[i + 2, j] == whoStep &&
                             field[i + 3, j] == whoStep && field[i + 4, j] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i + 1, j });
                        steps.Add(new[] { i + 4, j });
                    }
                    else if (i + 4 < rowsCnt && field[i + 1, j] == ObjectType.Empty && field[i + 2, j] == ObjectType.Empty &&
                             field[i + 3, j] == whoStep && field[i + 4, j] == whoStep)
                    {
                        steps.Add(new[] { i + 1, j });
                        steps.Add(new[] { i + 2, j });
                    }
                    else if (i - 1 >= 0 && i + 3 < rowsCnt && field[i + 1, j] == ObjectType.Empty && field[i + 2, j] == whoStep &&
                             field[i - 1, j] == ObjectType.Empty && field[i + 3, j] == whoStep)
                    {
                        steps.Add(new[] { i - 1, j });
                        steps.Add(new[] { i + 1, j });
                    }
                    else if (i - 1 >= 0 && i + 3 < rowsCnt && field[i + 1, j] == whoStep && field[i + 2, j] == ObjectType.Empty &&
                             field[i - 1, j] == ObjectType.Empty && field[i + 3, j] == whoStep)
                    {
                        steps.Add(new[] { i - 1, j });
                        steps.Add(new[] { i + 2, j });
                    }
                    #endregion

                    #region Diagonal from left top to right bottom cheks ((i-, j-) -> (i+, j+))
                    // Ищем атакующие ходы для двух одинаковых рядом лежащих объектов с пустыми клетками по боками
                    if (i - 1 >= 0 && j - 1 >=0 && i + 2 < rowsCnt && j + 2 < columnsCnt && field[i + 1, j + 1] == whoStep &&
                        field[i - 1, j - 1] == ObjectType.Empty && field[i + 2, j + 2] == ObjectType.Empty)
                    {
                        if (i - 3 >= 0 && j - 3 >= 0 && field[i - 2, j - 2] == ObjectType.Empty && field[i - 3, j - 3] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i - 2, j - 2 });
                            steps.Add(new[] { i - 1, j - 1 });
                        }
                        else if (i - 2 >= 0 && j - 2 >= 0 && i + 3 < rowsCnt && j + 3 < columnsCnt && 
                                 field[i - 2, j - 2] == ObjectType.Empty && field[i + 3, j + 3] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i - 1, j - 1 });
                        }

                        if (i + 4 < rowsCnt && j + 4 < columnsCnt && field[i + 3, j + 3] == ObjectType.Empty && field[i + 4, j + 4] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i + 2, j + 2 });
                            steps.Add(new[] { i + 3, j + 3 });
                        }
                        else if (i - 2 >= 0 && j - 2 >= 0 && i + 3 < rowsCnt && j + 3 < columnsCnt && 
                                 field[i - 2, j - 2] == ObjectType.Empty && field[i + 3, j + 3] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i + 2, j + 2 });
                        }
                    }

                    // Ищем атакующие ходы для двух одинаковых объектов через пустую клетку с пустыми клетками по боками
                    if (i - 1 >= 0 && j - 1 >= 0 && i + 3 < rowsCnt && j + 3 < columnsCnt && field[i + 1, j + 1] == ObjectType.Empty && 
                        field[i + 2, j + 2] == whoStep && field[i + 3, j + 3] == ObjectType.Empty && field[i - 1, j - 1] == ObjectType.Empty)
                    {
                        if (i - 2 >= 0 && j - 2 >= 0 && field[i - 2, j - 2] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i - 1, j - 1 });
                        }
                        if (i - 2 >= 0 && j - 2 >= 0 && field[i - 2, j - 2] == ObjectType.Empty ||
                            i + 4 < rowsCnt && j + 4 < columnsCnt && field[i + 4, j + 4] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i + 1, j + 1 });
                        }
                        if (i + 4 < rowsCnt && j + 4 < columnsCnt && field[i + 4, j + 4] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i + 3, j + 3 });
                        }
                    }

                    // Ищем атакующие ходы для трех одинаковых объектов после вражеского или начала поля
                    if (i + 4 < rowsCnt && j + 4 < columnsCnt && field[i + 1, j + 1] == whoStep && field[i + 2, j + 2] == whoStep &&
                             field[i + 3, j + 3] == ObjectType.Empty && field[i + 4, j + 4] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i + 3, j + 3 });
                        steps.Add(new[] { i + 4, j + 4 });
                    }
                    else if (i - 1 >= 0 && j - 1 >= 0 && i + 3 < rowsCnt && j + 3 < columnsCnt && field[i + 1, j + 1] == whoStep && 
                             field[i + 2, j + 2] == whoStep && field[i - 1, j - 1] == ObjectType.Empty && field[i + 3, j + 3] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i - 1, j - 1 });
                        steps.Add(new[] { i + 3, j + 3 });
                    }
                    else if (i - 2 >= 0 && j - 2 >= 0 && i + 2 < rowsCnt && j + 2 < columnsCnt && field[i + 1, j + 1] == whoStep && 
                             field[i + 2, j + 2] == whoStep && field[i - 1, j - 1] == ObjectType.Empty && field[i - 2, j - 2] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i - 2, j - 2 });
                        steps.Add(new[] { i - 1, j - 1 });
                    }
                    else if (i + 4 < rowsCnt && j + 4 < columnsCnt && field[i + 1, j + 1] == ObjectType.Empty && 
                             field[i + 2, j + 2] == whoStep && field[i + 3, j + 3] == ObjectType.Empty && field[i + 4, j + 4] == whoStep)
                    {
                        steps.Add(new[] { i + 1, j + 1 });
                        steps.Add(new[] { i + 3, j + 3 });
                    }
                    else if (i + 4 < rowsCnt && j + 4 < columnsCnt && field[i + 1, j + 1] == whoStep && field[i + 2, j + 2] == ObjectType.Empty &&
                             field[i + 3, j + 3] == ObjectType.Empty && field[i + 4, j + 4] == whoStep)
                    {
                        steps.Add(new[] { i + 2, j + 2 });
                        steps.Add(new[] { i + 3, j + 3 });
                    }
                    else if (i + 4 < rowsCnt && j + 4 < columnsCnt && field[i + 1, j + 1] == whoStep && field[i + 2, j + 2] == ObjectType.Empty &&
                             field[i + 3, j + 3] == whoStep && field[i + 4, j + 4] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i + 2, j + 2 });
                        steps.Add(new[] { i + 4, j + 4 });
                    }
                    else if (i + 4 < rowsCnt && j + 4 < columnsCnt && field[i + 1, j + 1] == ObjectType.Empty && 
                             field[i + 2, j + 2] == whoStep && field[i + 3, j + 3] == whoStep && field[i + 4, j + 4] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i + 1, j + 1 });
                        steps.Add(new[] { i + 4, j + 4 });
                    }
                    else if (i + 4 < rowsCnt && j + 4 < columnsCnt && field[i + 1, j + 1] == ObjectType.Empty && field[i + 2, j + 2] == ObjectType.Empty &&
                             field[i + 3, j + 3] == whoStep && field[i + 4, j + 4] == whoStep)
                    {
                        steps.Add(new[] { i + 1, j + 1 });
                        steps.Add(new[] { i + 2, j + 2 });
                    }
                    else if (i - 1 >= 0 && j - 1 >= 0 && i + 3 < rowsCnt && j + 3 < columnsCnt && field[i + 1, j + 1] == ObjectType.Empty && 
                             field[i + 2, j + 2] == whoStep && field[i - 1, j - 1] == ObjectType.Empty && field[i + 3, j + 3] == whoStep)
                    {
                        steps.Add(new[] { i - 1, j - 1 });
                        steps.Add(new[] { i + 1, j + 1 });
                    }
                    else if (i - 1 >= 0 && j - 1 >= 0 && i + 3 < rowsCnt && j + 3 < columnsCnt && field[i + 1, j + 1] == whoStep && 
                             field[i + 2, j + 2] == ObjectType.Empty && field[i - 1, j - 1] == ObjectType.Empty && field[i + 3, j + 3] == whoStep)
                    {
                        steps.Add(new[] { i - 1, j - 1 });
                        steps.Add(new[] { i + 2, j + 2 });
                    }
                    #endregion

                    #region Diagonal from left bottom to right top cheks ((i+, j-) -> (i-, j+))
                    // Ищем атакующие ходы для двух одинаковых рядом лежащих объектов с пустыми клетками по боками
                    if (i - 2 >= 0 && j - 1 >= 0 && i + 1 < rowsCnt && j + 2 < columnsCnt && field[i - 1, j + 1] == whoStep &&
                        field[i + 1, j - 1] == ObjectType.Empty && field[i - 2, j + 2] == ObjectType.Empty)
                    {
                        if (i - 4 >= 0 && j + 4 < columnsCnt && field[i - 3, j + 3] == ObjectType.Empty && field[i - 4, j + 4] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i - 3, j + 3 });
                            steps.Add(new[] { i - 2, j + 2 });
                        }
                        else if (i - 3 >= 0 && j - 2 >= 0 && i + 2 < rowsCnt && j + 3 < columnsCnt &&
                                 field[i + 2, j - 2] == ObjectType.Empty && field[i - 3, j + 3] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i - 2, j + 2 });
                        }

                        if (i + 3 < rowsCnt && j - 3 >= 0 && field[i + 2, j - 2] == ObjectType.Empty && field[i + 3, j - 3] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i + 1, j - 1 });
                            steps.Add(new[] { i + 2, j - 2 });
                        }
                        else if (i - 3 >= 0 && j - 2 >= 0 && i + 2 < rowsCnt && j + 3 < columnsCnt &&
                                 field[i + 2, j - 2] == ObjectType.Empty && field[i - 3, j + 3] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i + 1, j - 1 });
                        }
                    }

                    // Ищем атакующие ходы для двух одинаковых объектов через пустую клетку с пустыми клетками по боками
                    if (i - 3 >= 0 && j - 1 >= 0 && i + 1 < rowsCnt && j + 3 < columnsCnt && field[i - 1, j + 1] == ObjectType.Empty &&
                        field[i - 2, j + 2] == whoStep && field[i - 3, j + 3] == ObjectType.Empty && field[i + 1, j - 1] == ObjectType.Empty)
                    {
                        if (i - 4 >= 0 && j + 4 < columnsCnt && field[i - 4, j + 4] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i - 3, j + 3 });
                        }
                        if (i + 2 < rowsCnt && j - 2 >= 0 && field[i + 2, j - 2] == ObjectType.Empty ||
                            i - 4 >= 0 && j + 4 < columnsCnt && field[i - 4, j + 4] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i - 1, j + 1 });
                        }
                        if (i + 2 < rowsCnt && j - 2 >= 0 && field[i + 2, j - 2] == ObjectType.Empty)
                        {
                            steps.Add(new[] { i + 1, j - 1 });
                        }
                    }

                    // Ищем атакующие ходы для трех одинаковых объектов после вражеского или начала поля
                    if (i - 4 >= 0 && j + 4 < columnsCnt && field[i - 1, j + 1] == whoStep && field[i - 2, j + 2] == whoStep &&
                             field[i - 3, j + 3] == ObjectType.Empty && field[i - 4, j + 4] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i - 4, j + 4 });
                        steps.Add(new[] { i - 3, j + 3 });
                    }
                    else if (i - 3 >= 0 && j - 1 >= 0 && i + 1 < rowsCnt && j + 3 < columnsCnt && field[i - 1, j + 1] == whoStep &&
                             field[i - 2, j + 2] == whoStep && field[i + 1, j - 1] == ObjectType.Empty && field[i - 3, j + 3] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i - 3, j + 3 });
                        steps.Add(new[] { i + 1, j - 1 });
                    }
                    else if (i - 2 >= 0 && j - 2 >= 0 && i + 2 < rowsCnt && j + 2 < columnsCnt && field[i - 1, j + 1] == whoStep &&
                             field[i - 2, j + 2] == whoStep && field[i + 1, j - 1] == ObjectType.Empty && field[i + 2, j - 2] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i + 1, j - 1 });
                        steps.Add(new[] { i + 2, j - 2 });
                    }
                    else if (i - 4 >= 0 && j + 4 < columnsCnt && field[i - 1, j + 1] == ObjectType.Empty &&
                             field[i - 2, j + 2] == whoStep && field[i - 3, j + 3] == ObjectType.Empty && field[i - 4, j + 4] == whoStep)
                    {
                        steps.Add(new[] { i - 3, j + 3 });
                        steps.Add(new[] { i - 1, j + 1 });
                    }
                    else if (i - 4 >= 0 && j + 4 < columnsCnt && field[i - 1, j + 1] == whoStep && field[i - 2, j + 2] == ObjectType.Empty &&
                             field[i - 3, j + 3] == ObjectType.Empty && field[i - 4, j + 4] == whoStep)
                    {
                        steps.Add(new[] { i - 3, j + 3 });
                        steps.Add(new[] { i - 2, j + 2 });
                    }
                    else if (i - 4 >= 0 && j + 4 < columnsCnt && field[i - 1, j + 1] == whoStep && field[i - 2, j + 2] == ObjectType.Empty &&
                             field[i - 3, j + 3] == whoStep && field[i - 4, j + 4] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i - 4, j + 4 });
                        steps.Add(new[] { i - 2, j + 2 });
                    }
                    else if (i - 4 >= 0 && j + 4 < columnsCnt && field[i - 1, j + 1] == ObjectType.Empty &&
                             field[i - 2, j + 2] == whoStep && field[i - 3, j + 3] == whoStep && field[i - 4, j + 4] == ObjectType.Empty)
                    {
                        steps.Add(new[] { i - 4, j + 4 });
                        steps.Add(new[] { i - 1, j + 1 });
                    }
                    else if (i - 4 >= 0 && j + 4 < columnsCnt && field[i - 1, j + 1] == ObjectType.Empty && field[i - 2, j + 2] == ObjectType.Empty &&
                             field[i - 3, j + 3] == whoStep && field[i - 4, j + 4] == whoStep)
                    {
                        steps.Add(new[] { i - 2, j + 2 });
                        steps.Add(new[] { i - 1, j + 1 });
                    }
                    else if (i - 3 >= 0 && j - 1 >= 0 && i + 1 < rowsCnt && j + 3 < columnsCnt && field[i - 1, j + 1] == ObjectType.Empty &&
                             field[i - 2, j + 2] == whoStep && field[i + 1, j - 1] == ObjectType.Empty && field[i - 3, j + 3] == whoStep)
                    {
                        steps.Add(new[] { i - 1, j + 1 });
                        steps.Add(new[] { i + 1, j - 1 });
                    }
                    else if (i - 3 >= 0 && j - 1 >= 0 && i + 1 < rowsCnt && j + 3 < columnsCnt && field[i - 1, j + 1] == whoStep &&
                             field[i - 2, j + 2] == ObjectType.Empty && field[i + 1, j - 1] == ObjectType.Empty && field[i - 3, j + 3] == whoStep)
                    {
                        steps.Add(new[] { i - 2, j + 2 });
                        steps.Add(new[] { i + 1, j - 1 });
                    }
                    #endregion
                }
            }

            return steps;
        }
    }
}