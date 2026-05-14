using System;
using Confirmit.CATI.Core.EmailReports;

namespace Confirmit.CATI.Core.EmailReports.Fakes
{
    public class StubIReportBuilder : IReportBuilder 
    {
        private IReportBuilder _inner;

        public StubIReportBuilder()
        {
            _inner = null;
        }

        public IReportBuilder Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IReport BuildReportDateTimeDateTimeDelegate(DateTime reportStartTime, DateTime reportEndTime);
        public BuildReportDateTimeDateTimeDelegate BuildReportDateTimeDateTime;

        IReport IReportBuilder.BuildReport(DateTime reportStartTime, DateTime reportEndTime)
        {


            if (BuildReportDateTimeDateTime != null)
            {
                return BuildReportDateTimeDateTime(reportStartTime, reportEndTime);
            } else if (_inner != null)
            {
                return ((IReportBuilder)_inner).BuildReport(reportStartTime, reportEndTime);
            }

            return default(IReport);
        }

        public delegate string ExportReportToDiskIReportStringDelegate(IReport report, string fileName);
        public ExportReportToDiskIReportStringDelegate ExportReportToDiskIReportString;

        string IReportBuilder.ExportReportToDisk(IReport report, string fileName)
        {


            if (ExportReportToDiskIReportString != null)
            {
                return ExportReportToDiskIReportString(report, fileName);
            } else if (_inner != null)
            {
                return ((IReportBuilder)_inner).ExportReportToDisk(report, fileName);
            }

            return default(string);
        }

        public delegate void PrepareDelegate();
        public PrepareDelegate Prepare;

        void IReportBuilder.Prepare()
        {

            if (Prepare != null)
            {
                Prepare();
            } else if (_inner != null)
            {
                ((IReportBuilder)_inner).Prepare();
            }
        }

        private bool _ShouldBeEncrypted;
        public Func<bool> ShouldBeEncryptedGet;
        public Action<bool> ShouldBeEncryptedSetBoolean;

        bool IReportBuilder.ShouldBeEncrypted
        {
            get
            {
                if (ShouldBeEncryptedGet != null)
                {
                    return ShouldBeEncryptedGet();
                } else if (_inner != null)
                {
                    return ((IReportBuilder)_inner).ShouldBeEncrypted;
                }

                if (ShouldBeEncryptedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ShouldBeEncrypted;
                }

                return default(bool);
            }

        }

    }
}