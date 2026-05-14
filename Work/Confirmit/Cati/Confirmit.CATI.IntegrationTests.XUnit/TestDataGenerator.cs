using System.Collections;
using System.Collections.Generic;

namespace Confirmit.CATI.IntegrationTests.XUnit
{
    public class TestDataGenerator : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new List<object[]>
        {
            new object[] { SecurityMode.Restricted }
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public enum SecurityMode
    {
        Unrestricted,
        Restricted
    }
}