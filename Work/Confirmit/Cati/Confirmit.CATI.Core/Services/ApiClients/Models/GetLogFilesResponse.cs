using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;

namespace Confirmit.CATI.Core.Services.ApiClients.Models
{
    public class GetLogFilesResponse : DialerResponse
    {
        public IEnumerable<LogFileInfo> LogFileInfos { get; set; }
    }
}