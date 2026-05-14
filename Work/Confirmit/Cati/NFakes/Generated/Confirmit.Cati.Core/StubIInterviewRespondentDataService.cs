using System;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data.Fakes
{
    public class StubIInterviewRespondentDataService : IInterviewRespondentDataService 
    {
        private IInterviewRespondentDataService _inner;

        public StubIInterviewRespondentDataService()
        {
            _inner = null;
        }

        public IInterviewRespondentDataService Inner
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

    }
}