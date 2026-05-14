using System.Collections.Generic;
using System.Linq;

using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    public class DialerRecordingWrapper : IDialerRecordingWrapper
    {
        private readonly IMnTciTools _mnTciTools;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IDialerSettings _dialerSettings;

        public DialerRecordingWrapper(
            IMnTciTools mnTciTools,
            ISurveyRepository surveyRepository,
            IDialerSettings dialerSettings)
        {
            _mnTciTools = mnTciTools;
            _surveyRepository = surveyRepository;
            _dialerSettings = dialerSettings;
        }

        public IEnumerable<AudioRecordInfo> GetInterviewRecordings(int dialerId, int tenantId, int surveySid, int interviewId)
        {
            var dialerRecording = _mnTciTools.CreateDialerRecording(dialerId);

            var survey = _surveyRepository.GetById(surveySid);

            var result = dialerRecording.GetAudioRecords(tenantId, survey.CampaignId, interviewId, dialerId).Where(x => x?.Url != null).ToList();

            foreach (var audioRecord in result)
            {
                audioRecord.Url = audioRecord.Url.Replace("\\", "/");
                audioRecord.DialerId = dialerId;
            }

            return result;
        }

        public AudioFile GetAudioFile(int companyId, int dialerId, string audioUrl)
        {
            var dialerRecording = _mnTciTools.CreateDialerRecording(dialerId);

            return dialerRecording.GetAudioFile(companyId, dialerId, audioUrl);
        }
        
        public IDictionary<CampaignInterviewIdentity, IEnumerable<AudioRecordInfo>> GetBulkInterviewRecordings(int dialerId, int tenantId, IEnumerable<CampaignInterviewIdentity> interviewIdentities)
        {
            var dialerRecording = _mnTciTools.CreateDialerRecording(dialerId);

            // converts Backend survey identifier to dialer compaign identifiers
            var identitiesWithCompaign = (from identity in interviewIdentities
                                          select
                                              new CampaignInterviewIdentity
                                              {
                                                  InterviewId = identity.InterviewId,
                                                  CampaignId = identity.CampaignId,
                                              }).Distinct().ToArray();

            int pageSize = _dialerSettings.AudioRecordingsPageSize;
            int i = 0;
            var recordsData = new Dictionary<CampaignInterviewIdentity, IEnumerable<AudioRecordInfo>>();

            // we do not want to increase maximum WCF message size, 
            // so we'll retrieve data by pages of size 100
            while (true)
            {
                var page = identitiesWithCompaign.Skip(i * pageSize).Take(pageSize);

                if (page.Any())
                {
                    var pageData = dialerRecording.GetBulkAudioRecords(tenantId, page, dialerId);

                    for (int k = 0; k < pageData.CampaignInterviewIdentities.Length; k++)
                    {
                        recordsData.Add(
                            pageData.CampaignInterviewIdentities[k],
                            pageData.AudioRecords[k].Select(
                                audio =>
                                new AudioRecordInfo
                                {
                                    DateTime = audio.DateTime,
                                    Url = audio.Url.Replace("\\", "/")
                                }).ToArray());
                    }
                }
                else
                {
                    break;
                }

                i++;
            }

            return recordsData;
        }

        public bool[] AreRecordsExists(int dialerId, int tenantId, int surveySid, int[] interviewIds)
        {
            var dialerRecording = _mnTciTools.CreateDialerRecording(dialerId);

            var survey = _surveyRepository.GetById(surveySid);

            var result = dialerRecording.AreRecordsExists(tenantId, survey.CampaignId, interviewIds, dialerId);

            return result;
        }
    }
}
