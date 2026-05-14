using System;
using BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data.Fakes
{
    public class StubIInterviewFormDataSourceService : IInterviewFormDataSourceService 
    {
        private IInterviewFormDataSourceService _inner;

        public StubIInterviewFormDataSourceService()
        {
            _inner = null;
        }

        public IInterviewFormDataSourceService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetFormValueFormDescBaseStringArrayOfStringDelegate(FormDescBase desc, string category, string[] loopQualifyer);
        public GetFormValueFormDescBaseStringArrayOfStringDelegate GetFormValueFormDescBaseStringArrayOfString;

        string IInterviewFormDataService.GetFormValue(FormDescBase desc, string category, string[] loopQualifyer)
        {


            if (GetFormValueFormDescBaseStringArrayOfString != null)
            {
                return GetFormValueFormDescBaseStringArrayOfString(desc, category, loopQualifyer);
            } else if (_inner != null)
            {
                return ((IInterviewFormDataService)_inner).GetFormValue(desc, category, loopQualifyer);
            }

            return default(string);
        }

        public delegate void SetFormValueFormDescBaseStringArrayOfStringStringDelegate(FormDescBase desc, string category, string[] loopQualifyer, string value);
        public SetFormValueFormDescBaseStringArrayOfStringStringDelegate SetFormValueFormDescBaseStringArrayOfStringString;

        void IInterviewFormDataService.SetFormValue(FormDescBase desc, string category, string[] loopQualifyer, string value)
        {

            if (SetFormValueFormDescBaseStringArrayOfStringString != null)
            {
                SetFormValueFormDescBaseStringArrayOfStringString(desc, category, loopQualifyer, value);
            } else if (_inner != null)
            {
                ((IInterviewFormDataService)_inner).SetFormValue(desc, category, loopQualifyer, value);
            }
        }

        public delegate void InitializeInt32Int32Delegate(int surveyId, int interviewId);
        public InitializeInt32Int32Delegate InitializeInt32Int32;

        void IInterviewFormDataSourceService.Initialize(int surveyId, int interviewId)
        {

            if (InitializeInt32Int32 != null)
            {
                InitializeInt32Int32(surveyId, interviewId);
            } else if (_inner != null)
            {
                ((IInterviewFormDataSourceService)_inner).Initialize(surveyId, interviewId);
            }
        }

        public delegate void CommitDelegate();
        public CommitDelegate Commit;

        void IInterviewFormDataSourceService.Commit()
        {

            if (Commit != null)
            {
                Commit();
            } else if (_inner != null)
            {
                ((IInterviewFormDataSourceService)_inner).Commit();
            }
        }

    }
}