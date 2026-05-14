using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;

namespace Confirmit.CATI.Core.Services.CleaningService
{
    public class CleaningServiceEmailInfo
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string NotificationEmail { get; set; }
        public DateTime? LastTouchTime { get; set; }
        public int? SampleSize { get; set; }
        public string Creator { get; set; }

        public CleaningServiceEmailInfo(BvSpSurveyCleanup_GetSurveysWhichAreReadyForNoticeEntity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Description = entity.Description;
            NotificationEmail = entity.NotificationEmail;
            LastTouchTime = entity.LastTouchTime;
            SampleSize = entity.SampleSize;
        }

        public CleaningServiceEmailInfo(BvSpSurveyCleanup_GetSurveysWhichAreReadyForCleanupEntity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Description = entity.Description;
            NotificationEmail = entity.NotificationEmail;
            LastTouchTime = entity.LastTouchTime;
            SampleSize = entity.SampleSize;
        }
    }
}