using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Batch
{
    public interface IMemoryBatch : IBatch, IDisposable
    {
        IEnumerable<int> Items { get; }
    }
}
