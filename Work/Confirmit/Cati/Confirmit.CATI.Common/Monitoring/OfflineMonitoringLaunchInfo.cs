using System;
using System.Collections.Generic;
using System.Linq;

namespace Confirmit.CATI.Common.Monitoring
{
    [Serializable]
    public class OfflineMonitoringLaunchInfo
    {
        public string VideoFilePath { get; set; }
        public List<string> AudioFilesPaths { get; set; }
        public string MetadataFilePath { get; set; }

        public OfflineMonitoringLaunchInfo()
        {
            AudioFilesPaths = new List<string>();
        }

        public IEnumerable<string> GetFileNames()
        {
            var result = new List<string> {VideoFilePath, MetadataFilePath};
            result.AddRange(AudioFilesPaths);
            return result.Where(f => !string.IsNullOrWhiteSpace(f)).Distinct().ToList();
        }
    }
}
