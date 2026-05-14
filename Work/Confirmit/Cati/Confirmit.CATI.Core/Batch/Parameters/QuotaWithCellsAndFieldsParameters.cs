using System;

namespace Confirmit.CATI.Core.Batch
{

    [Serializable]
    public class QuotaWithCellsAndFieldsParameters
    {
        public string[] QuotaFields { get; set; }
        public string QuotaName { get; set; }
        public int[] CellIds { get; set; }
    }
}