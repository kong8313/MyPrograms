// Disable missing XML comment for publicly visible type or member
#pragma warning disable 1591

using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.DAL.Handmade.Entity.Table
{
    [Serializable]
    public class BvPersonDeferredMonitoringPartEntity
    {
        ///<summary>
        /// INT, NOT NULL
        ///</summary>
        public int ID { get; set; }

        ///<summary>
        /// INT, NOT NULL
        ///</summary>
        public int PersonSID { get; set; }

        ///<summary>
        /// INT, NOT NULL
        ///</summary>
        public int InterviewID { get; set; }

        ///<summary>
        /// INT, NOT NULL
        ///</summary>
        public int SurveySID { get; set; }

        ///<summary>
        /// DATETIME, NOT NULL
        ///</summary>
        public DateTime TimeStamp { get; set; }
        public DateTime RecordCreationTime { get; set; }

        ///<summary>
        /// BIT, NOT NULL, Default: ((1))
        ///</summary>
        public bool IsRecording { get; set; }

        ///<summary>
        /// BIT, NOT NULL, Default: ((0))
        ///</summary>
        public bool IsComplete { get; set; }

        ///<summary>
        /// DATETIME, NOT NULL, Default: (getutcdate())
        ///</summary>
        public DateTime ClientTimeUtc { get; set; }

        ///<summary>
        /// DATETIME, NOT NULL, Default: (getutcdate())
        ///</summary>
        public DateTime ServerTimeUtc { get; set; }

        /// <summary>
        /// INT, NULL
        /// </summary>
        public int? CallID { get; set; }

        /// <summary>
        /// INT, NULL
        /// </summary>
        public int? ExtendedStatus { get; set; }

        /// <summary>
        /// INT, NOT NULL, Default: 0
        /// </summary>
        public int InterviewDuration { get; set; }
		
		/// <summary>
        /// DATETIME, NULL
        /// </summary>
        public DateTime? ScreenRecordingStartTime { get; set; }

        public BvPersonDeferredMonitoringPartEntity()
        {
            ID = 0;
            PersonSID = 0;
            InterviewID = 0;
            SurveySID = 0;
            TimeStamp = DateTime.MinValue;
            ClientTimeUtc = DateTime.MinValue;
            ServerTimeUtc = DateTime.MinValue;
            InterviewDuration = 0;
        }

        public BvPersonDeferredMonitoringPartEntity(BvPersonDeferredMonitoringEntity entity)
        {
            ID = entity.ID;
            PersonSID = entity.PersonSID;
            InterviewID = entity.InterviewID;
            SurveySID = entity.SurveySID;
            TimeStamp = entity.TimeStamp;
            RecordCreationTime = entity.RecordCreationTime;
            ClientTimeUtc = entity.ClientTimeUtc;
            ServerTimeUtc = entity.ServerTimeUtc;
            CallID = entity.CallID;
            ExtendedStatus = entity.ExtendedStatus;
            InterviewDuration = entity.InterviewDuration;
        }

    }
}
