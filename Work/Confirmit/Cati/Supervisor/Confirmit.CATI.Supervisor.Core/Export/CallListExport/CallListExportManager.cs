using System;
using System.Data;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using System.Collections.Generic;
using System.Xml.Linq;
using Confirmit.CATI.Supervisor.Core.Export.Tools;

namespace Confirmit.CATI.Supervisor.Core.Export.CallListExport
{
    /// <summary>
    /// Export manager for call list in Call Management.
    /// Call list may contain several Confirmit variable, which are added dynamicaly by 
    /// user. So static Excel template of call list can't support this. This manager will
    /// modify static template before export according info about dynamic Confirmit
    /// variables. This gives us ability to use modified template with ExportManager class.
    /// </summary>
    public class CallListExportManager
    {
        #region Nested types

        private class VarColumnInfo
        {
            /// <summary>
            /// Gets/sets variable name.
            /// </summary>
            public string VariableName
            {
                get;
                set;
            }

            /// <summary>
            /// Gets/sets column name. This name will be used for retrieving data from data provider.
            /// </summary>
            public string ColumnName
            {
                get;
                set;
            }

            /// <summary>
            /// Gets/sets type of column data.
            /// </summary>
            public Type Type
            {
                get;
                set;
            }
        }

        #endregion

        #region Fields

        private const int m_RowHeaderIndex = 2;
        /// <summary>
        /// Name of the cell header format which will be used for new added confirmit vars column headers.
        /// </summary>
        private const string m_ColumnHeaderFormatCell = "A";
        private const int m_RowDataIndex = 3;
        /// <summary>
        /// Name of the cell with date format. Format of this cell will be used for identifying
        /// date format in template.
        /// </summary>
        private const string m_DateFormatColumn = "B1";

        #endregion

        #region Methods

        /// <summary>
        /// Adds variables definition to existing template.
        /// </summary>
        /// <param name="templateFilePath">Excel template.</param>
        /// <param name="data">Call list.</param>
        public static void AddVariablesToCallListTemplate(string templateFilePath, DataTable data)
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(templateFilePath, true))
            {
                WorksheetPart sheet = doc.WorkbookPart.WorksheetParts.First();
                XDocument sheetXml = OpenXmlHelper.GetXmlFromPart(sheet);
                XNamespace nm = sheetXml.Root.Name.Namespace;

                XElement rowHeader = (from c in sheetXml.Root.Element(nm + "sheetData").Elements()
                                      where c.Name.LocalName == "row" && c.Attribute("r").Value == m_RowHeaderIndex.ToString()
                                      select c).First();
                XElement rowData = (from c in sheetXml.Root.Element(nm + "sheetData").Elements()
                                      where c.Name.LocalName == "row" && c.Attribute("r").Value == m_RowDataIndex.ToString()
                                      select c).First();
                string lastColumnName = OpenXmlHelper.GetExcelColumnName(
                    (from c in rowHeader.Elements()
                     where c.Name.LocalName == "c"
                     orderby c.Attribute("r").Value ascending
                     select c.Attribute("r").Value).Last()
                );
                // getting date format from cell (m_DateFormatRow, m_DateFormatColumn)
                string dateFormat = OpenXmlHelper.GetCellStyle(
                    sheetXml.Root.Element(nm + "sheetData"), 
                    m_DateFormatColumn
                );
                // getting header format for new added header cells
                string headerFormat = OpenXmlHelper.GetCellStyle(
                    sheetXml.Root.Element(nm + "sheetData"), 
                    (m_ColumnHeaderFormatCell + rowHeader.Attribute("r").Value)
                );

                ExcelColumnName currentColumn = new ExcelColumnName(lastColumnName);
                ExcelColumnName startMergeColumn = new ExcelColumnName(lastColumnName);
                startMergeColumn++;
                VarColumnInfo[] varColumns = GetVariablesInfo(data);
                foreach (VarColumnInfo varColumn in varColumns)
                {
                    currentColumn++;

                    XElement cellHeader = new XElement(
                        nm + "c",
                        new XAttribute("r", currentColumn.Name + m_RowHeaderIndex.ToString()),
                        new XAttribute("s", headerFormat)
                    );

                    OpenXmlHelper.SetCellValue(cellHeader, varColumn.VariableName, nm);
                    rowHeader.Add(cellHeader);

                    XElement cellData = new XElement(
                        nm + "c",
                        new XAttribute("r", currentColumn.Name + m_RowDataIndex.ToString())
                    );
                    if(varColumn.Type == typeof(DateTime))
                    {
                        cellData.Add(new XAttribute("s", dateFormat));
                    }

                    OpenXmlHelper.SetCellValue(cellData, String.Format("<%Data.{0}%>", varColumn.ColumnName), nm);
                    rowData.Add(cellData);
                }
                
                OpenXmlHelper.SaveSheetDocument(sheetXml, sheet);
            }
        }

        /// <summary>
        /// Analyzes call list and extracts and returns variables columns definition.
        /// </summary>
        /// <param name="data">Call list.</param>
        /// <returns>Collection of variable columns.</returns>
        private static VarColumnInfo[] GetVariablesInfo(DataTable data)
        {
            List<VarColumnInfo> vars = new List<VarColumnInfo>();

            foreach (DataColumn column in data.Columns)
            {
                if (column.ColumnName.StartsWith("Var"))
                {
                    // column contains variable
                    vars.Add(
                        new VarColumnInfo()
                        {
                            ColumnName = column.ColumnName,
                            VariableName = column.ColumnName.Substring(3),
                            Type = column.DataType
                        }
                    );
                }
            }

            return vars.ToArray();
        }

        #endregion
    }
}
