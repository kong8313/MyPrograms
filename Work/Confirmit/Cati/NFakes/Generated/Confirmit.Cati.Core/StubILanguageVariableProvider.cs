using System;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubILanguageVariableProvider : ILanguageVariableProvider 
    {
        private ILanguageVariableProvider _inner;

        public StubILanguageVariableProvider()
        {
            _inner = null;
        }

        public ILanguageVariableProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int? GetLanguageForInterviewInt32Int32Delegate(int surveySid, int interviewId);
        public GetLanguageForInterviewInt32Int32Delegate GetLanguageForInterviewInt32Int32;

        int? ILanguageVariableProvider.GetLanguageForInterview(int surveySid, int interviewId)
        {


            if (GetLanguageForInterviewInt32Int32 != null)
            {
                return GetLanguageForInterviewInt32Int32(surveySid, interviewId);
            } else if (_inner != null)
            {
                return ((ILanguageVariableProvider)_inner).GetLanguageForInterview(surveySid, interviewId);
            }

            return default(int?);
        }

    }
}