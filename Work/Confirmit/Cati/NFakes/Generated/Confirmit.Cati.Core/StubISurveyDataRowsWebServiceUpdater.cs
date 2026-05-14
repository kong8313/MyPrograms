using System;
using Confirmit.CATI.Core.Services.Survey.Data;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data.Fakes
{
    public class StubISurveyDataRowsWebServiceUpdater : ISurveyDataRowsWebServiceUpdater 
    {
        private ISurveyDataRowsWebServiceUpdater _inner;

        public StubISurveyDataRowsWebServiceUpdater()
        {
            _inner = null;
        }

        public ISurveyDataRowsWebServiceUpdater Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void UpdateInt32Int32ArrayOfSurveyDataRowCacheDelegate(int surveyId, int interviewId, SurveyDataRowCache[] rows);
        public UpdateInt32Int32ArrayOfSurveyDataRowCacheDelegate UpdateInt32Int32ArrayOfSurveyDataRowCache;

        void ISurveyDataRowsWebServiceUpdater.Update(int surveyId, int interviewId, SurveyDataRowCache[] rows)
        {

            if (UpdateInt32Int32ArrayOfSurveyDataRowCache != null)
            {
                UpdateInt32Int32ArrayOfSurveyDataRowCache(surveyId, interviewId, rows);
            } else if (_inner != null)
            {
                ((ISurveyDataRowsWebServiceUpdater)_inner).Update(surveyId, interviewId, rows);
            }
        }

    }
}