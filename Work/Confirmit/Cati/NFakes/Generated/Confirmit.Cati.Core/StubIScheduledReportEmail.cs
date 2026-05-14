using System;
using Confirmit.CATI.Core.EmailReports;

namespace Confirmit.CATI.Core.EmailReports.Fakes
{
    public class StubIScheduledReportEmail : IScheduledReportEmail 
    {
        private IScheduledReportEmail _inner;

        public StubIScheduledReportEmail()
        {
            _inner = null;
        }

        public IScheduledReportEmail Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool IsSwitchedOnAndConfiguredAndItsTimeToSendDelegate();
        public IsSwitchedOnAndConfiguredAndItsTimeToSendDelegate IsSwitchedOnAndConfiguredAndItsTimeToSend;

        bool IScheduledReportEmail.IsSwitchedOnAndConfiguredAndItsTimeToSend()
        {


            if (IsSwitchedOnAndConfiguredAndItsTimeToSend != null)
            {
                return IsSwitchedOnAndConfiguredAndItsTimeToSend();
            } else if (_inner != null)
            {
                return ((IScheduledReportEmail)_inner).IsSwitchedOnAndConfiguredAndItsTimeToSend();
            }

            return default(bool);
        }

        public delegate bool IsLastDateSentRecentDelegate();
        public IsLastDateSentRecentDelegate IsLastDateSentRecent;

        bool IScheduledReportEmail.IsLastDateSentRecent()
        {


            if (IsLastDateSentRecent != null)
            {
                return IsLastDateSentRecent();
            } else if (_inner != null)
            {
                return ((IScheduledReportEmail)_inner).IsLastDateSentRecent();
            }

            return default(bool);
        }

        public delegate void UpdateReportLastSentTimeDelegate();
        public UpdateReportLastSentTimeDelegate UpdateReportLastSentTime;

        void IScheduledReportEmail.UpdateReportLastSentTime()
        {

            if (UpdateReportLastSentTime != null)
            {
                UpdateReportLastSentTime();
            } else if (_inner != null)
            {
                ((IScheduledReportEmail)_inner).UpdateReportLastSentTime();
            }
        }

        public delegate IReportBuilder GetReportBuilderDelegate();
        public GetReportBuilderDelegate GetReportBuilder;

        IReportBuilder IScheduledReportEmail.GetReportBuilder()
        {


            if (GetReportBuilder != null)
            {
                return GetReportBuilder();
            } else if (_inner != null)
            {
                return ((IScheduledReportEmail)_inner).GetReportBuilder();
            }

            return default(IReportBuilder);
        }

        private ReportType _ReportType;
        public Func<ReportType> ReportTypeGet;
        public Action<ReportType> ReportTypeSetReportType;

        ReportType IScheduledReportEmail.ReportType
        {
            get
            {
                if (ReportTypeGet != null)
                {
                    return ReportTypeGet();
                } else if (_inner != null)
                {
                    return ((IScheduledReportEmail)_inner).ReportType;
                }

                if (ReportTypeSetReportType == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ReportType;
                }

                return default(ReportType);
            }

        }

        private string _ReportDataExportFileName;
        public Func<string> ReportDataExportFileNameGet;
        public Action<string> ReportDataExportFileNameSetString;

        string IScheduledReportEmail.ReportDataExportFileName
        {
            get
            {
                if (ReportDataExportFileNameGet != null)
                {
                    return ReportDataExportFileNameGet();
                } else if (_inner != null)
                {
                    return ((IScheduledReportEmail)_inner).ReportDataExportFileName;
                }

                if (ReportDataExportFileNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ReportDataExportFileName;
                }

                return default(string);
            }

        }

        private string _ReportRecipients;
        public Func<string> ReportRecipientsGet;
        public Action<string> ReportRecipientsSetString;

        string IScheduledReportEmail.ReportRecipients
        {
            get
            {
                if (ReportRecipientsGet != null)
                {
                    return ReportRecipientsGet();
                } else if (_inner != null)
                {
                    return ((IScheduledReportEmail)_inner).ReportRecipients;
                }

                if (ReportRecipientsSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ReportRecipients;
                }

                return default(string);
            }

        }

    }
}