namespace Confirmit.CATI.Core.Reports
{

    /// <summary>
    /// Summary description for CatiProductivityReport.
    /// </summary>
    public partial class InterviewerProductivityReport : Telerik.Reporting.Report
    {
        public InterviewerProductivityReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public void ChangeOrientationToLandscape()
        {
            new InterviewerProductivityReportAppearanceModifier().ChangeOrientationToLandscape(
                this,
                Area1,
                groupHeaderSection,
                Area3,
                Area4,
                PageNofM1,
                true);
        }
    }
}