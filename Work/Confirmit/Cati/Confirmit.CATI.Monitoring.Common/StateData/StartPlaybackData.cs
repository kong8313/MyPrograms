using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    /// <summary>
    /// Represents state data of start voice file playback telephony command.
    /// </summary>
    [Serializable]
    public class StartPlaybackData : BaseStateData
    {
        public string FileName { get; set; }
    }
}

