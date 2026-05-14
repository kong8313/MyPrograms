using System;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Confirmit.CATI.Supervisor.Core.Export.Tools
{
    /// <summary>
    /// Helper class for OpenXML
    /// </summary>
    internal class OpenXmlHelper
    {
        /// <summary>
        /// Returns XML document constructed from OpenXML part content.
        /// </summary>
        /// <param name="part">OpenXML part.</param>
        /// <returns>XDocument object.</returns>
        public static XDocument GetXmlFromPart(OpenXmlPart part)
        {
            using(TextReader reader = new StreamReader(part.GetStream()))
            {
                return XDocument.Load(reader);
            }
        }

        /// <summary>
        /// Saves modified document into old worksheet part.
        /// </summary>
        /// <param name="sheet">Modified sheet xml content.</param>
        /// <param name="worksheet">Worksheet part to save new content.</param>
        public static void SaveSheetDocument(XDocument sheet, WorksheetPart worksheet)
        {
            using (TextWriter writer = new StreamWriter(worksheet.GetStream(FileMode.Create, FileAccess.Write)))
            {
                sheet.Save(writer);
            }
        }

        /// <summary>
        /// Returns column value.
        /// </summary>
        /// <param name="column">Column.</param>
        /// <param name="nm">Namespace name.</param>
        /// <param name="sharedStrings">Shared strings collection.</param>
        /// <param name="columnName">Returns column name (for example "AA").</param>
        /// <returns>Column value or empty string.</returns>
        public static string GetColumnValue(XElement column, XNamespace nm, string[] sharedStrings, out string columnName, out string columnStyle)
        {
            string value = String.Empty;
            columnName = String.Empty;
            columnStyle = String.Empty;

            if (!column.IsEmpty)
            {
                XAttribute t = column.Attribute("t");
                if (t != null && t.Value == "inlineStr")
                {
                    value = column.Element(nm + "is").Element(nm + "t").Value;
                }
                else if (t != null && t.Value == "s")
                {
                    // value is stored in shared strings
                    value = sharedStrings[Int32.Parse(column.Element(nm + "v").Value)];
                }
                else
                {
                    // not shared strings value
                    value = column.Element(nm + "v").Value;
                }

                columnName = OpenXmlHelper.GetExcelColumnName(column.Attribute("r").Value);

                XAttribute a = column.Attribute("s");

                if (a != null)
                {
                    columnStyle = a.Value;
                }
            }

            return value;
        }

        /// <summary>
        /// Sets given value to given Xml cell element.
        /// </summary>
        /// <remarks>We can update value for already existent column</remarks>
        /// <param name="cell">Xml cell element.</param>
        /// <param name="cellValue">Value to set.</param>
        /// <param name="nm">Namespace name.</param>
        public static void SetCellValue(XElement cell, object cellValue, XNamespace nm)
        {
            cell.Elements().Remove();            
            cell.Attributes().Where(x => x.Name.LocalName == "t").Remove();

            if (cellValue is String)
            {
                cell.Add(new XAttribute("t", "inlineStr"));
                cell.Add(
                    new XElement(
                        nm + "is",
                        new XElement(nm + "t", SanitizeXmlString(cellValue.ToString()))
                    )
                );
            }
            else if (cellValue is DateTime)
            {
                XElement value = new XElement(nm + "v", ((DateTime)cellValue).ToOADate());
                cell.Add(value);
            }
            else if (cellValue is TimeSpan)
            {
                XElement value = new XElement(nm + "v", ((TimeSpan)cellValue).TotalDays);
                cell.Add(value);
            }
            else
            {
                XElement value = new XElement(nm + "v", SanitizeXmlString(cellValue.ToString()));
                cell.Add(value);
            }
        }
        
        private static string SanitizeXmlString(string text)
        {
            if (string.IsNullOrEmpty(text) || text.All(XmlConvert.IsXmlChar)) return text;
            
            return new string(text.Where(XmlConvert.IsXmlChar).ToArray());
        }

        /// <summary>
        /// Sets given value to given Xml cell element.
        /// </summary>
        /// <param name="sheetData">Sheet data Xml element.</param>
        /// <param name="cellName">Cell name.</param>
        /// <param name="cellValue">Value to set.</param>
        /// <param name="nm">Namespace name.</param>
        public static void SetCellValue(XElement sheetData, string cellName, object cellValue, XNamespace nm)
        {
            XElement cell = GetCell(sheetData, cellName);
            if (cell != null)
            {
                SetCellValue(cell, cellValue, nm);
            }
        }

        /// <summary>
        /// Returns worksheet by name from given workbook.
        /// </summary>
        /// <param name="workbook">Work book.</param>
        /// <param name="name">Name of worsheet.</param>
        /// <returns>Worksheet, if found; otherwise null.</returns>
        public static WorksheetPart GetWorksheetByName(WorkbookPart workbook, string name)
        {
            string relID;

            using (TextReader reader = new StreamReader(workbook.GetStream()))
            {
                XDocument doc = XDocument.Load(reader);
                relID = (from a in
                             (from c in
                                  (from c in doc.Root.Elements() where c.Name.LocalName == "sheets" select c).Elements()
                              where c.Name.LocalName == "sheet"
                                  && c.Attribute("name").Value == name
                              select c).Attributes()
                         where a.Name.LocalName == "id"
                             && a.Name.NamespaceName == "http://schemas.openxmlformats.org/officeDocument/2006/relationships"
                         select a.Value
                        ).FirstOrDefault();
            }

            if (!String.IsNullOrEmpty(relID))
            {
                return (WorksheetPart)workbook.GetPartById(relID);
            }
            else
            {
                return null;
            }

            /*ega Add exception handling*/
        }

        /// <summary>
        /// Returns array of shared strings from shared strings part.
        /// </summary>
        /// <param name="sharedStringsPart">Shared strings part.</param>
        /// <param name="nm">Namespace name.</param>
        /// <returns>Array of shared strings.</returns>
        public static string[] GetSharedStrings(SharedStringTablePart sharedStringsPart, XNamespace nm)
        {
            XDocument strings;
            using (TextReader reader = new StreamReader(sharedStringsPart.GetStream()))
            {
                strings = XDocument.Load(reader);
            }

            return (from c in strings.Root.Elements()
                    where c.Name.LocalName == "si"
                    select c.Element(nm + "t").Value).ToArray();
        }


        /// <summary>
        /// Extracts literal column name from column reference string.
        /// </summary>
        /// <param name="referenceString"></param>
        /// <remarks>Function takes string like "A1" and extracts column name "A" from it.</remarks>
        /// <returns>Excel column name.</returns>
        public static string GetExcelColumnName(string referenceString)
        {
            string result = String.Empty;

            Regex reg = new Regex(@"^\w+(?=\d+$)");
            Match match = reg.Match(referenceString);
            if (match.Success)
            {
                result = match.Value;
            }

            return result;
        }

        /// <summary>
        /// Gets cell with given name.
        /// </summary>
        /// <param name="sheetData">Sheet data Xml element.</param>
        /// <param name="cellName">Cell name.</param>
        /// <returns>Cell Xml element or null.</returns>
        public static XElement GetCell(XElement sheetData, string cellName)
        {
            XElement el = (from r in sheetData.Elements()
                           where r.Name.LocalName == "row"
                           from c in r.Elements()
                           where c.Attribute("r").Value == cellName
                           select c).FirstOrDefault();
            return el;
        }

        /// <summary>
        /// Gets style reference for specified cell
        /// </summary>
        public static string GetCellStyle(XElement sheetData, string cellName)
        {                      
            XElement el = GetCell(sheetData, cellName);

            if (el != null)
            {
                XAttribute a = el.Attribute("s");
                return a != null ? a.Value : string.Empty;
            }

            return String.Empty;
        }

        /// <summary>
        /// Merges range of cells.
        /// </summary>
        /// <param name="docRoot">Document root element.</param>
        /// <param name="nm">Root namespace.</param>
        /// <param name="startColumn">Staring column name.</param>
        /// <param name="endColumn">End column name.</param>
        public static void MergeCells(XElement docRoot, XNamespace nm, string startColumn, string endColumn)
        {
            XElement mergeElement = docRoot.Elements(nm + "mergeCells").First();
            mergeElement.Add(
                new XElement(
                    nm + "mergeCell",
                    new XAttribute("ref", String.Format("{0}:{1}", startColumn, endColumn))
                )
            );
            int mergeCount = Int32.Parse(mergeElement.Attribute("count").Value);
            mergeElement.Attribute("count").Value = (++mergeCount).ToString();
        }
    }
}
