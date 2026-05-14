using System;
using System.Collections.Generic;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Telephony.DialerService.Contract;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    public class CodiVersion37RecordingProxy : ICodiVersionRecordingProxy
    {
        private readonly IChannelFactoryWrapper<IDialerService> _recordingChannel;

        public CodiVersion37RecordingProxy(IChannelFactoryWrapper<IDialerService> recordingChannel)
        {
            _recordingChannel = recordingChannel;
        }

        public void InitializeRecording(int dialerId)
        {
            _recordingChannel.Execute(x => x.InitializeRecording(dialerId));
        }

        public IEnumerable<AudioRecordInfo> GetAudioRecords(int companyId, long campaignId, int interviewId, int dialerId)
        {
            return _recordingChannel.Execute(
                x => x.GetAudioRecords(
                    companyId,
                    campaignId,
                    interviewId,
                    dialerId));
        }

        public AudioFile GetAudioFile(int companyId, int dialerId, string audioUrl)
        {
            return _recordingChannel.Execute(
                x => x.GetAudioFile(
                    companyId,
                    dialerId,
                    audioUrl));
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
                    interviewIds,
                    dialerId));
        }

        public void ReleaseDialerChannel()
        {
            _recordingChannel.Release();
        }
    }
}