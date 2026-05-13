using System;
using System.Text;

namespace TicTacToe
{
    public class FieldConverter
    {
        /// <summary>
        /// Convert field in text view to array
        /// </summary>
        /// <param name="text">Field in text view</param>
        /// <param name="rowsCount">Rows count</param>
        /// <param name="columnsCount">Columns count</param>
        /// <returns></returns>
        public ObjectType[,] TextToField(string text, int rowsCount, int columnsCount)
        {
            var field = new ObjectType[rowsCount, columnsCount];
            int i = 0;
            int j = 0;
            int q = 0;
            while (q < text.Length)
            {
                ObjectType obj = (ObjectType) Convert.ToByte(text[q].ToString());
                q++;

                SetNewObject(field, obj, ref i, ref j, columnsCount);

                if (q < text.Length && text[q] == '_')
                {
                    q++;
                    int repeatCnt = 0;

                    while (text[q] != '_')
                    {
                        repeatCnt = repeatCnt * 10 + Convert.ToByte(text[q].ToString());
                        q++;
                    }

                    for (int w = 0; w < repeatCnt - 1; w++)
                    {
                        SetNewObject(field, obj, ref i, ref j, columnsCount);
                    }

                    q++;
                }
            }

            return field;
        }

        private void SetNewObject(ObjectType[,] field, ObjectType obj, ref int i, ref int j, int columnsCount)
        {
            field[i, j] = obj;
            j++;
            if (j >= columnsCount)
            {
                j = 0;
                i++;
            }
        }

        public string FieldToText(ObjectType[,] field)
        {
            var text = new StringBuilder();
            var theSame = new StringBuilder();

            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    string obj = ((byte)field[i, j]).ToString();

                    if (theSame.Length == 0)
                    {
                        theSame.Append(obj);
                        continue;
                    }

                    if (theSame[0].ToString() == obj)
                    {
                        theSame.Append(obj);
                    }
                    else
                    {
                        if (theSame.Length < 4)
                        {
                            text.Append(theSame);
                        }
                        else
                        {
                            text.Append(theSame[0] + "_" + theSame.Length + "_");
                        }

                        theSame.Clear();
                        theSame.Append(obj);
                    }
                }
            }

            if (theSame.Length < 4)
            {
                text.Append(theSame);
            }
            else
            {
                text.Append(theSame[0] + "_" + theSame.Length + "_");
            }

            return text.ToString();
        }
    }
}