using System;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubISurveyConnectionStringProvider : ISurveyConnectionStringProvider 
    {
        private ISurveyConnectionStringProvider _inner;

        public StubISurveyConnectionStringProvider()
        {
            _inner = null;
        }

        public ISurveyConnectionStringProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate SurveyConnectionInfo GetConnectionInfoInt32BooleanDelegate(int surveyId, bool updateLastConnectionTime);
        public GetConnectionInfoInt32BooleanDelegate GetConnectionInfoInt32Boolean;

        SurveyConnectionInfo ISurveyConnectionStringProvider.GetConnectionInfo(int surveyId, bool updateLastConnectionTime)
        {


            if (GetConnectionInfoInt32Boolean != null)
            {
                return GetConnectionInfoInt32Boolean(surveyId, updateLastConnectionTime);
            } else if (_inner != null)
            {
                return ((ISurveyConnectionStringProvider)_inner).GetConnectionInfo(surveyId, updateLastConnectionTime);
            }

            return default(SurveyConnectionInfo);
        }

    }
}