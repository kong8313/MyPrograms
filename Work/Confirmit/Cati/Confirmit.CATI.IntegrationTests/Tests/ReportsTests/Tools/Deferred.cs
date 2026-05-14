namespace Confirmit.CATI.IntegrationTests.Tests.ReportsTests.Tools
{
    internal class Deferred<T>
    {
        internal T Value{ set; get; }

        internal Deferred(T value)
        {
            Value = value;
        }

        internal Deferred()
        {}
    }
}
