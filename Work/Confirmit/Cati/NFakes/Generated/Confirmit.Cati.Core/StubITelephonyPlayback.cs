using System;
using Confirmit.CATI.Core.Telephony;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubITelephonyPlayback : ITelephonyPlayback 
    {
        private ITelephonyPlayback _inner;

        public StubITelephonyPlayback()
        {
            _inner = null;
        }

        public ITelephonyPlayback Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate DialerErrorCode StartPlaybackInt32Int64StringInt32Int32StringInt32OutDelegate(int dialerId, long campaignId, string agentId, int interviewId, int callId, string fileName, out int timeOfPlayingInSeconds);
        public StartPlaybackInt32Int64StringInt32Int32StringInt32OutDelegate StartPlaybackInt32Int64StringInt32Int32StringInt32Out;

        DialerErrorCode ITelephonyPlayback.StartPlayback(int dialerId, long campaignId, string agentId, int interviewId, int callId, string fileName, out int timeOfPlayingInSeconds)
        {
            timeOfPlayingInSeconds = default(int);


            if (StartPlaybackInt32Int64StringInt32Int32StringInt32Out != null)
            {
                return StartPlaybackInt32Int64StringInt32Int32StringInt32Out(dialerId, campaignId, agentId, interviewId, callId, fileName, out timeOfPlayingInSeconds);
            } else if (_inner != null)
            {
                return ((ITelephonyPlayback)_inner).StartPlayback(dialerId, campaignId, agentId, interviewId, callId, fileName, out timeOfPlayingInSeconds);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StopPlaybackInt32Int64StringInt32Int32Delegate(int dialerId, long campaignId, string agentId, int interviewId, int callId);
        public StopPlaybackInt32Int64StringInt32Int32Delegate StopPlaybackInt32Int64StringInt32Int32;

        DialerErrorCode ITelephonyPlayback.StopPlayback(int dialerId, long campaignId, string agentId, int interviewId, int callId)
        {


            if (StopPlaybackInt32Int64StringInt32Int32 != null)
            {
                return StopPlaybackInt32Int64StringInt32Int32(dialerId, campaignId, agentId, interviewId, callId);
            } else if (_inner != null)
            {
                return ((ITelephonyPlayback)_inner).StopPlayback(dialerId, campaignId, agentId, interviewId, callId);
            }

            return default(DialerErrorCode);
        }

        public delegate bool IsPauseOrResumePlaybackSupportedNullableOfInt32Delegate(int? dialerId);
        public IsPauseOrResumePlaybackSupportedNullableOfInt32Delegate IsPauseOrResumePlaybackSupportedNullableOfInt32;

        bool ITelephonyPlayback.IsPauseOrResumePlaybackSupported(int? dialerId)
        {


            if (IsPauseOrResumePlaybackSupportedNullableOfInt32 != null)
            {
                return IsPauseOrResumePlaybackSupportedNullableOfInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ITelephonyPlayback)_inner).IsPauseOrResumePlaybackSupported(dialerId);
            }

            return default(bool);
        }

        public delegate bool IsToggleInterviewerListensToPlaybackOrRespondentSupportedNullableOfInt32Delegate(int? dialerId);
        public IsToggleInterviewerListensToPlaybackOrRespondentSupportedNullableOfInt32Delegate IsToggleInterviewerListensToPlaybackOrRespondentSupportedNullableOfInt32;

        bool ITelephonyPlayback.IsToggleInterviewerListensToPlaybackOrRespondentSupported(int? dialerId)
        {


            if (IsToggleInterviewerListensToPlaybackOrRespondentSupportedNullableOfInt32 != null)
            {
                return IsToggleInterviewerListensToPlaybackOrRespondentSupportedNullableOfInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ITelephonyPlayback)_inner).IsToggleInterviewerListensToPlaybackOrRespondentSupported(dialerId);
            }

            return default(bool);
        }

        public delegate DialerErrorCode PauseOrResumePlaybackInt32Int64StringInt32Int32Delegate(int dialerId, long campaignId, string agentId, int interviewId, int callId);
        public PauseOrResumePlaybackInt32Int64StringInt32Int32Delegate PauseOrResumePlaybackInt32Int64StringInt32Int32;

        DialerErrorCode ITelephonyPlayback.PauseOrResumePlayback(int dialerId, long campaignId, string agentId, int interviewId, int callId)
        {


            if (PauseOrResumePlaybackInt32Int64StringInt32Int32 != null)
            {
                return PauseOrResumePlaybackInt32Int64StringInt32Int32(dialerId, campaignId, agentId, interviewId, callId);
            } else if (_inner != null)
            {
                return ((ITelephonyPlayback)_inner).PauseOrResumePlayback(dialerId, campaignId, agentId, interviewId, callId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondentInt32Int64StringInt32Int32Delegate(int dialerId, long campaignId, string agentId, int interviewId, int callId);
        public ToggleInterviewerListensToPlaybackOrRespondentInt32Int64StringInt32Int32Delegate ToggleInterviewerListensToPlaybackOrRespondentInt32Int64StringInt32Int32;

        DialerErrorCode ITelephonyPlayback.ToggleInterviewerListensToPlaybackOrRespondent(int dialerId, long campaignId, string agentId, int interviewId, int callId)
        {


            if (ToggleInterviewerListensToPlaybackOrRespondentInt32Int64StringInt32Int32 != null)
            {
                return ToggleInterviewerListensToPlaybackOrRespondentInt32Int64StringInt32Int32(dialerId, campaignId, agentId, interviewId, callId);
            } else if (_inner != null)
            {
                return ((ITelephonyPlayback)_inner).ToggleInterviewerListensToPlaybackOrRespondent(dialerId, campaignId, agentId, interviewId, callId);
            }

            return default(DialerErrorCode);
        }

    }
}