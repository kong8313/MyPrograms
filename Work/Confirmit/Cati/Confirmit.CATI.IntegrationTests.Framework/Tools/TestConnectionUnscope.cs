using Confirmit.CATI.Core.DAL.Framework;
using System;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class TestConnectionUnscope : IDisposable
    {
        private readonly ConnectionScope _oldConnectionScope;

        public TestConnectionUnscope()
        {
            _oldConnectionScope = ConnectionScope.Current;
            ConnectionScope.Current = null;
        }

        public void Dispose()
        {
            ConnectionScope.Current = _oldConnectionScope;
        }
    }
}
