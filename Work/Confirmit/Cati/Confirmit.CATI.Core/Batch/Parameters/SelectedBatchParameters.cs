using System.Collections.Generic;
using System.Linq;

namespace Confirmit.CATI.Core.Batch
{
    public class SelectedBatchParameters : BatchParameters
    {
        public int[] Items{ get;set;}

        public SelectedBatchParameters() { }

        public SelectedBatchParameters(IEnumerable<int> items)
        {
            Items = items.ToArray();
        }

        public override BatchType Type { get { return BatchType.Selected; } }
    }
}