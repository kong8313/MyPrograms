using System;

namespace Confirmit.CATI.Core.Repositories
{
    public class SurveyNotFoundException : Exception
    {
        private readonly string _surveyName;
        private readonly int _surveyId;

        public SurveyNotFoundException(int surveyId) : base(string.Format("Survey {0} is not found.", surveyId))
        {
            _surveyId = surveyId;
        }

        public SurveyNotFoundException(string surveyName): base(string.Format("Survey {0} is not found.", surveyName))
        {
            _surveyName = surveyName;
        }

        public int SurveyId
        {
            get { return _surveyId; }
        }

        public string SurveyName
        {
            get { return _surveyName; }
        }
    }
}