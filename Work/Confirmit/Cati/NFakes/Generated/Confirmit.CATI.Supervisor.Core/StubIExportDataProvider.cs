using System;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Export;
using System.Collections;

namespace Confirmit.CATI.Supervisor.Core.Export.Fakes
{
    public class StubIExportDataProvider : IExportDataProvider 
    {
        private IExportDataProvider _inner;

        public StubIExportDataProvider()
        {
            _inner = null;
        }

        public IExportDataProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerator<IExportRecordProvider> GetEnumeratorDelegate();
        public GetEnumeratorDelegate GetEnumerator;

        IEnumerator<IExportRecordProvider> IEnumerable<IExportRecordProvider>.GetEnumerator()
        {


            if (GetEnumerator != null)
            {
                return GetEnumerator();
            } else if (_inner != null)
            {
                return ((IEnumerable<IExportRecordProvider>)_inner).GetEnumerator();
            }

            return default(IEnumerator<IExportRecordProvider>);
        }

        public delegate IEnumerator GetEnumeratorDelegate1();
        public GetEnumeratorDelegate1 GetEnumerator1;

        IEnumerator IEnumerable.GetEnumerator()
        {


            if (GetEnumerator1 != null)
            {
                return GetEnumerator1();
            } else if (_inner != null)
            {
                return ((IEnumerable)_inner).GetEnumerator();
            }

            return default(IEnumerator);
        }

        public delegate string GetParameterStringDelegate(string key);
        public GetParameterStringDelegate GetParameterString;

        string IExportDataProvider.GetParameter(string key)
        {


            if (GetParameterString != null)
            {
                return GetParameterString(key);
            } else if (_inner != null)
            {
                return ((IExportDataProvider)_inner).GetParameter(key);
            }

            return default(string);
        }

    }
}