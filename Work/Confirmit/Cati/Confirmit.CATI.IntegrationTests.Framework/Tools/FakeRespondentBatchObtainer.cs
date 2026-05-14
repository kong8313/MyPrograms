using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class FakeRespondentBatchObtainer : IRespondentBatchObtainer
    {
        private readonly IEnumerable<RespondentRecord> _respondentRecords;

        public FakeRespondentBatchObtainer()
            : this(1, 3, Enumerable.Repeat(1, 3))
        {
        }

        public FakeRespondentBatchObtainer(int startRespId, int count, IEnumerable<int> timeZones, int[] resources = null)
        {
            _respondentRecords = GenerateRespondentRecords(startRespId, count, timeZones, resources);
        }

        public FakeRespondentBatchObtainer(IEnumerable<RespondentRecord> respondentRecords)
        {
            _respondentRecords = respondentRecords;
        }

        public RespondentRecord[] GetRespondentBatchPartition(BvSurveyEntity survey, int batchId, int startRangeOfInterviewId, int partitionSize, bool isSampleUpdate)
        {
            return _respondentRecords.Where(x => x.InterviewId >= startRangeOfInterviewId).OrderBy(y => y.InterviewId).Take(partitionSize).ToArray();
        }

        public RespondentRecord[] GetRespondentsForSynchronization(BvSurveyEntity survey, int partitionSize)
        {
            throw new NotImplementedException();
        }

        public int GetRespondentBatchId(BvSurveyEntity survey, int respId)
        {
            throw new NotImplementedException();
        }


        private static IEnumerable<RespondentRecord> GenerateRespondentRecords(int startRespId, int count, IEnumerable<int> timeZones, int[] resource = null)
        {
            timeZones = timeZones ?? Enumerable.Repeat(1, count);
            var zones = timeZones.ToArray();

            var result = new List<RespondentRecord>();
            for (int i = startRespId, resCnt = 0; i < startRespId + count; ++i)
            {
                var timeZoneId = zones.ElementAtOrDefault(i - startRespId);
                result.Add(new RespondentRecord
                {
                    Sid = i.ToString(CultureInfo.InvariantCulture),
                    InterviewId = i,
                    RespondentName = "resp" + i,
                    RespondentPhone = i.ToString(CultureInfo.InvariantCulture),
                    LastCallTime = null,
                    TotalDuration = i,
                    ExtensionNumber = i.ToString(CultureInfo.InvariantCulture),
                    DialAttempts = i,
                    TimeZoneId = timeZoneId,
                    LastChannelId = (byte)i,
                    Resource = resource == null ? i : resource[resCnt],
                    IsClosedCell = false,
                    CatiCallTime = string.Empty,
                    CatiCallExpirationTime = string.Empty,
                    CatiExtendedStatus = string.Empty
                });
               
                resCnt = resource == null ? 0 : resCnt == resource.Length - 1 ? 0 : resCnt+1;
            }

            return result.AsEnumerable();
        }
    }
}