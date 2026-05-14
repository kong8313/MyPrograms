using Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport;
using System.Collections.Generic;
using System.Linq;
using Telerik.Reporting;
using Telerik.Reporting.Drawing;

namespace Confirmit.CATI.Core.Reports
{
    public class InterviewerProductivityReportAppearanceModifier
    {
        public void ChangeOrientationToLandscape(
            Report report,
            ReportItemBase headerArea,
            ReportItemBase columnHeaders,
            ReportItemBase columns,
            ReportItemBase total,
            TextBox pagingTextBox,
            bool extendColumns)
        {
            report.PageSettings.Landscape = true;

            var reportTitleBox = (TextBox)headerArea.Items.FirstOrDefault(x => x.Name == "reportTitle");
            var reportDateBox = (TextBox)headerArea.Items.FirstOrDefault(x => x.Name == "reportDate");

            if (reportTitleBox != null)
            {
                reportTitleBox.Location = GetCenteredLocation(reportTitleBox, report);
            }

            if (reportDateBox != null)
            {
                reportDateBox.Location = GetCenteredLocation(reportDateBox, report);
            }

            if (extendColumns)
            {
                var incrementInCm = GetIncrementInCm(columnHeaders.Items, report);

                ExtendColumns(columnHeaders.Items, incrementInCm);
                ExtendColumns(columns.Items, incrementInCm);
                ExtendColumns(total.Items, incrementInCm);

                // Set new paging position
                pagingTextBox.Location = new PointU(
                    pagingTextBox.Location.X + Unit.Cm(columnHeaders.Items.Count * incrementInCm),
                    pagingTextBox.Location.Y);
            }
            else
            {
                pagingTextBox.Location = new PointU(
                    pagingTextBox.Location.X + Unit.Cm(6),
                    pagingTextBox.Location.Y);
            }
        }

        private float GetIncrementInCm(ReportItemBase.ItemCollection items, Report report)
        {
            var widths = new Unit();
            foreach (var reportItemBase in items)
            {
                var column = (TextBox)reportItemBase;
                widths += column.Width;
            }

            var freeSpaceRest = report.PageSettings.PaperSize.Height - widths - Unit.Mm(25);

            return (freeSpaceRest / items.Count).ChangeType(UnitType.Cm).Value;
        }

        private PointU GetCenteredLocation(TextBox reportTitleBox, Report report)
        {
            return new PointU(report.PageSettings.PaperSize.Height / 2 - reportTitleBox.Width / 2, reportTitleBox.Location.Y);
        }

        private void ExtendColumns(IEnumerable<ReportItemBase> columns, float incrementInCm)
        {
            var colIndex = 0;

            foreach (var reportItemBase in columns.OrderBy(x => ((TextBox)x).Left))
            {
                var item = (TextBox)reportItemBase;

                if (colIndex != 0)
                {
                    item.Location = new PointU(item.Location.X + Unit.Cm(colIndex * incrementInCm), item.Location.Y);
                }

                item.Width += Unit.Cm(incrementInCm);
                colIndex++;
            }
        }

        public void AddCaptionText(GroupHeaderSection groupHeaderSectionArea, ReportColumnInfo reportColumnInfo)
        {
            AddTextItem(
                groupHeaderSectionArea.Items, 
                $"CaptionOf{reportColumnInfo.FieldName}", 
                reportColumnInfo.Caption,
                reportColumnInfo.ValueFormat,
                new PointU(Unit.Cm(reportColumnInfo.LocationX), Unit.Cm(0.04)), 
                new SizeU(Unit.Cm(reportColumnInfo.ColumnWidth), Unit.Cm(1.4)), 
                true, true, true, 7D);
        }

        public void AddDetailText(DetailSection detailSectionArea, ReportColumnInfo reportColumnInfo)
        {
            AddTextItem(
                detailSectionArea.Items, 
                reportColumnInfo.FieldName, 
                reportColumnInfo.DetailValue,
                reportColumnInfo.ValueFormat,
                new PointU(Unit.Cm(reportColumnInfo.LocationX), Unit.Cm(0)),
                new SizeU(Unit.Cm(reportColumnInfo.ColumnWidth), detailSectionArea.Height), 
                false, false, false, 8D);
        }

        public void AddFooterText(ReportFooterSection footerSectionArea, ReportColumnInfo reportColumnInfo)
        {
            AddTextItem(
                footerSectionArea.Items, 
                $"SumOf{reportColumnInfo.FieldName}",
                reportColumnInfo.FooterValue,
                reportColumnInfo.ValueFormat,
                new PointU(Unit.Cm(reportColumnInfo.LocationX), Unit.Cm(0)), 
                new SizeU(Unit.Cm(reportColumnInfo.ColumnWidth), footerSectionArea.Height), 
                true, false, false, 8D);
        }

        private void AddTextItem(
            ReportItemBase.ItemCollection columnItems, string name, string value, string format, PointU location, SizeU size, bool isBold, bool canGrow, bool canShrink, double fontSize)
        {
            var textBox = new TextBox
            {
                CanGrow = canGrow,
                CanShrink = canShrink,
                Format = format,
                Location = location,
                Name = name,
                Size = size,
                Value = value
            };
            textBox.Style.BackgroundColor = System.Drawing.Color.Transparent;
            textBox.Style.BorderColor.Default = System.Drawing.Color.Black;
            textBox.Style.BorderStyle.Default = BorderType.None;
            textBox.Style.Color = System.Drawing.Color.Black;
            textBox.Style.Font.Bold = isBold;
            textBox.Style.Font.Italic = false;
            textBox.Style.Font.Name = "Verdana";
            textBox.Style.Font.Size = Unit.Point(fontSize);
            textBox.Style.Font.Strikeout = false;
            textBox.Style.Font.Underline = false;
            textBox.Style.TextAlign = HorizontalAlign.Left;
            textBox.Style.Visible = true;

            columnItems.Add(textBox);
        }
    }
}
