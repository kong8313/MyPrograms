using System;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubISurveyCleaningService : ISurveyCleaningService 
    {
        private ISurveyCleaningService _inner;

        public StubISurveyCleaningService()
        {
            _inner = null;
        }

        public ISurveyCleaningService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CleanAllUnusedSurveysDelegate();
        public CleanAllUnusedSurveysDelegate CleanAllUnusedSurveys;

        void ISurveyCleaningService.CleanAllUnusedSurveys()
        {

            if (CleanAllUnusedSurveys != null)
            {
                CleanAllUnusedSurveys();
            } else if (_inner != null)
            {
                ((ISurveyCleaningService)_inner).CleanAllUnusedSurveys();
            }
        }

    }
}