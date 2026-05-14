using System;
using Confirmit.CATI.Core.EmailReports;

namespace Confirmit.CATI.IntegrationTests.Tests.EmailReports
{
    public class FakeLocalTimeProvider : ILocalTimeProvider
    {
        private readonly int _fakeHoursOffset;
        private DateTime _fakeTimeToReturn;

        public FakeLocalTimeProvider(DateTime fakeTimeToReturn)
        {
            _fakeTimeToReturn = fakeTimeToReturn;
        }

        public FakeLocalTimeProvider(DateTime fakeTimeToReturn, int fakeHoursOffset) : this(fakeTimeToReturn)
        {
            _fakeHoursOffset = fakeHoursOffset;
        }

        public DateTime GetCurrentLocalTime()
        {
            return _fakeTimeToReturn;
        }

        public string GetCurrentLocalTimezoneName()
        {
            throw new NotImplementedException();
        }

        public DateTime ConvertToLocalTime(DateTime utc)
        {
            return utc.AddHours(_fakeHoursOffset);
        }
    }
}