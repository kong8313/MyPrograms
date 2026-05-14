using System;
using System.Data;

using Confirmit.CATI.Core.DAL.Framework.BulkCopy;

namespace Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging
{
    public class InterviewerActivityEventsBulkCopySerializer : BulkCopyEntitySerializerBase<IInterviewerActivityEventBase>
    {
        public override string TableName 
        {
            get { return "CatiInterviewerActivity"; }
        }

        public override DataColumn[]  GetTableColumns()
        {
            return new[]
                       {
                           new DataColumn("ID", typeof (int)) {AllowDBNull = false, AutoIncrement = true},
                           new DataColumn("EventTypeId", typeof (int)) {AllowDBNull = false},
                           new DataColumn("EventTypeName", typeof (string)) {AllowDBNull = false},
                           new DataColumn("ServerName", typeof (string)) {AllowDBNull = false},
                           new DataColumn("CompanyId", typeof (int)) {AllowDBNull = false},
                           new DataColumn("SurveyId", typeof (int)) {AllowDBNull = true},
                           new DataColumn("SurveyName", typeof (string)) {AllowDBNull = true},
                           new DataColumn("InterviewerSid", typeof (int)) {AllowDBNull = false},
                           new DataColumn("StartTime", typeof (DateTime)) {AllowDBNull = false},
                           new DataColumn("FinishTime", typeof (DateTime)) {AllowDBNull = false},
                           new DataColumn("Duration", typeof (int)) {AllowDBNull = false},
                           new DataColumn("PhoneNumber", typeof (string)) {AllowDBNull = true},
                           new DataColumn("Details", typeof (string)) {AllowDBNull = true},
                           new DataColumn("InterviewId", typeof (int)) {AllowDBNull = true}
                       };
        }

        public override void SerializeEvent2DataRow(IInterviewerActivityEventBase entity, DataRow row)
        {
            row.ItemArray = new object[]
                                {
                                    0,
                                    (int)entity.EventTypeId,
                                    entity.EventTypeName,
                                    entity.ServerName,
                                    entity.CompanyId,
                                    entity.SurveySid,
                                    entity.SurveyName,
                                    entity.InterviewerSid,
                                    entity.StartTime,
                                    entity.FinishTime,
                                    entity.Duration.TotalMilliseconds,
                                    entity.PhoneNumber,
                                    entity.DetailsToXml(),
                                    entity.InterviewId
                                };
        }
    }
}
