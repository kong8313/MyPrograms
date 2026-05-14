using System;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data.Fakes
{
    public class StubIInterviewRespondentDataSourceService : IInterviewRespondentDataSourceService 
    {
        private IInterviewRespondentDataSourceService _inner;

        public StubIInterviewRespondentDataSourceService()
        {
            _inner = null;
        }

        public IInterviewRespondentDataSourceService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Object GetRespondentValueStringDelegate(string fieldName);
        public GetRespondentValueStringDelegate GetRespondentValueString;

        Object IInterviewRespondentDataService.GetRespondentValue(string fieldName)
        {


            if (GetRespondentValueString != null)
            {
                return GetRespondentValueString(fieldName);
            } else if (_inner != null)
            {
                return ((IInterviewRespondentDataService)_inner).GetRespondentValue(fieldName);
            }

            return default(Object);
        }

        public delegate void SetRespondentValueStringObjectDelegate(string fieldName, Object value);
        public SetRespondentValueStringObjectDelegate SetRespondentValueStringObject;

        void IInterviewRespondentDataService.SetRespondentValue(string fieldName, Object value)
        {

            if (SetRespondentValueStringObject != null)
            {
                SetRespondentValueStringObject(fieldName, value);
            } else if (_inner != null)
            {
                ((IInterviewRespondentDataService)_inner).SetRespondentValue(fieldName, value);
            }
        }

        public delegate string GetDiffDelegate();
        public GetDiffDelegate GetDiff;

        string IInterviewRespondentDataService.GetDiff()
        {


            if (GetDiff != null)
            {
                return GetDiff();
            } else if (_inner != null)
            {
                return ((IInterviewRespondentDataService)_inner).GetDiff();
            }

            return default(string);
        }

        public delegate void InitializeInt32Int32Delegate(int surveyId, int interviewId);
        public InitializeInt32Int32Delegate InitializeInt32Int32;

        void IInterviewRespondentDataSourceService.Initialize(int surveyId, int interviewId)
        {

            if (InitializeInt32Int32 != null)
            {
                InitializeInt32Int32(surveyId, interviewId);
            } else if (_inner != null)
            {
                ((IInterviewRespondentDataSourceService)_inner).Initialize(surveyId, interviewId);
            }
        }

        public delegate void CommitDelegate();
        public CommitDelegate Commit;

        void IInterviewRespondentDataSourceService.Commit()
        {

            if (Commit != null)
            {
                Commit();
            } else if (_inner != null)
            {
                ((IInterviewRespondentDataSourceService)_inner).Commit();
            }
        }

    }
}