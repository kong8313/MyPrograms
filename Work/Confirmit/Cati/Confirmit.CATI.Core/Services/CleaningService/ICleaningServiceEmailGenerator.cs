using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.CleaningService
{
    public interface ICleaningServiceEmailGenerator
    {
        string GetWarningBody(List<CleaningServiceEmailInfo> surveys);

        string GetCleanupBody(List<CleaningServiceEmailInfo> surveys);
    }
}