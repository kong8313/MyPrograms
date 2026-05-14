using System;
using Confirmit.CATI.Core.Services.TimeService;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class TestTimeService : ITimeService
    {
        private DateTime? _specificDateTime;
        private int _seconds;

        public TestTimeService(int seconds)
        {
            _specificDateTime = null;
            _seconds = seconds;
        }

        public TestTimeService(DateTime specificDateTime)
        {
            _specificDateTime = specificDateTime;
            _seconds = 0;
        }

        public DateTime GetUtcNow()
        {
            if (_specificDateTime.HasValue)
            {
                return _specificDateTime.Value;
            }

            return DateTime.UtcNow.AddSeconds(_seconds);
        }

        public void SetDateTime(DateTime specificDateTime)
        {
            _specificDateTime = specificDateTime;
        }
    }
}