using System;
using Confirmit.CATI.Supervisor.Core.Persons;

namespace Confirmit.CATI.Supervisor.Core.Persons.Fakes
{
    public class StubIChangeAutomaticSurveyService : IChangeAutomaticSurveyService 
    {
        private IChangeAutomaticSurveyService _inner;

        public StubIChangeAutomaticSurveyService()
        {
            _inner = null;
        }

        public IChangeAutomaticSurveyService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool ChangeSeamlessInt32Int32Delegate(int personId, int surveyId);
        public ChangeSeamlessInt32Int32Delegate ChangeSeamlessInt32Int32;

        bool IChangeAutomaticSurveyService.ChangeSeamless(int personId, int surveyId)
        {


            if (ChangeSeamlessInt32Int32 != null)
            {
                return ChangeSeamlessInt32Int32(personId, surveyId);
            } else if (_inner != null)
            {
                return ((IChangeAutomaticSurveyService)_inner).ChangeSeamless(personId, surveyId);
            }

            return default(bool);
        }

    }
}