using System;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data.Fakes
{
    public class StubISurveyDatabaseInfoProvider : ISurveyDatabaseInfoProvider 
    {
        private ISurveyDatabaseInfoProvider _inner;

        public StubISurveyDatabaseInfoProvider()
        {
            _inner = null;
        }

        public ISurveyDatabaseInfoProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate SurveyDatabaseFormInfo GetFormInfoInt32StringDelegate(int surveyId, string name);
        public GetFormInfoInt32StringDelegate GetFormInfoInt32String;

        SurveyDatabaseFormInfo ISurveyDatabaseInfoProvider.GetFormInfo(int surveyId, string name)
        {


            if (GetFormInfoInt32String != null)
            {
                return GetFormInfoInt32String(surveyId, name);
            } else if (_inner != null)
            {
                return ((ISurveyDatabaseInfoProvider)_inner).GetFormInfo(surveyId, name);
            }

            return default(SurveyDatabaseFormInfo);
        }

        public delegate IEnumerable<SurveyDatabaseFieldInfo> GetRespondentFieldsInfoInt32Delegate(int surveyId);
        public GetRespondentFieldsInfoInt32Delegate GetRespondentFieldsInfoInt32;

        IEnumerable<SurveyDatabaseFieldInfo> ISurveyDatabaseInfoProvider.GetRespondentFieldsInfo(int surveyId)
        {


            if (GetRespondentFieldsInfoInt32 != null)
            {
                return GetRespondentFieldsInfoInt32(surveyId);
            } else if (_inner != null)
            {
                return ((ISurveyDatabaseInfoProvider)_inner).GetRespondentFieldsInfo(surveyId);
            }

            return default(IEnumerable<SurveyDatabaseFieldInfo>);
        }

    }
}