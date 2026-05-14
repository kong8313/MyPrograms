using System;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubIRedialNumberSaver : IRedialNumberSaver 
    {
        private IRedialNumberSaver _inner;

        public StubIRedialNumberSaver()
        {
            _inner = null;
        }

        public IRedialNumberSaver Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SaveAlternativeNumberInt32StringInt32Delegate(int surveySid, string currentPhoneNumber, int interviewId);
        public SaveAlternativeNumberInt32StringInt32Delegate SaveAlternativeNumberInt32StringInt32;

        void IRedialNumberSaver.SaveAlternativeNumber(int surveySid, string currentPhoneNumber, int interviewId)
        {

            if (SaveAlternativeNumberInt32StringInt32 != null)
            {
                SaveAlternativeNumberInt32StringInt32(surveySid, currentPhoneNumber, interviewId);
            } else if (_inner != null)
            {
                ((IRedialNumberSaver)_inner).SaveAlternativeNumber(surveySid, currentPhoneNumber, interviewId);
            }
        }

    }
}