using System;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubISurveyPublishService : ISurveyPublishService 
    {
        private ISurveyPublishService _inner;

        public StubISurveyPublishService()
        {
            _inner = null;
        }

        public ISurveyPublishService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void OnLaunchSurveyInt32ActionOfStringDelegate(int sid, Action<string> taskLog);
        public OnLaunchSurveyInt32ActionOfStringDelegate OnLaunchSurveyInt32ActionOfString;

        void ISurveyPublishService.OnLaunchSurvey(int sid, Action<string> taskLog)
        {

            if (OnLaunchSurveyInt32ActionOfString != null)
            {
                OnLaunchSurveyInt32ActionOfString(sid, taskLog);
            } else if (_inner != null)
            {
                ((ISurveyPublishService)_inner).OnLaunchSurvey(sid, taskLog);
            }
        }

        public delegate void OnDeleteSurveyInt32Delegate(int sid);
        public OnDeleteSurveyInt32Delegate OnDeleteSurveyInt32;

        void ISurveyPublishService.OnDeleteSurvey(int sid)
        {

            if (OnDeleteSurveyInt32 != null)
            {
                OnDeleteSurveyInt32(sid);
            } else if (_inner != null)
            {
                ((ISurveyPublishService)_inner).OnDeleteSurvey(sid);
            }
        }

    }
}