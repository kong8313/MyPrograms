using System;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface ISurveyPublishService
    {
        void OnLaunchSurvey(int sid, Action<string> taskLog = null);
        void OnDeleteSurvey(int sid);
    }
}