using System;
using System.Text.RegularExpressions;

namespace Confirmit.CATI.Supervisor.Core.Export.Tools
{
    /// <summary>
    /// Respresents utility class which works with Excel column names like 'AA' and is able to increment columns.
    /// </summary>
    internal class ExcelColumnName
    {
        #region Nested types

        /// <summary>
        /// English alphabet.
        /// </summary>
        private enum Alphabet
        {
            A,
            B,
            C,
            D,
            E,
            F,
            G,
            H,
            I,
            J,
            K,
            L,
            M,
            N,
            O,
            P,
            Q,
            R,
            S,
            T,
            U,
            V,
            W,
            X,
            Y,
            Z
        }

        #endregion

        #region Fields

        private string m_ColumnName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes new instance of ExcelColumnName class and fills it with given data.
        /// </summary>
        /// <param name="columnName">Excel column name.</param>
        public ExcelColumnName(string columnName)
        {
            string tmp = columnName.Trim().ToUpper();
            if (!Regex.IsMatch(tmp, @"^[a-zA-Z]+$"))
            {
                throw new ArgumentException();
            }

            m_ColumnName = tmp;
        }

        /// <summary>
        /// Initializes new instance of ExcelColumnName class and fills it with given data.
        /// </summary>
        /// <param name="columnName">Excel column name.</param>
        public ExcelColumnName(char columnName)
        {
            string tmp = columnName.ToString().Trim().ToUpper();
            if (!Regex.IsMatch(tmp, @"^[a-zA-Z]+$"))
            {
                throw new ArgumentException();
            }

            m_ColumnName = tmp;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets column name.
        /// </summary>
        public string Name
        {
            get
            {
                return m_ColumnName;
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Moves one column left.
        /// </summary>
        /// <param name="column">Column to move.</param>
        /// <returns>Current column with new column name.</returns>
        public static ExcelColumnName operator ++(ExcelColumnName column)
        {
            if (column.m_ColumnName.Length == 1)
            {
                if (column.m_ColumnName == "Z")
                {
                    column.m_ColumnName = "AA";
                }
                else
                {
                    Alphabet letter = (Alphabet)Enum.Parse(typeof(Alphabet), column.m_ColumnName);
                    letter++;
                    column.m_ColumnName = letter.ToString();
                }
            }
            else
            {
                Char lastChar = column.m_ColumnName[column.m_ColumnName.Length - 1];
                if (lastChar == 'Z')
                {
                    ExcelColumnName tmp = new ExcelColumnName(column.m_ColumnName.Substring(0, column.m_ColumnName.Length - 1));
                    tmp++;
                    column.m_ColumnName = tmp.Name + "A";
                }
                else
                {
                    string oldPart = column.m_ColumnName.Substring(0, column.m_ColumnName.Length - 1);
                    ExcelColumnName tmp = new ExcelColumnName(lastChar);
                    tmp++;
                    column.m_ColumnName = oldPart + tmp.Name;
                }
            }

            return column;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return m_ColumnName;
        }

        #endregion
    }
}
