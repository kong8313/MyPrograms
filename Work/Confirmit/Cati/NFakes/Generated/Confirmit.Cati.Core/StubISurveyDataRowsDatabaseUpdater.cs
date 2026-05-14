using System;
using Confirmit.CATI.Core.Services.Survey.Data;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data.Fakes
{
    public class StubISurveyDataRowsDatabaseUpdater : ISurveyDataRowsDatabaseUpdater 
    {
        private ISurveyDataRowsDatabaseUpdater _inner;

        public StubISurveyDataRowsDatabaseUpdater()
        {
            _inner = null;
        }

        public ISurveyDataRowsDatabaseUpdater Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool UpdateInt32Int32ArrayOfSurveyDataRowCacheDelegate(int surveyId, int interviewId, SurveyDataRowCache[] rows);
        public UpdateInt32Int32ArrayOfSurveyDataRowCacheDelegate UpdateInt32Int32ArrayOfSurveyDataRowCache;

        bool ISurveyDataRowsDatabaseUpdater.Update(int surveyId, int interviewId, SurveyDataRowCache[] rows)
        {


            if (UpdateInt32Int32ArrayOfSurveyDataRowCache != null)
            {
                return UpdateInt32Int32ArrayOfSurveyDataRowCache(surveyId, interviewId, rows);
            } else if (_inner != null)
            {
                return ((ISurveyDataRowsDatabaseUpdater)_inner).Update(surveyId, interviewId, rows);
            }

            return default(bool);
        }

        public delegate bool ProcessInt32Int32ArrayOfSurveyDataRowCacheDelegate(int surveyId, int interviewId, SurveyDataRowCache[] rows);
        public ProcessInt32Int32ArrayOfSurveyDataRowCacheDelegate ProcessInt32Int32ArrayOfSurveyDataRowCache;

        bool ISurveyDataRowsDatabaseUpdater.Process(int surveyId, int interviewId, SurveyDataRowCache[] rows)
        {


            if (ProcessInt32Int32ArrayOfSurveyDataRowCache != null)
            {
                return ProcessInt32Int32ArrayOfSurveyDataRowCache(surveyId, interviewId, rows);
            } else if (_inner != null)
            {
                return ((ISurveyDataRowsDatabaseUpdater)_inner).Process(surveyId, interviewId, rows);
            }

            return default(bool);
        }

    }
}