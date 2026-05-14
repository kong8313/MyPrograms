extern alias CodiV35;

using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.WcfTools;
using ConfirmitDialerInterface;

using IDialerService35 = CodiV35::Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    public class CodiVersion35RecordingProxy : ICodiVersionRecordingProxy
    {
        private readonly IChannelFactoryWrapper<IDialerService35> _recordingChannel;

        public CodiVersion35RecordingProxy(IChannelFactoryWrapper<IDialerService35> recordingChannel)
        {
            _recordingChannel = recordingChannel;
        }

        public void InitializeRecording(int dialerId)
        {
            _recordingChannel.Execute(x => x.InitializeRecording());
        }

        public IEnumerable<AudioRecordInfo> GetAudioRecords(int companyId, long campaignId, int interviewId, int dialerId)
        {
            var result = _recordingChannel.Execute(
                x => x.GetAudioRecords(
                    companyId,
                    campaignId,
                    interviewId));

            // Convert result to currently supported type
            return result.Select(x => new AudioRecordInfo
            {
                DateTime = x.DateTime,
                Url = x.Url
            });
        }

        public AudioFile GetAudioFile(int companyId, int dialerId, string audioUrl)
        {
            // The method is available in the 3.7 version and older
            throw new NotImplementedException();
        }
        
        public BulkAudioResult GetBulkAudioRecords(int companyId, IEnumerable<CampaignInterviewIdentity> interviewIdentities, int dialerId)
        {
            // The method is not currently used

            throw new NotImplementedException();

            //            return _recordingChannel.Execute(
            //                x => x.GetBulkAudioRecords(
            //                    companyId,
            //                    interviewIdentities));
        }

        public bool[] AreRecordsExists(int companyId, long campaignId, int[] interviewIds, int dialerId)
        {
            return _recordingChannel.Execute(
                x => x.AreRecordsExists(
                    companyId,
                    campaignId,
                    interviewIds));
        }

        public void ReleaseDialerChannel()
        {
            _recordingChannel.Release();
        }
    }
}