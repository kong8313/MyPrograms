using System;
using Confirmit.CATI.Backend.WebApiServices;
using System.Collections.Generic;

namespace Confirmit.CATI.Backend.WebApiServices.Fakes
{
    public class StubIRequestExecutionLog : IRequestExecutionLog 
    {
        private IRequestExecutionLog _inner;

        public StubIRequestExecutionLog()
        {
            _inner = null;
        }

        public IRequestExecutionLog Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AddEntryStringDelegate(string entry);
        public AddEntryStringDelegate AddEntryString;

        void IRequestExecutionLog.AddEntry(string entry)
        {

            if (AddEntryString != null)
            {
                AddEntryString(entry);
            } else if (_inner != null)
            {
                ((IRequestExecutionLog)_inner).AddEntry(entry);
            }
        }

        public delegate IEnumerable<string> GetEntriesDelegate();
        public GetEntriesDelegate GetEntries;

        IEnumerable<string> IRequestExecutionLog.GetEntries()
        {


            if (GetEntries != null)
            {
                return GetEntries();
            } else if (_inner != null)
            {
                return ((IRequestExecutionLog)_inner).GetEntries();
            }

            return default(IEnumerable<string>);
        }

    }
}