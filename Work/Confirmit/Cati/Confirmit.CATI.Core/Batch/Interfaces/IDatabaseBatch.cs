using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Batch
{
    public interface IDatabaseBatch : IBatch, IDisposable
    {
        int Id { get; }

        new int Size { get; set; }

        void Clear();
    }
}
