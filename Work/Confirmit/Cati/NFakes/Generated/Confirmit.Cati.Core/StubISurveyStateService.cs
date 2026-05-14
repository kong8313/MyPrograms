using System;
using Confirmit.CATI.Core.Services.Survey;

namespace Confirmit.CATI.Core.Services.Survey.Fakes
{
    public class StubISurveyStateService : ISurveyStateService 
    {
        private ISurveyStateService _inner;

        public StubISurveyStateService()
        {
            _inner = null;
        }

        public ISurveyStateService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CloseSurveyInt32Delegate(int sid);
        public CloseSurveyInt32Delegate CloseSurveyInt32;

        void ISurveyStateService.CloseSurvey(int sid)
        {

            if (CloseSurveyInt32 != null)
            {
                CloseSurveyInt32(sid);
            } else if (_inner != null)
            {
                ((ISurveyStateService)_inner).CloseSurvey(sid);
            }
        }

        public delegate void ShutdownSurveyInt32Delegate(int sid);
        public ShutdownSurveyInt32Delegate ShutdownSurveyInt32;

        void ISurveyStateService.ShutdownSurvey(int sid)
        {

            if (ShutdownSurveyInt32 != null)
            {
                ShutdownSurveyInt32(sid);
            } else if (_inner != null)
            {
                ((ISurveyStateService)_inner).ShutdownSurvey(sid);
            }
        }

        public delegate void OpenInt32Delegate(int sid);
        public OpenInt32Delegate OpenInt32;

        void ISurveyStateService.Open(int sid)
        {

            if (OpenInt32 != null)
            {
                OpenInt32(sid);
            } else if (_inner != null)
            {
                ((ISurveyStateService)_inner).Open(sid);
            }
        }

    }
}