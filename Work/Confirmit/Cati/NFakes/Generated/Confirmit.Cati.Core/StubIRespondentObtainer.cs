using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;

namespace Confirmit.CATI.Core.Services.InterviewServiceImplementation.Fakes
{
    public class StubIRespondentObtainer : IRespondentObtainer 
    {
        private IRespondentObtainer _inner;

        public StubIRespondentObtainer()
        {
            _inner = null;
        }

        public IRespondentObtainer Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate RespondentRecord GetRespondentBvSurveyEntityInt32Delegate(BvSurveyEntity survey, int respId);
        public GetRespondentBvSurveyEntityInt32Delegate GetRespondentBvSurveyEntityInt32;

        RespondentRecord IRespondentObtainer.GetRespondent(BvSurveyEntity survey, int respId)
        {


            if (GetRespondentBvSurveyEntityInt32 != null)
            {
                return GetRespondentBvSurveyEntityInt32(survey, respId);
            } else if (_inner != null)
            {
                return ((IRespondentObtainer)_inner).GetRespondent(survey, respId);
            }

            return default(RespondentRecord);
        }

    }
}