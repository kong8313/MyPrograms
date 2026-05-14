using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confirmit.CATI.Core.DAL.Handmade.Entity.Query
{
    public class PromotedCellEntity
    {
        public int QuotaId { get; set; }
        public string QuotaName { get; set; }
        public int CellId { get; set; }
        public int Priority { get; set; }
        public double CallsCountNeededToPromote { get; set; }
    }
}
