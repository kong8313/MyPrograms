using System.Collections;
using System.Collections.Generic;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.IntegrationTests.Framework.Data
{
    public class PredictiveDialTypes: IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { DialType.Landline };
            yield return new object[] { DialType.Assisted };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}