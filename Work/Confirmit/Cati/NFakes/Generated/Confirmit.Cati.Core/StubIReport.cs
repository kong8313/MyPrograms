using System;
using Confirmit.CATI.Core.EmailReports;
using System.Collections;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.EmailReports.Fakes
{
    public class StubIReport : IReport 
    {
        private IReport _inner;

        public StubIReport()
        {
            _inner = null;
        }

        public IReport Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private string _Title;
        public Func<string> TitleGet;
        public Action<string> TitleSetString;

        string IReport.Title
        {
            get
            {
                if (TitleGet != null)
                {
                    return TitleGet();
                } else if (_inner != null)
                {
                    return ((IReport)_inner).Title;
                }

                if (TitleSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Title;
                }

                return default(string);
            }

        }

        private string _Name;
        public Func<string> NameGet;
        public Action<string> NameSetString;

        string IReport.Name
        {
            get
            {
                if (NameGet != null)
                {
                    return NameGet();
                } else if (_inner != null)
                {
                    return ((IReport)_inner).Name;
                }

                if (NameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Name;
                }

                return default(string);
            }

        }

        private IEnumerable _ReportDataSource;
        public Func<IEnumerable> ReportDataSourceGet;
        public Action<IEnumerable> ReportDataSourceSetIEnumerable;

        IEnumerable IReport.ReportDataSource
        {
            get
            {
                if (ReportDataSourceGet != null)
                {
                    return ReportDataSourceGet();
                } else if (_inner != null)
                {
                    return ((IReport)_inner).ReportDataSource;
                }

                if (ReportDataSourceSetIEnumerable == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ReportDataSource;
                }

                return default(IEnumerable);
            }

        }

        private ICollection<KeyValuePair<string, Object>> _ReportParametersCollection;
        public Func<ICollection<KeyValuePair<string, Object>>> ReportParametersCollectionGet;
        public Action<ICollection<KeyValuePair<string, Object>>> ReportParametersCollectionSetICollectionOfKeyValuePairOfStringObject;

        ICollection<KeyValuePair<string, Object>> IReport.ReportParametersCollection
        {
            get
            {
                if (ReportParametersCollectionGet != null)
                {
                    return ReportParametersCollectionGet();
                } else if (_inner != null)
                {
                    return ((IReport)_inner).ReportParametersCollection;
                }

                if (ReportParametersCollectionSetICollectionOfKeyValuePairOfStringObject == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ReportParametersCollection;
                }

                return default(ICollection<KeyValuePair<string, Object>>);
            }

        }

    }
}