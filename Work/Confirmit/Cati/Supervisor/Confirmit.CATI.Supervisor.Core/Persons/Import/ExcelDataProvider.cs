using System;
using System.Data;
using System.IO;
using FlexCel.Core;

namespace Confirmit.CATI.Supervisor.Core.Persons.Import
{
    public class ExcelDataProvider
    {
        /// <summary>
        /// Gets active sheet data
        /// </summary>
        public static DataTable GetXlsData(Stream srcStream)
        {
            ExcelFile xls;
            try
            {
                xls = new FlexCel.XlsAdapter.XlsFile();
                srcStream.Position = 0;
                xls.Open(srcStream);
                xls.ActiveSheet = 1;
            }
            catch (Exception)
            {
                throw new ErrorReadingXlsFileException();
            }

            if (xls.ColCount < 1 || xls.RowCount < 1)
            {
                throw new EmptyXlsSheetException();
            }

            var dt = new DataTable("Interviewers");
            for (int x = 0; x < xls.ColCount; x++)
            {
                dt.Columns.Add("DataColumn" + x);
            }

            for (int y = 0; y < xls.RowCount; y++)
            {
                var row = new object[xls.ColCount];
                for (int x = 0; x < xls.ColCount; x++)
                {
                    // FlexCel indexes all object from 1, not from 0.
                    object cell = xls.GetCellValue(y + 1, x + 1);
                    row[x] = cell;
                }

                dt.Rows.Add(row);
            }

            return dt;
        }            
    }
}
