using System;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using Confirmit.CATI.Supervisor.Core.Export.Parse;
using Confirmit.CATI.Supervisor.Core.Export.Tools;
using System.Text.RegularExpressions;
using Confirmit.CATI.Supervisor.Core.Common;

namespace Confirmit.CATI.Supervisor.Core.Export
{
    /// <summary>
    /// Represents data which define export data.
    /// </summary>
    public class ExportDefinitionData
    {
        /// <summary>
        /// Gets/sets name of sheet which contains export template.
        /// </summary>
        public string SheetName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets data for export.
        /// </summary>
        public IExportDataProvider Data
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents manager class for export into OpenXML Excel format.
    /// </summary>
    public static class ExportManager
    {
        /// <summary>
        /// Copies template file from App_Data directory to temporary directory and returns path to copied file.
        /// </summary>
        /// <param name="templateName">Template file name</param>
        /// <returns>Path to copied file</returns>
        public static string GetTemplatePath(string templateName)
        {
            string appDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_data");
            string templatePath = Path.Combine(appDataPath, templateName);

            string tempFileName = Path.GetTempFileName();

            File.Copy(templatePath, tempFileName, true);

            if ((File.GetAttributes(tempFileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                // temporary file is read-only. We should make it writable
                File.SetAttributes(tempFileName, File.GetAttributes(tempFileName) ^ FileAttributes.ReadOnly);
            }

            return tempFileName;
        }

        /// <summary>
        /// Exports data given by provider using xlsx template file.
        /// </summary>
        /// <param name="data">Data to export.</param>
        /// <param name="templatePath">Path to template xlsx file.</param>
        public static void ExportUsingTemplate(string templatePath, IEnumerable<ExportDefinitionData> data)
        {
            using (SpreadsheetDocument template = SpreadsheetDocument.Open(templatePath, true))
            {
                WorkbookPart workbook = template.WorkbookPart;
                SharedStringTablePart sharedStrings = workbook.SharedStringTablePart;

                foreach (ExportDefinitionData export in data)
                {
                    ExportSheet(OpenXmlHelper.GetWorksheetByName(workbook, export.SheetName), sharedStrings, export.Data);
                }
            }
        }

        /// <summary>
        /// Exports single sheet.
        /// </summary>
        /// <param name="worksheet">Sheet for export.</param>
        /// <param name="sharedStringsPart"></param>
        /// <param name="data">Data to export.</param>
        private static void ExportSheet(WorksheetPart worksheet, SharedStringTablePart sharedStringsPart, IExportDataProvider data)
        {
            XNamespace nm = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            string[] sharedStrings = OpenXmlHelper.GetSharedStrings(sharedStringsPart, nm);
            Dictionary<string, ColumnProperty> columnToPropertyMap = new Dictionary<string, ColumnProperty>();

            XDocument sheet = OpenXmlHelper.GetXmlFromPart(worksheet);

            // enumerating and parse rows in template

            int currentRow = -1;
            string horizontalDetailsColumnName = String.Empty;
            string horizontalDetailsColumnStyle = String.Empty;

            XElement sheetData = sheet.Root.Element(nm + "sheetData");
            foreach (XElement row in (from c in sheetData.Elements() where c.Name.LocalName == "row" select c))
            {
                currentRow = Int32.Parse(row.Attribute("r").Value);
                foreach (XElement column in (from c in row.Elements() where c.Name.LocalName == "c" select c))
                {
                    string columnName, columnStyle;
                    string columnValue = OpenXmlHelper.GetColumnValue(column, nm, sharedStrings, out columnName, out columnStyle);
                    if (!String.IsNullOrEmpty(columnValue))
                    {
                        // trying to parse content cell content
                        LexemeInfo[] lexemes = ExcelTemplateParser.ParseCellMarkup(columnValue);

                        foreach (LexemeInfo lexeme in lexemes)
                        {
                            switch (lexeme.Type)
                            {
                                case LexemeType.Data:
                                    // Data lexeme means that current column should contain data from 
                                    // property. So we add it to our "Columns-To-Propery" map
                                    columnToPropertyMap.Add(columnName, new ColumnProperty(lexeme.Value, columnStyle));
                                    break;
                                case LexemeType.HorizontalDetails:
                                    // we have details in this template, so we should store column
                                    // from which details should start
                                    horizontalDetailsColumnName = columnName;
                                    horizontalDetailsColumnStyle = OpenXmlHelper.GetCellStyle(sheetData, lexeme.Value);
                                    break;
                                case LexemeType.Resources:
                                    //Replace value using resources                                    
                                    OpenXmlHelper.SetCellValue(column, ResourceWrapper.Instance.GetString(lexeme.Value), sheet.Root.Name.Namespace);
                                    break;
                                case LexemeType.Params:
                                    //Replace value using resources                                    
                                    OpenXmlHelper.SetCellValue(column, data.GetParameter(lexeme.Value), sheet.Root.Name.Namespace);
                                    break;
                                case LexemeType.Date:
                                    //Replace value using resources                                    
                                    OpenXmlHelper.SetCellValue(column, DateTime.UtcNow, sheet.Root.Name.Namespace);
                                    break;

                            }
                        }
                    }
                }
            }

            // we remove last row in template because it should contain only Data definitions
            (from c in sheetData.Elements()
             where c.Name.LocalName == "row" && c.Attribute("r").Value == currentRow.ToString()
             select c).First().Remove();

            FillWorksheetWithData(sheet, nm, data, columnToPropertyMap, currentRow, horizontalDetailsColumnName, horizontalDetailsColumnStyle);

            OpenXmlHelper.SaveSheetDocument(sheet, worksheet);
        }

        /// <summary>
        /// Fills worksheet with given data according given "Column-To-Property" map.
        /// </summary>
        /// <param name="sheet">Worksheet to fill.</param>
        /// <param name="nm">Namespace name.</param>
        /// <param name="data">Data.</param>
        /// <param name="columnToPropertyMap">"Column-To-Property" map.</param>
        /// <param name="startRow">Data starting row.</param>
        /// <param name="horizontalDetailsColumnName">The name of the column from which we should show details info.</param>
        /// <param name="horizontalDetailsColumnStyle"></param>
        private static void FillWorksheetWithData(
            XDocument sheet,
            XNamespace nm,
            IExportDataProvider data,
            Dictionary<string, ColumnProperty> columnToPropertyMap,
            int startRow,
            string horizontalDetailsColumnName,
            string horizontalDetailsColumnStyle)
        {
            bool hasHorizontalDetails = !String.IsNullOrEmpty(horizontalDetailsColumnName);
            int currentRow = startRow;
            XElement sheetData = sheet.Root.Element(nm + "sheetData");
            foreach (IExportRecordProvider record in data)
            {
                XElement row = new XElement(sheet.Root.Name.Namespace + "row", new XAttribute("r", currentRow));

                foreach (KeyValuePair<string, ColumnProperty> pair in columnToPropertyMap)
                {
                    string key = pair.Key;
                    object columnValue = record[pair.Value.Value];

                    if (columnValue != null)
                    {
                        XElement column = new XElement(sheet.Root.Name.Namespace + "c",
                            new XAttribute("r", key + currentRow));

                        if (String.IsNullOrEmpty(pair.Value.Style) == false)
                        {
                            column.SetAttributeValue("s", pair.Value.Style);
                        }

                        OpenXmlHelper.SetCellValue(column, columnValue, sheet.Root.Name.Namespace);

                        row.Add(column);
                    }
                }

                sheetData.Add(row);
                currentRow++;

                if (hasHorizontalDetails && record.Descendants != null)
                {
                    // we have horizontal details flag in template, so we should fill 
                    // horizontal details                    
                    FillHorizontalDetails(record.Descendants, sheetData, sheet.Root.Name.Namespace, horizontalDetailsColumnName, horizontalDetailsColumnStyle, ref currentRow);
                }
            }

            // updating sheet dimensions after filling data
            XElement dimensionElement = sheet.Root.Element(nm + "dimension");
            string dimensionRef = dimensionElement.Attribute("ref").Value;
            dimensionRef = Regex.Replace(dimensionRef, @"(?<=\w+\d+[:]\w+)\d+", (currentRow - 1).ToString());
            dimensionElement.Attribute("ref").Value = dimensionRef;
        }

        /// <summary>
        /// Fills horizontal details table. Function takes details from data record Descendants property
        /// and export them as horizontal table starting from given column. 
        /// </summary>
        /// <param name="data">Details data to export.</param>
        /// <param name="sheetData">Sheet data xml element.</param>
        /// <param name="nm">Namespace name.</param>
        /// <param name="horizontalDetailsColumnName">Starting column.</param>
        /// <param name="horizontalDetailsColumnStyle"></param>
        /// <param name="currentRow">Current row.</param>
        private static void FillHorizontalDetails(
            IExportRecordProvider data,
            XElement sheetData,
            XNamespace nm,
            string horizontalDetailsColumnName,
            string horizontalDetailsColumnStyle,
            ref int currentRow
        )
        {
            int titleRowIndex = currentRow;
            int dataRowIndex = currentRow + 1;

            XElement titleRow = new XElement(
                nm + "row",
                new XAttribute("r", titleRowIndex),
                new XAttribute("outlineLevel", 1),
                new XAttribute("hidden", 1)
            );

            XElement dataRow = new XElement(
                nm + "row",
                new XAttribute("r", dataRowIndex),
                new XAttribute("outlineLevel", 1),
                new XAttribute("hidden", 1)
            );

            ExcelColumnName currentColumn = new ExcelColumnName(horizontalDetailsColumnName);

            foreach (ExportItem item in data)
            {
                XElement titleColumn = new XElement(
                    nm + "c",
                    new XAttribute("r", currentColumn.Name + titleRowIndex.ToString())
                );

                if (string.IsNullOrEmpty(horizontalDetailsColumnStyle) == false)
                {
                    titleColumn.SetAttributeValue("s", horizontalDetailsColumnStyle);
                }

                OpenXmlHelper.SetCellValue(titleColumn, item.Name, nm);

                XElement dataColumn = new XElement(
                    nm + "c",
                    new XAttribute("r", currentColumn.Name + dataRowIndex.ToString())
                );
                OpenXmlHelper.SetCellValue(dataColumn, item.Value, nm);

                titleRow.Add(titleColumn);
                dataRow.Add(dataColumn);
                currentColumn++;
            }

            sheetData.Add(titleRow);
            sheetData.Add(dataRow);
            currentRow += 2;
        }

        public static int GetMaxValueForPageRange(int pageSize, int totalCount)
        {
            var size = pageSize == 0 ? 1 : pageSize;

            var tmp = (int)Math.Ceiling((double)totalCount / size);
            if (tmp == 0)
            {
                tmp = 1;
            }
            return tmp;
        }
    }
}
